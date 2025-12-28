// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ChannelCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
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

#nullable disable
namespace SapiLayer1;

public sealed class ChannelCollection : ChannelBaseCollection
{
  internal const int TLSLAVEWAITTIME = 100;
  private List<BackgroundConnect> pendingConnections;
  private List<ConnectionResource> pendingResources;
  private volatile bool autoConnecting;
  private List<EcuAutoConnect> ecusInAutoConnect;
  private List<Channel> offlineOutOfCollection;

  internal ChannelCollection()
  {
    this.autoConnecting = false;
    this.pendingConnections = new List<BackgroundConnect>();
    this.pendingResources = new List<ConnectionResource>();
    this.offlineOutOfCollection = new List<Channel>();
  }

  internal Channel GetItem(CaesarChannel requested)
  {
    if (requested != null)
    {
      foreach (Channel channel in (ChannelBaseCollection) this)
      {
        if (channel.IsOwner(requested))
          return channel;
      }
    }
    return (Channel) null;
  }

  public void AbortPendingConnections()
  {
    this.AbortPendingConnections((System.Func<ConnectionResource, bool>) (cr => true));
  }

  internal void AbortPendingConnections(
    System.Func<ConnectionResource, bool> connectionResourceMatchFunc)
  {
    lock (this.pendingConnections)
    {
      foreach (BackgroundConnect backgroundConnect in this.pendingConnections.Where<BackgroundConnect>((System.Func<BackgroundConnect, bool>) (bg => connectionResourceMatchFunc(bg.ConnectionResource))).ToList<BackgroundConnect>())
        backgroundConnect.Abort();
    }
  }

  public int GetConnectingCountForIdentifier(string identifier)
  {
    return this.GetConnectingCountForIdentifier(identifier, (BackgroundConnect) null);
  }

  private int GetConnectingCountForIdentifier(
    string identifier,
    BackgroundConnect previousBackgroundConnect)
  {
    lock (this.pendingConnections)
      return this.pendingConnections.Count<BackgroundConnect>((System.Func<BackgroundConnect, bool>) (bg => bg != previousBackgroundConnect && bg.ConnectionResource.Ecu.Identifier == identifier));
  }

  internal void WaitForBackgroundConnection()
  {
    while (true)
    {
      BackgroundConnect backgroundConnect = (BackgroundConnect) null;
      lock (this.pendingConnections)
      {
        if (this.pendingConnections.Count <= 0)
          break;
        backgroundConnect = this.pendingConnections[0];
      }
      backgroundConnect.BackgroundThread.Join();
    }
  }

  internal Channel ConnectComplete(BackgroundConnect backgroundConnect)
  {
    Sapi sapi = Sapi.GetSapi();
    BackgroundConnect backgroundConnect1 = backgroundConnect;
    if (backgroundConnect.Initialised && !backgroundConnect.VariantMatched)
    {
      sapi.RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Connected to base variant for {0}, other options may be available", (object) backgroundConnect.ConnectionResource.Ecu.Name));
      string identifier = backgroundConnect.ConnectionResource.Ecu.Identifier;
      DiagnosisVariant diagnosisVariant = (DiagnosisVariant) null;
      if (backgroundConnect.CaesarChannel != null)
        diagnosisVariant = sapi.Ecus.GetDiagnosisVariantFromIDBlock((object) identifier, backgroundConnect.CaesarChannel.IdBlock);
      else if (backgroundConnect.MCDLogicalLink != null)
        diagnosisVariant = sapi.Ecus.GetDiagnosisVariantFromIDBlock((object) identifier, backgroundConnect.MCDLogicalLink);
      if (diagnosisVariant != null)
      {
        sapi.RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Got actual match for {0} variant {1} - looking for equivalent connection resource", (object) diagnosisVariant.Ecu.Name, (object) diagnosisVariant.Name));
        backgroundConnect = this.ReconnectToEquivalentResource(diagnosisVariant.Ecu, backgroundConnect);
      }
      else
      {
        sapi.RaiseDebugInfoEvent((object) this, "No better ECU found as a direct match for " + backgroundConnect.ConnectionResource.Ecu.Name);
        uint? nullable = new uint?();
        if (backgroundConnect.CaesarChannel != null)
          nullable = backgroundConnect.CaesarChannel.IdBlock.DiagVersionLong;
        else if (backgroundConnect.MCDLogicalLink != null)
        {
          McdValue identificationResult = backgroundConnect.MCDLogicalLink.GetVariantIdentificationResult("ActiveDiagnosticInformation_Read", "Identification");
          if (identificationResult != null)
            nullable = new uint?(Convert.ToUInt32(identificationResult.Value, (IFormatProvider) CultureInfo.InvariantCulture));
        }
        if (nullable.HasValue)
        {
          if (!backgroundConnect.ConnectionResource.Ecu.HasMultipleByteDiagnosticVersion)
          {
            sapi.RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IdBlock has long diagnostic version 0x{1:x}, maybe we can try using that?", (object) backgroundConnect.ConnectionResource.Ecu.Name, (object) nullable));
            uint maskedVersion = nullable.Value & 4294967040U;
            Ecu bestEcuFromVariant = sapi.Ecus.GetBestEcuFromVariant(identifier, (Predicate<DiagnosisVariant>) (v =>
            {
              if (!v.Ecu.IsSameEcuOnDifferentDiagnosisSource(backgroundConnect.ConnectionResource.Ecu))
              {
                long? diagnosticVersionLong = v.DiagnosticVersionLong;
                if (diagnosticVersionLong.HasValue)
                {
                  diagnosticVersionLong = v.DiagnosticVersionLong;
                  return (diagnosticVersionLong.Value & 4294967040L) == (long) maskedVersion;
                }
              }
              return false;
            }));
            if (bestEcuFromVariant != null)
            {
              if (bestEcuFromVariant == backgroundConnect.ConnectionResource.Ecu)
              {
                sapi.RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Masked long diagnostic version 0x{0:x} indicates that the existing connection to {1}.{2} is the best match; remaining connected using original connection resource", (object) maskedVersion, (object) backgroundConnect.ConnectionResource.Ecu.DiagnosisSource, (object) backgroundConnect.ConnectionResource.Ecu.Name));
              }
              else
              {
                sapi.RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Got possible match for {0}.{1} using masked long diagnostic version - looking for equivalent connection resource", (object) bestEcuFromVariant.DiagnosisSource, (object) bestEcuFromVariant.Name));
                backgroundConnect = this.ReconnectToEquivalentResource(bestEcuFromVariant, backgroundConnect);
              }
            }
            else
            {
              sapi.RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No better ECU found for {0} using masked long diagnostic version", (object) backgroundConnect.ConnectionResource.Ecu.Name));
              if (((int) nullable.Value & 65536 /*0x010000*/) == 65536 /*0x010000*/ && !backgroundConnect.ConnectionResource.Ecu.DiagnosisVariants.Any<DiagnosisVariant>((System.Func<DiagnosisVariant, bool>) (v => v.IsBoot)))
                sapi.RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: connection is allowed because it is a boot variant for an ECU whose diagnostic description does not describe boot variants", (object) backgroundConnect.ConnectionResource.Ecu.Name));
              else if (backgroundConnect.AutoConnect || this.ProhibitImplausibleOrNoVariantManualConnections)
                backgroundConnect.SetConnectCompleteFailure(SapiError.NoPlausibleVariantMatch);
            }
          }
        }
        else
        {
          sapi.RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: No long diagnostic version available", (object) backgroundConnect.ConnectionResource.Ecu.Name));
          if (backgroundConnect.AutoConnect || this.ProhibitImplausibleOrNoVariantManualConnections)
            backgroundConnect.SetConnectCompleteFailure(SapiError.NoDiagnosticVersionForVariantMatch);
        }
      }
    }
    Channel sender = (Channel) null;
    if (backgroundConnect != null)
    {
      if (backgroundConnect.Initialised)
      {
        this.RaiseConnectProgressEvent(backgroundConnect.ConnectionResource, 85.0);
        sender = backgroundConnect.CaesarChannel == null ? this.Add(backgroundConnect.MCDLogicalLink, backgroundConnect.ConnectionResource, backgroundConnect.ChannelOptions) : this.Add(backgroundConnect.CaesarChannel, backgroundConnect.ConnectionResource, backgroundConnect.ChannelOptions);
        this.RaiseConnectProgressEvent(backgroundConnect.ConnectionResource, 100.0);
        this.RaiseConnectCompleteEvent((object) sender, backgroundConnect, (Exception) backgroundConnect.CaesarException);
      }
      else
      {
        Sapi.GetSapi().LogFiles.LabelLog(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to connect {0} - {1}", (object) backgroundConnect.ConnectionResource.ToString(), (object) backgroundConnect.CaesarException.Message), backgroundConnect.ConnectionResource.Ecu);
        this.RaiseConnectCompleteEvent((object) backgroundConnect.ConnectionResource, backgroundConnect, (Exception) backgroundConnect.CaesarException);
      }
    }
    lock (this.pendingConnections)
      this.pendingConnections.Remove(backgroundConnect1);
    return sender;
  }

  private BackgroundConnect ReconnectToEquivalentResource(
    Ecu matchingEcu,
    BackgroundConnect backgroundConnect)
  {
    Sapi sapi = Sapi.GetSapi();
    ConnectionResource resource = (ConnectionResource) null;
    ConnectionResourceCollection connectionResources = matchingEcu.GetConnectionResources();
    if (matchingEcu.DiagnosisSource == DiagnosisSource.McdDatabase)
    {
      ConnectionResource connectionResource = connectionResources.FirstOrDefault<ConnectionResource>((System.Func<ConnectionResource, bool>) (cr => cr.IsEthernet));
      if (connectionResource != null)
      {
        bool flag = RollCallDoIP.GlobalInstance.GetActiveAddresses().Any<int>((System.Func<int, bool>) (addr => matchingEcu.IsRelated((RollCall) RollCallDoIP.GlobalInstance, addr, (Ecu) null)));
        if (flag)
          resource = connectionResource;
        sapi.RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "An ethernet resource for {0}.{1} exists. A related DoIP entity was {2} found. The ethernet resource will {2} be used.", (object) matchingEcu.DiagnosisSource, (object) matchingEcu.Name, flag ? (object) string.Empty : (object) "not"));
      }
    }
    if (resource == null)
      resource = connectionResources.GetEquivalent(backgroundConnect.ConnectionResource);
    if (resource != null)
    {
      sapi.RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Got equivalent connection resource, disconnecting {0}", (object) backgroundConnect.ConnectionResource.Ecu));
      BackgroundConnect previousBackgroundConnect = backgroundConnect;
      this.RaiseConnectCompleteEvent((object) backgroundConnect.ConnectionResource, backgroundConnect, (Exception) new CaesarException(resource.Restricted ? SapiError.FoundBetterVariantMatchButResourceRestricted : SapiError.FoundBetterVariantMatch));
      backgroundConnect.Dispose();
      backgroundConnect = (BackgroundConnect) null;
      if (resource.Restricted)
      {
        sapi.RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Equivalent connection resource {0} for {1} is restricted and will not be used", (object) resource.ToString(), (object) matchingEcu.Name));
      }
      else
      {
        sapi.RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Trying to connect to {0}", (object) matchingEcu.Name));
        try
        {
          backgroundConnect = this.InternalConnect(resource, previousBackgroundConnect.ChannelOptions, true, ConnectSource.VariantTransfer, previousBackgroundConnect);
          previousBackgroundConnect.Child = backgroundConnect;
        }
        catch (CaesarException ex)
        {
        }
      }
    }
    else
    {
      sapi.RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No equivalent connection resource to {0} found for ECU {1}. Disconnecting {2} as it is known to be the incorrect variant. ", (object) backgroundConnect.ConnectionResource.ToString(), (object) matchingEcu.Name, (object) backgroundConnect.ConnectionResource.Ecu));
      this.RaiseConnectCompleteEvent((object) backgroundConnect.ConnectionResource, backgroundConnect, (Exception) new CaesarException(SapiError.FoundBetterVariantMatchButResourceUnavailable));
      backgroundConnect.Dispose();
      backgroundConnect = (BackgroundConnect) null;
    }
    return backgroundConnect;
  }

  internal Channel AddFromRollCall(ConnectionResource resource, DiagnosisVariant variant)
  {
    Channel sender = new Channel((ChannelBaseCollection) this, resource, variant);
    lock (this.Items)
      this.Items.Add(sender);
    this.RaiseConnectCompleteEvent((object) sender, (Exception) null);
    return sender;
  }

  internal static void InitComParameterCollection(
    McdLogicalLink logicalLink,
    ConnectionResource resource)
  {
    foreach (DictionaryEntry infoComParameter in resource.EcuInfoComParameters)
    {
      try
      {
        logicalLink.SetComParameter(infoComParameter.Key.ToString(), infoComParameter.Value);
      }
      catch (McdException ex)
      {
        Sapi sapi = Sapi.GetSapi();
        sapi.RaiseExceptionEvent((object) sapi.Channels, (Exception) new CaesarException(ex));
      }
    }
  }

  internal static void InitComParameterCollection(CaesarChannel ch, ConnectionResource resource)
  {
    foreach (DictionaryEntry infoComParameter in resource.EcuInfoComParameters)
    {
      try
      {
        ch.SetComParameter(infoComParameter.Key.ToString(), infoComParameter.Value.ToString());
      }
      catch (CaesarErrorException ex)
      {
        Sapi sapi = Sapi.GetSapi();
        sapi.RaiseExceptionEvent((object) sapi.Channels, (Exception) new CaesarException(ex));
      }
    }
    if (!(resource.Ecu.ProtocolName == "E1939"))
      return;
    byte? sourceAddress = resource.SourceAddress;
    if (!sourceAddress.HasValue)
      return;
    CaesarChannel caesarChannel = ch;
    sourceAddress = resource.SourceAddress;
    string str = sourceAddress.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    caesarChannel.SetComParameter("CP_DESTADDRESS", str);
  }

  internal static void InitComParameterCollection(CaesarChannel chanHandle, string parameterList)
  {
    Sapi sapi = Sapi.GetSapi();
    foreach (string str in parameterList.Split(";".ToCharArray()))
    {
      string[] strArray = str.Split("=".ToCharArray());
      if (strArray.Length == 2)
      {
        try
        {
          if (!chanHandle.SetComParameter(strArray[0], strArray[1]))
            sapi.RaiseExceptionEvent((object) sapi.Channels, (Exception) new CaesarException(SapiError.ComParameterSpecUnavailable));
        }
        catch (CaesarErrorException ex)
        {
          sapi.RaiseExceptionEvent((object) sapi.Channels, (Exception) new CaesarException(ex));
        }
      }
    }
  }

  internal void ClearEcusInAutoConnect()
  {
    this.StopAutoConnect();
    if (this.ecusInAutoConnect == null)
      return;
    this.ecusInAutoConnect.Clear();
    this.ecusInAutoConnect = (List<EcuAutoConnect>) null;
  }

  internal Channel Open(ConnectionResource resource)
  {
    CaesarChannel ch = this.OpenCaesarChannel(resource, false);
    Thread.Sleep(100);
    ChannelCollection.InitComParameterCollection(ch, resource);
    BackgroundConnect backgroundConnect = new BackgroundConnect((ChannelCollection) null, ch, resource);
    backgroundConnect.Start();
    backgroundConnect.BackgroundThread.Join();
    if (backgroundConnect.CaesarException != null)
      throw backgroundConnect.CaesarException;
    return new Channel(ch, resource, (ChannelBaseCollection) null, ChannelOptions.All);
  }

  public Channel Connect(ConnectionResource resource, bool synchronous)
  {
    return this.Connect(resource, ChannelOptions.All, synchronous, ConnectSource.Manual);
  }

  public Channel Connect(ConnectionResource resource, ChannelOptions options, bool synchronous)
  {
    return this.Connect(resource, options, synchronous, ConnectSource.Manual);
  }

  internal Channel Connect(
    ConnectionResource resource,
    ChannelOptions options,
    bool synchronous,
    ConnectSource connectSource)
  {
    if (resource.Ecu.OfflineSupportOnly)
      throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The ECU ({0}) is only supported in offline mode.", (object) resource.Ecu.Name));
    BackgroundConnect backgroundConnect = this.InternalConnect(resource, options, synchronous, connectSource, (BackgroundConnect) null);
    if (synchronous)
    {
      if (backgroundConnect.Channel != null)
        return backgroundConnect.Channel;
      if (backgroundConnect.CaesarException != null)
        throw backgroundConnect.CaesarException;
    }
    return (Channel) null;
  }

  public Channel ConnectOffline(DiagnosisVariant diagnosisVariant)
  {
    Channel sender = this.InternalConnectOffline(diagnosisVariant, (LogFile) null);
    if (sender != null)
      this.RaiseConnectCompleteEvent((object) sender, (Exception) null);
    return sender;
  }

  public Channel ConnectOffline(
    string parameterFileName,
    ParameterFileFormat parameterFileFormat,
    Collection<string> unknownList)
  {
    using (StreamReader parameterFile = new StreamReader(parameterFileName))
      return this.ConnectOffline(parameterFile, parameterFileFormat, unknownList);
  }

  public Channel ConnectOffline(
    StreamReader parameterFile,
    ParameterFileFormat parameterFileFormat,
    Collection<string> unknownList)
  {
    Sapi sapi = Sapi.GetSapi();
    TargetEcuDetails targetEcuDetails = ParameterCollection.GetTargetEcuDetails(parameterFile, parameterFileFormat);
    Ecu ecu = sapi.Ecus[targetEcuDetails.Ecu];
    if (ecu == null)
      throw new DataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Ecu data {0} referenced by parameter file was not found.", (object) targetEcuDetails.Ecu));
    DiagnosisVariant diagnosisVariant = ecu.DiagnosisVariants[targetEcuDetails.DiagnosisVariant];
    if (diagnosisVariant == null)
    {
      Ecu ecu1 = sapi.Ecus.FirstOrDefault<Ecu>((System.Func<Ecu, bool>) (e => e.Name == targetEcuDetails.Ecu && e != ecu));
      if (ecu1 != null)
        diagnosisVariant = ecu1.DiagnosisVariants[targetEcuDetails.DiagnosisVariant];
    }
    Channel sender = diagnosisVariant != null ? this.InternalConnectOffline(diagnosisVariant, (LogFile) null) : throw new DataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Ecu variant data {0} referenced by parameter file was not found.", (object) targetEcuDetails.DiagnosisVariant));
    if (sender != null)
    {
      sender.Parameters.Load(parameterFile, parameterFileFormat, unknownList, false);
      this.RaiseConnectCompleteEvent((object) sender, (Exception) null);
    }
    return sender;
  }

  public Channel OpenOffline(DiagnosisVariant diagnosisVariant)
  {
    if (diagnosisVariant == null)
      return (Channel) null;
    Channel channel = (Channel) null;
    if (diagnosisVariant.Ecu.RollCallManager != null)
      channel = new Channel(diagnosisVariant, (ChannelBaseCollection) this, (LogFile) null);
    else if (diagnosisVariant.Ecu.IsMcd)
    {
      channel = new Channel(diagnosisVariant.GetMcdDBLocationForProtocol(diagnosisVariant.Ecu.ProtocolName), diagnosisVariant, (ChannelBaseCollection) this, (LogFile) null);
    }
    else
    {
      CaesarEcu ecuh = diagnosisVariant.OpenEcuHandle();
      if (ecuh != null)
        channel = new Channel(ecuh, diagnosisVariant, (ChannelBaseCollection) this, (LogFile) null);
    }
    lock (this.offlineOutOfCollection)
      this.offlineOutOfCollection.Add(channel);
    return channel;
  }

  internal override void Remove(Channel channel)
  {
    lock (this.offlineOutOfCollection)
    {
      if (this.offlineOutOfCollection.Contains(channel))
        this.offlineOutOfCollection.Remove(channel);
      else
        base.Remove(channel);
    }
  }

  internal void CleanupOffline()
  {
    while (true)
    {
      Channel channel = (Channel) null;
      lock (this.offlineOutOfCollection)
      {
        if (this.offlineOutOfCollection.Count <= 0)
          break;
        channel = this.offlineOutOfCollection[0];
      }
      channel.InternalDisconnect();
    }
  }

  public void StartAutoConnect() => this.StartAutoConnect(-1);

  public void StartAutoConnect(int numberOfAttemptCycles)
  {
    if (this.autoConnecting)
      return;
    this.autoConnecting = true;
    if (this.ecusInAutoConnect == null)
    {
      Sapi sapi = Sapi.GetSapi();
      this.ecusInAutoConnect = new List<EcuAutoConnect>();
      foreach (IEnumerable<Ecu> ecus in sapi.Ecus.Where<Ecu>((System.Func<Ecu, bool>) (e => !e.IsRollCall && !e.OfflineSupportOnly && !e.IsVirtual && !e.ProhibitAutoConnection)).GroupBy<Ecu, string>((System.Func<Ecu, string>) (e => e.Identifier)))
        this.ecusInAutoConnect.Add(new EcuAutoConnect(this, ecus));
    }
    for (int index = 0; index < this.ecusInAutoConnect.Count; ++index)
      this.ecusInAutoConnect[index].Start(numberOfAttemptCycles);
  }

  public void StopAutoConnect()
  {
    if (!this.autoConnecting)
      return;
    this.autoConnecting = false;
    if (this.ecusInAutoConnect == null)
      return;
    this.AbortPendingConnections();
    foreach (EcuAutoConnect ecuAutoConnect in this.ecusInAutoConnect.ToList<EcuAutoConnect>())
    {
      if (ecuAutoConnect != null && ecuAutoConnect.BackgroundThread != null)
        ecuAutoConnect.BackgroundThread.Join();
    }
  }

  public static IRollCall GetManager(Protocol protocol)
  {
    return (IRollCall) RollCall.GetManager(protocol);
  }

  public bool AutoConnecting => this.autoConnecting;

  public bool ProhibitImplausibleOrNoVariantManualConnections { get; set; }

  private void ConfirmBaudRateAllowed(ConnectionResource desiredResource)
  {
    lock (this.pendingResources)
    {
      foreach (ConnectionResource pendingResource in this.pendingResources)
      {
        if (pendingResource != null && ChannelCollection.SameResourceDifferentBaudRate(desiredResource, pendingResource))
          throw new CaesarException(SapiError.ConnectionResourceNotAvailableOtherBaudRateInUseByConnectingChannel);
      }
    }
    lock (this.pendingConnections)
    {
      foreach (BackgroundConnect pendingConnection in this.pendingConnections)
      {
        ConnectionResource connectionResource = pendingConnection.ConnectionResource;
        if (connectionResource != null && ChannelCollection.SameResourceDifferentBaudRate(desiredResource, connectionResource))
          throw new CaesarException(SapiError.ConnectionResourceNotAvailableOtherBaudRateInUseByConnectingChannel);
      }
    }
    foreach (Channel channel in (ChannelBaseCollection) this)
    {
      if (channel.Online && channel.ConnectionResource != null && ChannelCollection.SameResourceDifferentBaudRate(desiredResource, channel.ConnectionResource))
        throw new CaesarException(SapiError.ConnectionResourceNotAvailableOtherBaudRateInUseByConnectedChannel);
    }
  }

  private BackgroundConnect InternalConnect(
    ConnectionResource resource,
    ChannelOptions options,
    bool sync,
    ConnectSource connectSource,
    BackgroundConnect previousBackgroundConnect)
  {
    Sapi sapi = Sapi.GetSapi();
    switch (sapi.InitState)
    {
      case InitState.NotInitialized:
        throw new InvalidOperationException("Sapi not initialized");
      case InitState.Online:
        CaesarChannel ch = (CaesarChannel) null;
        McdLogicalLink mcdLogicalLink = (McdLogicalLink) null;
        this.RaiseConnectProgressEvent(resource, 15.0);
        BackgroundConnect backgroundConnect;
        lock (this.pendingConnections)
        {
          try
          {
            if (sapi.Ecus.GetConnectedCountForIdentifier(resource.Ecu.Identifier) > 0 || this.GetConnectingCountForIdentifier(resource.Ecu.Identifier, previousBackgroundConnect) > 0)
              throw new CaesarException(SapiError.ChannelAlreadyConnectedToIdentifier);
            if (resource.Ecu.IsMcd)
              mcdLogicalLink = this.OpenMcdChannel(resource);
            else
              ch = this.OpenCaesarChannel(resource, true);
          }
          catch (CaesarException ex)
          {
            this.RaiseConnectCompleteEvent((object) resource, (Exception) ex);
            if (!sync)
              return (BackgroundConnect) null;
            throw;
          }
          Thread.Sleep(100);
          if (resource.Ecu.IsMcd)
            ChannelCollection.InitComParameterCollection(mcdLogicalLink, resource);
          else
            ChannelCollection.InitComParameterCollection(ch, resource);
          this.RaiseConnectProgressEvent(resource, 35.0);
          backgroundConnect = !resource.Ecu.IsMcd ? new BackgroundConnect(connectSource != ConnectSource.VariantTransfer ? this : (ChannelCollection) null, ch, resource, options, connectSource == ConnectSource.Automatic) : new BackgroundConnect(connectSource != ConnectSource.VariantTransfer ? this : (ChannelCollection) null, mcdLogicalLink, resource, options, connectSource == ConnectSource.Automatic);
          if (connectSource != ConnectSource.VariantTransfer)
            this.pendingConnections.Add(backgroundConnect);
          backgroundConnect.Start();
          lock (this.pendingResources)
            this.pendingResources.Remove(resource);
        }
        if (!sync)
          return (BackgroundConnect) null;
        backgroundConnect.BackgroundThread.Join();
        return backgroundConnect;
      case InitState.Offline:
        throw new InvalidOperationException("Resource cannot be acquired in offline operation mode");
      default:
        return (BackgroundConnect) null;
    }
  }

  private McdLogicalLink OpenMcdChannel(ConnectionResource resource)
  {
    McdLogicalLink mcdLogicalLink = (McdLogicalLink) null;
    McdInterface mcdInterface = McdRoot.CurrentInterfaces.FirstOrDefault<McdInterface>((System.Func<McdInterface, bool>) (i => i.Qualifier == resource.MCDInterfaceQualifier));
    if (mcdInterface != null)
    {
      McdInterfaceResource theResource = mcdInterface.Resources.FirstOrDefault<McdInterfaceResource>((System.Func<McdInterfaceResource, bool>) (r => r.Qualifier == resource.MCDResourceQualifier));
      if (theResource != null)
      {
        try
        {
          lock (this.pendingResources)
          {
            if (resource.Ecu.IsByteMessaging)
            {
              mcdLogicalLink = McdRoot.CreateLogicalLink(resource.Ecu.DiagnosisProtocol.Name, theResource, string.Empty);
              if (resource.BaudRate != 0)
              {
                try
                {
                  mcdLogicalLink.SetComParameter("CP_Baudrate", (object) resource.BaudRate.ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
                catch (McdException ex)
                {
                  Sapi.GetSapi().RaiseExceptionEvent((object) this, (Exception) new CaesarException(ex));
                }
              }
            }
            else
              mcdLogicalLink = McdRoot.CreateLogicalLink(resource.Interface.Qualifier, theResource, resource.Ecu.Properties["FixedDiagnosisVariant"]);
            if (mcdLogicalLink != null)
            {
              if (!this.pendingResources.Contains(resource))
                this.pendingResources.Add(resource);
            }
          }
        }
        catch (McdException ex)
        {
          throw new CaesarException(ex);
        }
      }
    }
    return mcdLogicalLink != null ? mcdLogicalLink : throw new CaesarException(SapiError.ConnectionResourceNotAvailable);
  }

  internal CaesarChannel OpenCaesarChannel(ConnectionResource resource, bool addToPendingResources)
  {
    CaesarResource caesarResource = (CaesarResource) null;
    CaesarChannel caesarChannel = (CaesarChannel) null;
    this.ConfirmBaudRateAllowed(resource);
    try
    {
      CaesarRoot.LockResources();
    }
    catch (CaesarErrorException ex)
    {
      byte? negativeResponseCode = new byte?();
      throw new CaesarException(ex, negativeResponseCode);
    }
    try
    {
      uint num1 = resource.Ecu.IsByteMessaging ? CaesarRoot.GetAvailableProtocolResourceCount(resource.Ecu.DiagnosisProtocol.Name) : CaesarRoot.GetAvailableEcuResourceCount(resource.Ecu.Name);
      ushort num2 = 0;
      while ((uint) num2 < num1)
      {
        if (caesarResource == null)
        {
          try
          {
            CaesarResource resource1 = resource.Ecu.IsByteMessaging ? CaesarRoot.GetAvailableProtocolResource(resource.Ecu.DiagnosisProtocol.Name, num2++) : CaesarRoot.GetAvailableEcuResource(resource.Ecu.Name, num2++);
            ConnectionResource objB = resource.Ecu.IsByteMessaging ? new ConnectionResource(resource.Ecu.DiagnosisProtocol, resource1, resource.SourceAddress.Value, (uint) resource.BaudRate) : new ConnectionResource(resource.Ecu, resource1, resource.SourceAddress);
            if (object.Equals((object) resource, (object) objB))
              caesarResource = resource1;
          }
          catch (CaesarErrorException ex)
          {
          }
        }
        else
          break;
      }
      if (caesarResource != null)
      {
        lock (this.pendingResources)
        {
          if (resource.Ecu.IsByteMessaging)
          {
            caesarChannel = caesarResource.OpenChannel(resource.Ecu.DiagnosisProtocol.Name, resource.SourceAddress.Value);
            if (resource.BaudRate != 0)
            {
              try
              {
                caesarChannel.SetComParameter("CP_BAUDRATE", resource.BaudRate.ToString((IFormatProvider) CultureInfo.InvariantCulture));
              }
              catch (CaesarErrorException ex)
              {
                Sapi sapi = Sapi.GetSapi();
                sapi.RaiseExceptionEvent((object) sapi.Channels, (Exception) new CaesarException(ex));
              }
            }
          }
          else
            caesarChannel = caesarResource.OpenChannel(resource.Ecu.Name);
          if (addToPendingResources)
          {
            if (!this.pendingResources.Contains(resource))
              this.pendingResources.Add(resource);
          }
        }
      }
    }
    catch (CaesarErrorException ex)
    {
      byte? negativeResponseCode = new byte?();
      throw new CaesarException(ex, negativeResponseCode);
    }
    finally
    {
      CaesarRoot.UnlockResources();
    }
    if (caesarResource == null)
      throw new CaesarException(SapiError.ConnectionResourceNotAvailable);
    return caesarChannel;
  }

  private Channel Add(CaesarChannel ch, ConnectionResource cr, ChannelOptions options)
  {
    Channel channel = new Channel(ch, cr, (ChannelBaseCollection) this, options);
    lock (this.Items)
      this.Items.Add(channel);
    return channel;
  }

  private Channel Add(McdLogicalLink ll, ConnectionResource cr, ChannelOptions options)
  {
    Channel channel = new Channel(ll, cr, (ChannelBaseCollection) this, options);
    lock (this.Items)
      this.Items.Add(channel);
    return channel;
  }

  private static bool SameResourceDifferentBaudRate(
    ConnectionResource source,
    ConnectionResource test)
  {
    return string.Equals(source.Type, test.Type, StringComparison.Ordinal) && string.Equals(source.HardwareName, test.HardwareName, StringComparison.Ordinal) && source.PortIndex == test.PortIndex && !source.BaudRate.Equals(test.BaudRate);
  }

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("ConnectOffline(string, ParameterFileFormat, ArrayList) is deprecated, please use ConnectOffline(string, ParameterFileFormat, Collection<string>) instead.")]
  public Channel ConnectOffline(
    string parameterFileName,
    ParameterFileFormat parameterFileFormat,
    ArrayList unknownList)
  {
    using (StreamReader parameterFile = new StreamReader(parameterFileName))
      return this.ConnectOffline(parameterFile, parameterFileFormat, unknownList);
  }

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("ConnectOffline(StreamReader, ParameterFileFormat, ArrayList) is deprecated, please use ConnectOffline(StreamReader, ParameterFileFormat, Collection<string>) instead.")]
  public Channel ConnectOffline(
    StreamReader parameterFile,
    ParameterFileFormat parameterFileFormat,
    ArrayList unknownList)
  {
    Collection<string> unknownList1 = new Collection<string>();
    Channel channel = this.ConnectOffline(parameterFile, parameterFileFormat, unknownList1);
    foreach (string str in unknownList1)
      unknownList.Add((object) str);
    return channel;
  }
}
