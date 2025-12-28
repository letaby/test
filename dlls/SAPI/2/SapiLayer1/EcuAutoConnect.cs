using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace SapiLayer1;

internal class EcuAutoConnect
{
	private Thread thread;

	private List<Ecu> ecus;

	private ChannelCollection parent;

	private int numberOfAttemptCycles;

	private volatile bool notifyPrioritizeEthernet;

	internal string Identifier
	{
		get
		{
			string result = null;
			if (ecus.Count > 0)
			{
				result = ecus[0].Identifier;
			}
			return result;
		}
	}

	internal Thread BackgroundThread => thread;

	internal EcuAutoConnect(ChannelCollection parent, IEnumerable<Ecu> ecus)
	{
		this.parent = parent;
		this.ecus = new List<Ecu>(ecus);
		if (this.ecus.Any((Ecu e) => !e.ViaEcus.Any() && e.Interfaces.Any((EcuInterface i) => i.IsEthernet)))
		{
			RollCallDoIP.GlobalInstance.StateChangedEvent += GlobalInstance_StateChangedEvent;
		}
	}

	private void GlobalInstance_StateChangedEvent(object sender, StateChangedEventArgs e)
	{
		if (e.State == ConnectionState.ChannelsConnected)
		{
			notifyPrioritizeEthernet = true;
		}
	}

	internal void Start(int numberOfAttemptCycles)
	{
		this.numberOfAttemptCycles = numberOfAttemptCycles;
		thread = new Thread(ThreadFunc);
		thread.Name = GetType().Name + ": " + string.Join("-", ecus.Select((Ecu e) => e.Name));
		thread.Start();
	}

	private static bool AllowedToAutoConnect(Ecu tryEcu, out bool rollCallStatusChecked, out Channel extensionViaEcuChannel, out byte? sourceAddress)
	{
		bool flag = false;
		rollCallStatusChecked = false;
		extensionViaEcuChannel = null;
		sourceAddress = null;
		Channel allowedByViaEcuChannel = null;
		Sapi sapi = Sapi.GetSapi();
		if (tryEcu.MarkedForAutoConnect && sapi.Ecus.GetConnectedCountForIdentifier(tryEcu.Identifier) == 0 && sapi.Channels.GetConnectingCountForIdentifier(tryEcu.Identifier) == 0)
		{
			if (sapi.Channels.Any((Channel c) => c.CommunicationsState == CommunicationsState.ReadParameters || c.CommunicationsState == CommunicationsState.WriteParameters || c.CommunicationsState == CommunicationsState.ExecuteService || c.CommunicationsState == CommunicationsState.Flash || c.CommunicationsState == CommunicationsState.ByteMessage || c.CommunicationsState == CommunicationsState.Disconnecting))
			{
				return false;
			}
			if (tryEcu.ViaEcus.Count > 0)
			{
				bool validatedByExtension;
				Channel currentViaEcuChannel = tryEcu.GetCurrentViaEcuChannel(out validatedByExtension);
				if (currentViaEcuChannel != null)
				{
					flag = true;
					allowedByViaEcuChannel = currentViaEcuChannel;
					if (validatedByExtension)
					{
						extensionViaEcuChannel = currentViaEcuChannel;
					}
				}
			}
			else
			{
				flag = true;
			}
		}
		if (flag && tryEcu.HasRelatedAddressesWhenViaChannel(allowedByViaEcuChannel) && new RollCall[3]
		{
			RollCallDoIP.GlobalInstance,
			RollCallJ1939.GlobalInstance,
			RollCallJ1708.GlobalInstance
		}.Any((RollCall rc) => rc.ConnectEnabled))
		{
			flag = false;
			foreach (RollCall manager in new RollCall[3]
			{
				RollCallDoIP.GlobalInstance,
				RollCallJ1939.GlobalInstance,
				RollCallJ1708.GlobalInstance
			}.Where((RollCall rc) => rc.ConnectEnabled && tryEcu.IsRelated(rc.Protocol)))
			{
				IEnumerable<int> source = from ac in manager.GetActiveAddresses()
					where tryEcu.IsRelated(manager, ac, (allowedByViaEcuChannel != null) ? allowedByViaEcuChannel.Ecu : null)
					select ac;
				if (!source.Any())
				{
					continue;
				}
				flag = true;
				rollCallStatusChecked = true;
				if (manager == RollCallJ1939.GlobalInstance && tryEcu.ProtocolName == "E1939")
				{
					sourceAddress = (from addr in source
						select (byte)addr into addr
						orderby addr
						select addr).FirstOrDefault((byte addr) => !manager.IsVirtual(addr));
				}
				break;
			}
		}
		return flag;
	}

	private static bool AllAttemptsUsed(Dictionary<ConnectionResource, int> resourceAttempts, int maxAttempts)
	{
		if (resourceAttempts.Count > 0)
		{
			bool result = true;
			foreach (KeyValuePair<ConnectionResource, int> resourceAttempt in resourceAttempts)
			{
				if (resourceAttempt.Value < maxAttempts)
				{
					result = false;
					break;
				}
			}
			return result;
		}
		return false;
	}

	private void ThreadFunc()
	{
		bool flag = false;
		bool flag2 = numberOfAttemptCycles != -1;
		Channel channel = null;
		Sapi sapi = Sapi.GetSapi();
		Dictionary<ConnectionResource, int> dictionary = new Dictionary<ConnectionResource, int>();
		try
		{
			while (parent.AutoConnecting && !flag)
			{
				Thread.Sleep(1000);
				if (notifyPrioritizeEthernet)
				{
					ecus = ecus.OrderByDescending((Ecu ecu2) => ecu2.Interfaces.Any((EcuInterface i) => i.IsEthernet)).ToList();
					notifyPrioritizeEthernet = false;
				}
				for (int num = 0; num < ecus.Count; num++)
				{
					if (!parent.AutoConnecting)
					{
						break;
					}
					if (notifyPrioritizeEthernet)
					{
						break;
					}
					Ecu ecu = ecus[num];
					bool rollCallStatusChecked = false;
					Channel extensionViaEcuChannel = null;
					byte? sourceAddress = null;
					if (!AllowedToAutoConnect(ecu, out rollCallStatusChecked, out extensionViaEcuChannel, out sourceAddress))
					{
						continue;
					}
					Channel channel2 = null;
					channel = extensionViaEcuChannel;
					ConnectionResourceCollection connectionResources = ecu.GetConnectionResources(sourceAddress);
					for (int num2 = 0; num2 < connectionResources.Count; num2++)
					{
						if (!parent.AutoConnecting)
						{
							break;
						}
						if (channel2 != null)
						{
							break;
						}
						if (notifyPrioritizeEthernet)
						{
							break;
						}
						ConnectionResource cr = connectionResources[num2];
						ConnectionResource connectionResource = dictionary.Keys.FirstOrDefault((ConnectionResource ra) => ra.IsEquivalent(cr) && ra.Ecu.RequestId == ecu.RequestId);
						if (cr.Restricted)
						{
							continue;
						}
						bool flag3 = true;
						int num3 = 0;
						if ((!rollCallStatusChecked || extensionViaEcuChannel != null) && flag2 && connectionResource != null)
						{
							num3 = dictionary[connectionResource];
							if (num3 >= numberOfAttemptCycles)
							{
								flag3 = false;
							}
						}
						if (!flag3)
						{
							continue;
						}
						try
						{
							channel2 = parent.Connect(cr, ChannelOptions.All, synchronous: true, ConnectSource.Automatic);
							num3 = 0;
						}
						catch (CaesarException ex)
						{
							sapi.RaiseDebugInfoEvent(ecu, string.Format(CultureInfo.InvariantCulture, "Failed to connect {0} - {1}", cr.ToString(), ex.Message));
							switch (ex.ErrorNumber)
							{
							default:
								num3++;
								break;
							case 6614L:
								num3++;
								break;
							case 6624L:
							case 6632L:
							case 52912L:
								flag = true;
								break;
							case 6615L:
								break;
							}
						}
						if ((!rollCallStatusChecked || extensionViaEcuChannel != null) && flag2)
						{
							dictionary[connectionResource ?? cr] = num3;
						}
					}
					if (channel2 != null && channel2.Ecu != ecu)
					{
						ecus.Remove(channel2.Ecu);
						ecus.Insert(0, channel2.Ecu);
					}
				}
				if (!flag2 || !AllAttemptsUsed(dictionary, numberOfAttemptCycles))
				{
					continue;
				}
				if (channel != null)
				{
					if (!channel.Online)
					{
						channel = null;
						dictionary.Clear();
					}
				}
				else
				{
					flag = true;
				}
			}
		}
		catch (Exception e)
		{
			sapi.RaiseExceptionEvent(this, e);
		}
		if (flag)
		{
			sapi.Ecus.SetMarkedForAutoConnect(Identifier, marked: false);
		}
	}
}
