// Decompiled with JetBrains decompiler
// Type: SapiLayer1.EcuAutoConnect
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

#nullable disable
namespace SapiLayer1;

internal class EcuAutoConnect
{
  private Thread thread;
  private List<Ecu> ecus;
  private ChannelCollection parent;
  private int numberOfAttemptCycles;
  private volatile bool notifyPrioritizeEthernet;

  internal EcuAutoConnect(ChannelCollection parent, IEnumerable<Ecu> ecus)
  {
    this.parent = parent;
    this.ecus = new List<Ecu>(ecus);
    if (!this.ecus.Any<Ecu>((Func<Ecu, bool>) (e => !e.ViaEcus.Any<Ecu>() && e.Interfaces.Any<EcuInterface>((Func<EcuInterface, bool>) (i => i.IsEthernet)))))
      return;
    RollCallDoIP.GlobalInstance.StateChangedEvent += new EventHandler<StateChangedEventArgs>(this.GlobalInstance_StateChangedEvent);
  }

  private void GlobalInstance_StateChangedEvent(object sender, StateChangedEventArgs e)
  {
    if (e.State != ConnectionState.ChannelsConnected)
      return;
    this.notifyPrioritizeEthernet = true;
  }

  internal string Identifier
  {
    get
    {
      string identifier = (string) null;
      if (this.ecus.Count > 0)
        identifier = this.ecus[0].Identifier;
      return identifier;
    }
  }

  internal void Start(int numberOfAttemptCycles)
  {
    this.numberOfAttemptCycles = numberOfAttemptCycles;
    this.thread = new Thread(new ThreadStart(this.ThreadFunc));
    this.thread.Name = $"{this.GetType().Name}: {string.Join("-", this.ecus.Select<Ecu, string>((Func<Ecu, string>) (e => e.Name)))}";
    this.thread.Start();
  }

  internal Thread BackgroundThread => this.thread;

  private static bool AllowedToAutoConnect(
    Ecu tryEcu,
    out bool rollCallStatusChecked,
    out Channel extensionViaEcuChannel,
    out byte? sourceAddress)
  {
    bool autoConnect = false;
    rollCallStatusChecked = false;
    extensionViaEcuChannel = (Channel) null;
    sourceAddress = new byte?();
    Channel allowedByViaEcuChannel = (Channel) null;
    Sapi sapi = Sapi.GetSapi();
    if (tryEcu.MarkedForAutoConnect && sapi.Ecus.GetConnectedCountForIdentifier(tryEcu.Identifier) == 0 && sapi.Channels.GetConnectingCountForIdentifier(tryEcu.Identifier) == 0)
    {
      if (sapi.Channels.Any<Channel>((Func<Channel, bool>) (c => c.CommunicationsState == CommunicationsState.ReadParameters || c.CommunicationsState == CommunicationsState.WriteParameters || c.CommunicationsState == CommunicationsState.ExecuteService || c.CommunicationsState == CommunicationsState.Flash || c.CommunicationsState == CommunicationsState.ByteMessage || c.CommunicationsState == CommunicationsState.Disconnecting)))
        return false;
      if (tryEcu.ViaEcus.Count > 0)
      {
        bool validatedByExtension;
        Channel currentViaEcuChannel = tryEcu.GetCurrentViaEcuChannel(out validatedByExtension);
        if (currentViaEcuChannel != null)
        {
          autoConnect = true;
          allowedByViaEcuChannel = currentViaEcuChannel;
          if (validatedByExtension)
            extensionViaEcuChannel = currentViaEcuChannel;
        }
      }
      else
        autoConnect = true;
    }
    if (autoConnect && tryEcu.HasRelatedAddressesWhenViaChannel(allowedByViaEcuChannel))
    {
      if (((IEnumerable<RollCall>) new RollCall[3]
      {
        (RollCall) RollCallDoIP.GlobalInstance,
        (RollCall) RollCallJ1939.GlobalInstance,
        (RollCall) RollCallJ1708.GlobalInstance
      }).Any<RollCall>((Func<RollCall, bool>) (rc => rc.ConnectEnabled)))
      {
        autoConnect = false;
        foreach (RollCall rollCall in ((IEnumerable<RollCall>) new RollCall[3]
        {
          (RollCall) RollCallDoIP.GlobalInstance,
          (RollCall) RollCallJ1939.GlobalInstance,
          (RollCall) RollCallJ1708.GlobalInstance
        }).Where<RollCall>((Func<RollCall, bool>) (rc => rc.ConnectEnabled && tryEcu.IsRelated(rc.Protocol))))
        {
          RollCall manager = rollCall;
          IEnumerable<int> source = manager.GetActiveAddresses().Where<int>((Func<int, bool>) (ac => tryEcu.IsRelated(manager, ac, allowedByViaEcuChannel != null ? allowedByViaEcuChannel.Ecu : (Ecu) null)));
          if (source.Any<int>())
          {
            autoConnect = true;
            rollCallStatusChecked = true;
            if (manager == RollCallJ1939.GlobalInstance)
            {
              if (tryEcu.ProtocolName == "E1939")
              {
                sourceAddress = new byte?(source.Select<int, byte>((Func<int, byte>) (addr => (byte) addr)).OrderBy<byte, byte>((Func<byte, byte>) (addr => addr)).FirstOrDefault<byte>((Func<byte, bool>) (addr => !manager.IsVirtual((int) addr))));
                break;
              }
              break;
            }
            break;
          }
        }
      }
    }
    return autoConnect;
  }

  private static bool AllAttemptsUsed(
    Dictionary<ConnectionResource, int> resourceAttempts,
    int maxAttempts)
  {
    if (resourceAttempts.Count <= 0)
      return false;
    bool flag = true;
    foreach (KeyValuePair<ConnectionResource, int> resourceAttempt in resourceAttempts)
    {
      if (resourceAttempt.Value < maxAttempts)
      {
        flag = false;
        break;
      }
    }
    return flag;
  }

  private void ThreadFunc()
  {
    bool flag1 = false;
    bool flag2 = this.numberOfAttemptCycles != -1;
    Channel channel1 = (Channel) null;
    Sapi sapi = Sapi.GetSapi();
    Dictionary<ConnectionResource, int> resourceAttempts = new Dictionary<ConnectionResource, int>();
    try
    {
      while (this.parent.AutoConnecting)
      {
        if (!flag1)
        {
          Thread.Sleep(1000);
          if (this.notifyPrioritizeEthernet)
          {
            this.ecus = this.ecus.OrderByDescending<Ecu, bool>((Func<Ecu, bool>) (e => e.Interfaces.Any<EcuInterface>((Func<EcuInterface, bool>) (i => i.IsEthernet)))).ToList<Ecu>();
            this.notifyPrioritizeEthernet = false;
          }
          for (int index1 = 0; index1 < this.ecus.Count && this.parent.AutoConnecting && !this.notifyPrioritizeEthernet; ++index1)
          {
            Ecu ecu = this.ecus[index1];
            bool rollCallStatusChecked = false;
            Channel extensionViaEcuChannel = (Channel) null;
            byte? sourceAddress = new byte?();
            if (EcuAutoConnect.AllowedToAutoConnect(ecu, out rollCallStatusChecked, out extensionViaEcuChannel, out sourceAddress))
            {
              Channel channel2 = (Channel) null;
              channel1 = extensionViaEcuChannel;
              ConnectionResourceCollection connectionResources = ecu.GetConnectionResources(sourceAddress);
              for (int index2 = 0; index2 < connectionResources.Count && this.parent.AutoConnecting && channel2 == null && !this.notifyPrioritizeEthernet; ++index2)
              {
                ConnectionResource cr = connectionResources[index2];
                ConnectionResource key = resourceAttempts.Keys.FirstOrDefault<ConnectionResource>((Func<ConnectionResource, bool>) (ra =>
                {
                  if (!ra.IsEquivalent(cr))
                    return false;
                  uint? requestId1 = ra.Ecu.RequestId;
                  uint? requestId2 = ecu.RequestId;
                  return (int) requestId1.GetValueOrDefault() == (int) requestId2.GetValueOrDefault() && requestId1.HasValue == requestId2.HasValue;
                }));
                if (!cr.Restricted)
                {
                  bool flag3 = true;
                  int num = 0;
                  if (((!rollCallStatusChecked ? 1 : (extensionViaEcuChannel != null ? 1 : 0)) & (flag2 ? 1 : 0)) != 0 && key != null)
                  {
                    num = resourceAttempts[key];
                    if (num >= this.numberOfAttemptCycles)
                      flag3 = false;
                  }
                  if (flag3)
                  {
                    try
                    {
                      channel2 = this.parent.Connect(cr, ChannelOptions.All, true, ConnectSource.Automatic);
                      num = 0;
                    }
                    catch (CaesarException ex)
                    {
                      sapi.RaiseDebugInfoEvent((object) ecu, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to connect {0} - {1}", (object) cr.ToString(), (object) ex.Message));
                      switch (ex.ErrorNumber)
                      {
                        case 6614:
                          ++num;
                          break;
                        case 6615:
                          break;
                        case 6624:
                        case 6632:
                        case 52912:
                          flag1 = true;
                          break;
                        default:
                          ++num;
                          break;
                      }
                    }
                    if (((!rollCallStatusChecked ? 1 : (extensionViaEcuChannel != null ? 1 : 0)) & (flag2 ? 1 : 0)) != 0)
                      resourceAttempts[key ?? cr] = num;
                  }
                }
              }
              if (channel2 != null && channel2.Ecu != ecu)
              {
                this.ecus.Remove(channel2.Ecu);
                this.ecus.Insert(0, channel2.Ecu);
              }
            }
          }
          if (flag2 && EcuAutoConnect.AllAttemptsUsed(resourceAttempts, this.numberOfAttemptCycles))
          {
            if (channel1 != null)
            {
              if (!channel1.Online)
              {
                channel1 = (Channel) null;
                resourceAttempts.Clear();
              }
            }
            else
              flag1 = true;
          }
        }
        else
          break;
      }
    }
    catch (Exception ex)
    {
      sapi.RaiseExceptionEvent((object) this, ex);
    }
    if (!flag1)
      return;
    sapi.Ecus.SetMarkedForAutoConnect(this.Identifier, false);
  }
}
