using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security;
using System.ServiceProcess;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Common.Status;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using DetroitDiesel.Windows.Forms.Themed;
using Microsoft.Win32;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal class EcuStatusView : UserControl, ISupportEdit
{
	private IContainer components;

	private MenuItem menuItemConnect;

	private MenuItem menuItemClearConnectionErrors;

	private MenuItem menuItemCloseAllConnections;

	private MenuItem menuItemSeparator2;

	private TableLayoutPanel tableLayoutPanel;

	private ContextMenu connectionContextMenu;

	private Label headerLabel;

	private Panel headerPanel;

	private Panel bodyPanel;

	private EtchedLine separatorLine;

	private ChannelBaseCollection activeChannels;

	private MenuItem menuItemRefreshAutoConnect;

	private Dictionary<string, Tuple<ConnectionResource, ChannelOptions>> lastFailedResource = new Dictionary<string, Tuple<ConnectionResource, ChannelOptions>>();

	private string selectedIdentifier;

	private List<Channel> pendingCompatibilityCheck = new List<Channel>();

	private readonly bool isDesignMode;

	private static bool? isFirewallConfigurationIncorrect;

	private List<Ecu> certManagerUnavailableEcus = new List<Ecu>();

	private List<Ecu> certificatesUnavailableEcus = new List<Ecu>();

	private static bool IsFirewallConfigurationIncorrect
	{
		get
		{
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Expected O, but got Unknown
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Expected O, but got Unknown
			if (!isFirewallConfigurationIncorrect.HasValue)
			{
				bool flag = false;
				bool flag2 = false;
				try
				{
					RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\WindowsFirewall\\PublicProfile");
					if (registryKey != null)
					{
						object value = registryKey.GetValue("AllowLocalPolicyMerge");
						if (value != null && (int)value == 0)
						{
							flag = true;
						}
					}
					if (flag)
					{
						RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\WindowsFirewall\\FirewallRules");
						if (registryKey2 != null)
						{
							string[] valueNames = registryKey2.GetValueNames();
							foreach (string name in valueNames)
							{
								object value2 = registryKey2.GetValue(name);
								if (value2 != null && value2 is string text && text.IndexOf("Action=Allow", StringComparison.OrdinalIgnoreCase) != -1 && (text.IndexOf("LPort=13400", StringComparison.Ordinal) != -1 || text.IndexOf("LPort2_10=13400", StringComparison.Ordinal) != -1))
								{
									flag2 = true;
									break;
								}
							}
						}
					}
				}
				catch (SecurityException ex)
				{
					StatusLog.Add(new StatusMessage("SecurityException in IsFirewallConfigurationIncorrect:" + ex.Message, (StatusMessageType)1, (object)typeof(EcuStatusView)));
				}
				catch (UnauthorizedAccessException ex2)
				{
					StatusLog.Add(new StatusMessage("UnauthorizedAccesException in IsFirewallConfigurationIncorrect:" + ex2.Message, (StatusMessageType)1, (object)typeof(EcuStatusView)));
				}
				isFirewallConfigurationIncorrect = flag && !flag2;
			}
			return isFirewallConfigurationIncorrect.Value;
		}
	}

	public bool CanUndo => false;

	public bool CanCopy => true;

	public bool CanDelete => false;

	public bool CanPaste => false;

	public bool CanSelectAll => false;

	public bool CanCut => false;

	public event EventHandler ItemActivate;

	protected override void Dispose(bool disposing)
	{
		bodyPanel.MouseUp -= control_MouseUp;
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Container.EcuStatusView));
		this.menuItemConnect = new System.Windows.Forms.MenuItem();
		this.menuItemRefreshAutoConnect = new System.Windows.Forms.MenuItem();
		this.menuItemCloseAllConnections = new System.Windows.Forms.MenuItem();
		this.menuItemSeparator2 = new System.Windows.Forms.MenuItem();
		this.menuItemClearConnectionErrors = new System.Windows.Forms.MenuItem();
		this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
		this.connectionContextMenu = new System.Windows.Forms.ContextMenu();
		this.headerLabel = new System.Windows.Forms.Label();
		this.headerPanel = new System.Windows.Forms.Panel();
		this.bodyPanel = new System.Windows.Forms.Panel();
		this.separatorLine = new EtchedLine();
		System.Windows.Forms.MenuItem menuItem = new System.Windows.Forms.MenuItem();
		this.tableLayoutPanel.SuspendLayout();
		this.headerPanel.SuspendLayout();
		this.bodyPanel.SuspendLayout();
		base.SuspendLayout();
		menuItem.Index = 1;
		resources.ApplyResources(menuItem, "menuItemSeparator");
		this.menuItemConnect.Index = 0;
		resources.ApplyResources(this.menuItemConnect, "menuItemConnect");
		this.menuItemConnect.Click += new System.EventHandler(OnConnectClick);
		this.menuItemRefreshAutoConnect.Index = 2;
		resources.ApplyResources(this.menuItemRefreshAutoConnect, "menuItemRefreshAutoConnect");
		this.menuItemRefreshAutoConnect.Click += new System.EventHandler(OnRefreshAutoConnectClick);
		this.menuItemCloseAllConnections.Index = 3;
		resources.ApplyResources(this.menuItemCloseAllConnections, "menuItemCloseAllConnections");
		this.menuItemCloseAllConnections.Click += new System.EventHandler(OnCloseAllConnectionsClick);
		this.menuItemSeparator2.Index = 4;
		resources.ApplyResources(this.menuItemSeparator2, "menuItemSeparator2");
		this.menuItemClearConnectionErrors.Index = 5;
		resources.ApplyResources(this.menuItemClearConnectionErrors, "menuItemClearConnectionErrors");
		this.menuItemClearConnectionErrors.Click += new System.EventHandler(OnClearConnectionErrorsClick);
		resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
		this.tableLayoutPanel.BackColor = System.Drawing.SystemColors.Window;
		this.tableLayoutPanel.Controls.Add((System.Windows.Forms.Control)(object)this.separatorLine, 0, 0);
		this.tableLayoutPanel.Name = "tableLayoutPanel";
		this.connectionContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[6] { this.menuItemConnect, menuItem, this.menuItemRefreshAutoConnect, this.menuItemCloseAllConnections, this.menuItemSeparator2, this.menuItemClearConnectionErrors });
		this.connectionContextMenu.Popup += new System.EventHandler(ConnectionMenuPopup);
		resources.ApplyResources(this.headerLabel, "headerLabel");
		this.headerLabel.BackColor = System.Drawing.Color.Transparent;
		this.headerLabel.Name = "headerLabel";
		resources.ApplyResources(this.headerPanel, "headerPanel");
		this.headerPanel.BackColor = System.Drawing.Color.Transparent;
		this.headerPanel.Controls.Add(this.headerLabel);
		this.headerPanel.Name = "headerPanel";
		resources.ApplyResources(this.bodyPanel, "bodyPanel");
		this.bodyPanel.BackColor = System.Drawing.SystemColors.Window;
		this.bodyPanel.Controls.Add(this.tableLayoutPanel);
		this.bodyPanel.Name = "bodyPanel";
		this.bodyPanel.TabStop = true;
		resources.ApplyResources(this.separatorLine, "separatorLine");
		((System.Windows.Forms.Control)(object)this.separatorLine).Name = "separatorLine";
		this.BackColor = System.Drawing.SystemColors.Window;
		base.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		base.Controls.Add(this.bodyPanel);
		base.Controls.Add(this.headerPanel);
		this.Cursor = System.Windows.Forms.Cursors.Arrow;
		resources.ApplyResources(this, "$this");
		base.Name = "EcuStatusView";
		this.tableLayoutPanel.ResumeLayout(false);
		this.headerPanel.ResumeLayout(false);
		this.headerPanel.PerformLayout();
		this.bodyPanel.ResumeLayout(false);
		this.bodyPanel.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	public EcuStatusView()
	{
		isDesignMode = LicenseManager.UsageMode == LicenseUsageMode.Designtime;
		SapiManager.GlobalInstance.ActiveChannelsChanged += GlobalInstanceActiveChannelsChanged;
		InitializeComponent();
		ClearTableLayout();
		bodyPanel.MouseUp += control_MouseUp;
		ForeColor = SystemColors.WindowText;
		BackColor = SystemColors.Window;
	}

	private EcuStatusItem FindItemByTag(string identifierSought)
	{
		return (from item in tableLayoutPanel.Controls.OfType<EcuStatusItem>()
			where item.Identifier == identifierSought
			select item).FirstOrDefault();
	}

	private void OnConnectUpdateEvent(object sender, ProgressEventArgs e)
	{
		if (!base.IsDisposed)
		{
			ConnectionResource val = (ConnectionResource)((sender is ConnectionResource) ? sender : null);
			if (val != null && SapiManager.GlobalInstance.Sapi.Ecus.GetConnectedCountForIdentifier(val.Identifier) == 0 && !(activeChannels is LogFileChannelCollection))
			{
				string statusText = string.Format(CultureInfo.CurrentCulture, Resources.EcuStatus_Format_Connecting, val.Ecu.Name, e.PercentComplete.ToString(CultureInfo.CurrentCulture), SapiExtensions.ToDisplayString(val));
				AddOrUpdateItem(statusText, ConnectionIcon.Yellow, val.Ecu, val.Identifier);
			}
		}
	}

	private void AddOrUpdateItem(string statusText, ConnectionIcon imageIndex, Ecu ecu, string identifier)
	{
		EcuStatusItem ecuStatusItem = FindItemByTag(identifier);
		if (ecuStatusItem == null)
		{
			ecuStatusItem = new EcuStatusItem(identifier);
			ecuStatusItem.Ecu = ecu;
			AddToTableLayout(ecuStatusItem);
		}
		else if (ecuStatusItem.Ecu != ecu)
		{
			ecuStatusItem.Ecu = ecu;
			SetTableLayoutRows();
		}
		ecuStatusItem.StatusText = statusText;
		ecuStatusItem.Icon = imageIndex;
		ecuStatusItem.Notification.Count = 0;
	}

	private static void CheckForFirewallConfigurationIssue()
	{
		if (!IsFirewallConfigurationIncorrect)
		{
			return;
		}
		List<Channel> source = ((IEnumerable<Channel>)SapiManager.GlobalInstance.ActiveChannels).Where((Channel c) => c.ConnectionResource != null && c.ConnectionResource.IsEthernet).ToList();
		if (source.Any())
		{
			WarningsPanel.GlobalInstance.Add("firewallconfigincorrect", MessageBoxIcon.Exclamation, (string)null, string.Format(CultureInfo.CurrentCulture, Resources.EcuStatusView_FormatFirewallConfigurationIncorrect, ApplicationInformation.ProductName, string.Join(" / ", source.Select((Channel me) => me.Ecu.Name).ToArray())), (EventHandler)null);
		}
		else
		{
			WarningsPanel.GlobalInstance.Remove("firewallconfigincorrect");
		}
	}

	private void CheckForCertificateManagementError(Ecu ecu, string message)
	{
		if (message != null)
		{
			if (message.IndexOf("localhost/0:0:0:0:0:0:0:1:61000", StringComparison.OrdinalIgnoreCase) != -1)
			{
				WarningsPanel.GlobalInstance.Add("wrongcertsystem" + ecu.Name, MessageBoxIcon.Hand, (string)null, string.Format(CultureInfo.CurrentCulture, Resources.EcuStatusView_WrongCertificateManagementServerReference, ecu.Name), (EventHandler)null);
			}
			else if (message.IndexOf("localhost/0:0:0:0:0:0:0:1:62000", StringComparison.OrdinalIgnoreCase) != -1)
			{
				ServiceController serviceController = ServiceController.GetServices().FirstOrDefault((ServiceController s) => s.ServiceName == "ZenZefiT");
				bool flag = serviceController != null;
				string format = ((flag && serviceController.Status == ServiceControllerStatus.Running) ? Resources.EcuStatusView_CertificateManagementServerNotResponding : (flag ? Resources.EcuStatusView_CertificateManagementServerNotRunning : Resources.EcuStatusView_CertificateManagementServerNotInstalled));
				if (!certManagerUnavailableEcus.Contains(ecu))
				{
					certManagerUnavailableEcus.Add(ecu);
				}
				certificatesUnavailableEcus.Clear();
				WarningsPanel.GlobalInstance.Add("certsunavailable", MessageBoxIcon.Exclamation, (string)null, string.Format(CultureInfo.CurrentCulture, format, string.Join(" / ", certManagerUnavailableEcus.Select((Ecu me) => me.Name).ToArray())), (EventHandler)null);
			}
			else if (message.IndexOf("No Certificate was found matching the filter criteria", StringComparison.OrdinalIgnoreCase) != -1)
			{
				string ecuStatusView_CertificatesNotAvailable = Resources.EcuStatusView_CertificatesNotAvailable;
				if (!certificatesUnavailableEcus.Contains(ecu))
				{
					certificatesUnavailableEcus.Add(ecu);
				}
				certManagerUnavailableEcus.Clear();
				WarningsPanel.GlobalInstance.Add("certsunavailable", MessageBoxIcon.Exclamation, (string)null, string.Format(CultureInfo.CurrentCulture, ecuStatusView_CertificatesNotAvailable, string.Join(" / ", certificatesUnavailableEcus.Select((Ecu me) => me.Name).ToArray())), (EventHandler)null);
			}
		}
		else
		{
			if (certManagerUnavailableEcus.Contains(ecu))
			{
				certManagerUnavailableEcus.Clear();
			}
			if (certificatesUnavailableEcus.Contains(ecu))
			{
				certificatesUnavailableEcus.Remove(ecu);
			}
			if (certificatesUnavailableEcus.Count == 0 && certManagerUnavailableEcus.Count == 0)
			{
				WarningsPanel.GlobalInstance.Remove("certsunavailable");
			}
		}
	}

	private void channels_ConnectCompleteEvent(object sender, ConnectCompleteEventArgs e)
	{
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		if (base.IsDisposed)
		{
			return;
		}
		Channel val = (Channel)((sender is Channel) ? sender : null);
		if (val != null)
		{
			if (!((ResultEventArgs)e).Succeeded)
			{
				return;
			}
			AddChannel(val);
			if (val.Ecu.IsMcd)
			{
				CheckForCertificateManagementError(val.Ecu, null);
				if (val.ConnectionResource != null && val.ConnectionResource.IsEthernet)
				{
					CheckForFirewallConfigurationIssue();
				}
			}
			return;
		}
		ConnectionResource connectionResource = (ConnectionResource)((sender is ConnectionResource) ? sender : null);
		if (connectionResource == null)
		{
			return;
		}
		if (connectionResource.Ecu.IsRollCall)
		{
			EcuStatusItem ecuStatusItem = FindItemByTag(connectionResource.Identifier);
			if (ecuStatusItem != null)
			{
				RemoveFromTableLayout(ecuStatusItem);
			}
			return;
		}
		Exception exception = ((ResultEventArgs)e).Exception;
		CaesarException ex = (CaesarException)(object)((exception is CaesarException) ? exception : null);
		if (ex.ErrorNumber == 6611 || activeChannels is LogFileChannelCollection)
		{
			return;
		}
		Channel val2 = ((IEnumerable<Channel>)activeChannels).FirstOrDefault((Channel c) => c.Identifier == connectionResource.Identifier && c.Online);
		if (val2 == null)
		{
			AddOrUpdateItem(string.Format(CultureInfo.CurrentCulture, e.AutoConnect ? Resources.FormatAutoConnectFailed : Resources.FormatConnectFailed, connectionResource.Ecu.Name, ((ResultEventArgs)e).Exception.Message), ConnectionIcon.Red, connectionResource.Ecu, connectionResource.Identifier);
			lastFailedResource[connectionResource.Identifier] = Tuple.Create<ConnectionResource, ChannelOptions>(connectionResource, e.ChannelOptions);
			if (connectionResource.Ecu.IsMcd)
			{
				CheckForCertificateManagementError(connectionResource.Ecu, ((Exception)(object)ex).Message);
			}
		}
		else
		{
			StatusLog.Add(new StatusMessage("Ignoring " + connectionResource.Ecu.Name + " connection failure (" + ((Exception)(object)ex).Message + "), because we're connected to " + val2.Ecu.Name, (StatusMessageType)2, (object)this));
		}
	}

	private void OnDisconnectCompleteEvent(object sender, EventArgs e)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		Channel val = (Channel)((sender is Channel) ? sender : null);
		if (GetChannelForIdentifier(val.Identifier, activeChannels) == null)
		{
			if (SapiManager.GlobalInstance.IsManualConnection(val.Identifier) && val.DisconnectionException != null && ((IEnumerable<CommunicationsStateValue>)val.CommunicationsStateValues).Any((CommunicationsStateValue csv) => (int)csv.Value == 2))
			{
				lastFailedResource[val.Identifier] = Tuple.Create<ConnectionResource, ChannelOptions>(val.ConnectionResource, val.ChannelOptions);
			}
			else
			{
				RemoveChannel(val);
			}
		}
		if (val.Ecu.IsMcd && val.ConnectionResource != null && val.ConnectionResource.IsEthernet)
		{
			CheckForFirewallConfigurationIssue();
		}
	}

	private void UpdateChannelState(Channel channel, CommunicationsStateValue current = null)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Invalid comparison between Unknown and I4
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Invalid comparison between Unknown and I4
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Invalid comparison between Unknown and I4
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Invalid comparison between Unknown and I4
		EcuStatusItem ecuStatusItem = FindItemByTag(channel.Identifier);
		if (ecuStatusItem == null)
		{
			return;
		}
		string empty = string.Empty;
		CommunicationsState val;
		if (current != null)
		{
			val = current.Value;
			empty = SapiExtensions.FormatDisplayString(current, false);
			if (((int)current.Value == 3 || (int)current.Value == 2) && channel.DisconnectionException != null)
			{
				empty = empty + " - " + ((Exception)(object)channel.DisconnectionException).Message;
			}
		}
		else if (channel.CommunicationsStateValues.Current != null)
		{
			val = channel.CommunicationsStateValues.Current.Value;
			empty = SapiExtensions.FormatDisplayString(channel.CommunicationsStateValues.Current, false);
		}
		else if (channel.LogFile != null && ((ReadOnlyCollection<CommunicationsStateValue>)(object)channel.CommunicationsStateValues).Count > 0 && channel.LogFile.CurrentTime < ((ReadOnlyCollection<CommunicationsStateValue>)(object)channel.CommunicationsStateValues)[0].Time)
		{
			val = ((ReadOnlyCollection<CommunicationsStateValue>)(object)channel.CommunicationsStateValues)[0].Value;
			empty = SapiExtensions.FormatDisplayString(((ReadOnlyCollection<CommunicationsStateValue>)(object)channel.CommunicationsStateValues)[0], false);
		}
		else
		{
			val = channel.CommunicationsState;
			empty = SapiExtensions.FormatDisplayString(channel.CommunicationsState);
		}
		if ((int)val != 1)
		{
			if (val - 2 <= 1)
			{
				ecuStatusItem.Icon = ConnectionIcon.Red;
			}
			else
			{
				ecuStatusItem.Icon = ConnectionIcon.Yellow;
			}
		}
		else
		{
			ecuStatusItem.Icon = ConnectionIcon.Green;
		}
		int num = 0;
		List<string> list = new List<string>();
		if ((int)val == 1)
		{
			list.AddRange(ActionableFaultCodes(channel.FaultCodes.Current, (FaultCodeDisplayIncludeInfo)1));
			if (!channel.IsRollCall && SapiManager.GlobalInstance.CoalesceFaultCodes)
			{
				IEnumerable<FaultCode> enumerable = channel.RelatedChannels.SelectMany((Channel rc) => rc.FaultCodes.Current);
				IEnumerable<FaultCode> second = enumerable.Where((FaultCode rcf) => rcf.Channel.RelatedChannels.SelectMany((Channel rrc) => rrc.FaultCodes.Current).Any((FaultCode rrcf) => rrcf.IsRelated(rcf)));
				IEnumerable<FaultCode> faultCodes = enumerable.Except(second);
				list.AddRange(ActionableFaultCodes(faultCodes, (FaultCodeDisplayIncludeInfo)3));
			}
		}
		num = list.Count();
		if (num > 0)
		{
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			string ecuStatus_Format_ActiveFaults = Resources.EcuStatus_Format_ActiveFaults;
			object[] obj = new object[4]
			{
				channel.Ecu.Name,
				empty,
				null,
				null
			};
			ConnectionResource activeConnectionResource = SapiExtensions.GetActiveConnectionResource(channel);
			obj[2] = ((activeConnectionResource != null) ? SapiExtensions.ToDisplayString(activeConnectionResource) : null);
			obj[3] = string.Join(Environment.NewLine, list.ToArray());
			ecuStatusItem.StatusText = string.Format(currentCulture, ecuStatus_Format_ActiveFaults, obj);
		}
		else
		{
			CultureInfo currentCulture2 = CultureInfo.CurrentCulture;
			string ecuStatus_Format_OtherState = Resources.EcuStatus_Format_OtherState;
			string name = channel.Ecu.Name;
			string arg = empty;
			ConnectionResource activeConnectionResource2 = SapiExtensions.GetActiveConnectionResource(channel);
			ecuStatusItem.StatusText = string.Format(currentCulture2, ecuStatus_Format_OtherState, name, arg, (activeConnectionResource2 != null) ? SapiExtensions.ToDisplayString(activeConnectionResource2) : null);
		}
		ecuStatusItem.Notification.Count = num;
	}

	private static List<string> ActionableFaultCodes(IEnumerable faultCodes, FaultCodeDisplayIncludeInfo format)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		List<string> list = new List<string>();
		foreach (FaultCode faultCode in faultCodes)
		{
			FaultCode val = faultCode;
			FaultCodeIncident current = val.FaultCodeIncidents.Current;
			if (current != null && SapiManager.IsFaultActionable(current))
			{
				list.Add(SapiExtensions.FormatDisplayString(current.FaultCode, format));
			}
		}
		return list;
	}

	private void OnCommunicationsStateValueUpdateEvent(object sender, CommunicationsStateValueEventArgs e)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Invalid comparison between Unknown and I4
		Channel val = (Channel)((sender is Channel) ? sender : null);
		UpdateChannelState(val, e.CommunicationsStateValue);
		if (pendingCompatibilityCheck.Contains(val) && (int)e.CommunicationsStateValue.Value != 4)
		{
			pendingCompatibilityCheck.Remove(val);
			CheckCompatibility();
		}
	}

	private void OnFaultCodeCollectionUpdateEvent(object sender, ResultEventArgs e)
	{
		FaultCodeCollection faultCodes = (FaultCodeCollection)((sender is FaultCodeCollection) ? sender : null);
		UpdateChannelState(faultCodes.Channel);
		if (!SapiManager.GlobalInstance.CoalesceFaultCodes)
		{
			return;
		}
		if (faultCodes.Channel.IsRollCall)
		{
			foreach (Channel relatedChannel in faultCodes.Channel.RelatedChannels)
			{
				UpdateChannelState(relatedChannel);
			}
			return;
		}
		foreach (Channel item in from rrc in faultCodes.Channel.RelatedChannels.SelectMany((Channel rc) => rc.RelatedChannels)
			where rrc != faultCodes.Channel
			select rrc)
		{
			UpdateChannelState(item);
		}
	}

	private void OnConnectClick(object sender, EventArgs e)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (selectedIdentifier == null)
		{
			using (ConnectionDialog connectionDialog = new ConnectionDialog())
			{
				connectionDialog.ShowDialog();
				return;
			}
		}
		Channel channelForIdentifier = GetChannelForIdentifier(selectedIdentifier, SapiManager.GlobalInstance.ActiveChannels);
		if (channelForIdentifier == null)
		{
			if (lastFailedResource.TryGetValue(selectedIdentifier, out var value))
			{
				SapiManager.GlobalInstance.OpenConnection(value.Item1, value.Item2);
			}
		}
		else
		{
			SapiManager.GlobalInstance.CloseConnection(channelForIdentifier);
		}
	}

	private void OnRefreshAutoConnectClick(object sender, EventArgs e)
	{
		SapiManager.GlobalInstance.RefreshAutoConnect();
	}

	private static Channel GetChannelForIdentifier(string identifier, ChannelBaseCollection channels)
	{
		foreach (Channel channel in channels)
		{
			if (identifier.Equals(channel.Identifier))
			{
				return channel;
			}
		}
		return null;
	}

	private static IEnumerable<Ecu> GetEcusForIdentifier(string identifier)
	{
		if (SapiManager.GlobalInstance != null && SapiManager.GlobalInstance.Sapi != null)
		{
			IEnumerable<Ecu> source = from Ecu ecu in (IEnumerable)SapiManager.GlobalInstance.Sapi.Ecus
				where ecu.Identifier == identifier
				select ecu;
			return source.ToList().AsReadOnly();
		}
		return Enumerable.Empty<Ecu>();
	}

	private void ConnectionMenuPopup(object sender, EventArgs e)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Invalid comparison between Unknown and I4
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Invalid comparison between Unknown and I4
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Invalid comparison between Unknown and I4
		string text = Resources.EcuStatusMenu_Connect;
		string text2 = Resources.EcuStatusMenu_ClearAllConnectionErrors;
		bool flag = false;
		bool enabled = false;
		bool online = SapiManager.GlobalInstance.Online;
		if (CheckOrRemoveErrorItem(remove: false))
		{
			enabled = true;
		}
		if (selectedIdentifier == null)
		{
			flag = true;
		}
		else
		{
			Channel channelForIdentifier = GetChannelForIdentifier(selectedIdentifier, SapiManager.GlobalInstance.ActiveChannels);
			if (channelForIdentifier != null)
			{
				if (channelForIdentifier.Online)
				{
					if (channelForIdentifier.IsRollCall)
					{
						flag = false;
					}
					else if ((int)channelForIdentifier.CommunicationsState == 1 || (int)channelForIdentifier.CommunicationsState == 4 || (int)channelForIdentifier.CommunicationsState == 5)
					{
						flag = online;
					}
					text = Resources.EcuStatusMenu_Disconnect;
				}
				else if (channelForIdentifier.LogFile == null)
				{
					flag = true;
					text = Resources.EcuStatusMenu_Close;
				}
			}
			else
			{
				text2 = Resources.EcuStatusMenu_ClearConnectionError;
				if (SapiManager.GlobalInstance.IsManualConnection(selectedIdentifier) || !SapiManager.GlobalInstance.IsSetForAutoConnect(selectedIdentifier))
				{
					text = Resources.EcuStatusMenu_Reconnect;
					if (lastFailedResource.ContainsKey(selectedIdentifier))
					{
						flag = online;
					}
				}
				else
				{
					text = Resources.EcuStatusMenu_RetryAutomaticConnection;
					flag = online;
				}
			}
		}
		menuItemConnect.Text = text;
		menuItemConnect.Enabled = flag && !AdapterInformation.AdapterProhibited;
		menuItemRefreshAutoConnect.Enabled = SapiManager.GlobalInstance.AutoConnectNeedsRefresh && !AdapterInformation.AdapterProhibited;
		menuItemClearConnectionErrors.Text = text2;
		menuItemClearConnectionErrors.Enabled = enabled;
		if (SapiManager.GlobalInstance.Online)
		{
			menuItemCloseAllConnections.Text = Resources.CommandCloseConnections;
			menuItemCloseAllConnections.Enabled = ((ReadOnlyCollection<Channel>)(object)SapiManager.GlobalInstance.Sapi.Channels).Count > 0;
		}
		else
		{
			menuItemCloseAllConnections.Text = Resources.CommandCloseLog;
			menuItemCloseAllConnections.Enabled = true;
		}
	}

	protected override void OnLoad(EventArgs e)
	{
		if (!isDesignMode)
		{
			CheckPendingUploadData();
			ServerDataManager.GlobalInstance.DataUpdated += GlobalInstance_DataUpdated;
			ServerClient.GlobalInstance.InUseChanged += ServerClient_InUseChanged;
		}
		SapiManager.GlobalInstance.TranslatorConnectedChanged += GlobalInstance_TranslatorConnectedChanged;
		SapiManager.GlobalInstance.CoalesceFaultCodesChanged += GlobalInstance_CoalesceFaultCodesChanged;
		UpdateActiveChannels();
		base.OnLoad(e);
	}

	private void GlobalInstance_CoalesceFaultCodesChanged(object sender, EventArgs e)
	{
		UpdateActiveChannels();
	}

	private void GlobalInstance_TranslatorConnectedChanged(object sender, EventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		IRollCall manager = ChannelCollection.GetManager((Protocol)71993);
		if (manager != null && (int)AdapterInformation.SupportsNativeIso == 1 && manager.DeviceSupportedProtocols != null && manager.DeviceSupportedProtocols.Split(",".ToCharArray()).Contains("ISO15765"))
		{
			WarningsPanel.GlobalInstance.Add("nativeisonotrecommended", MessageBoxIcon.Exclamation, string.Empty, string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_NativeISONotRecommended, manager.DeviceName), (EventHandler)delegate
			{
				Program.RunSidConfigure(checkIniFile: false);
				SapiManager.GlobalInstance.ResetSapi();
			});
		}
		else
		{
			WarningsPanel.GlobalInstance.Remove("nativeisonotrecommended");
		}
	}

	private void GlobalInstanceActiveChannelsChanged(object sender, EventArgs e)
	{
		UpdateActiveChannels();
	}

	private void UpdateActiveChannels()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Expected O, but got Unknown
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		ClearTableLayout();
		if (activeChannels != null)
		{
			activeChannels.ConnectCompleteEvent2 -= channels_ConnectCompleteEvent;
			activeChannels.DisconnectCompleteEvent -= new DisconnectCompleteEventHandler(OnDisconnectCompleteEvent);
			activeChannels.ConnectProgressEvent -= new ConnectProgressEventHandler(OnConnectUpdateEvent);
			foreach (Channel activeChannel in activeChannels)
			{
				RemoveChannel(activeChannel);
			}
			if (activeChannels is LogFileChannelCollection)
			{
				ChannelBaseCollection obj = activeChannels;
				((LogFileChannelCollection)((obj is LogFileChannelCollection) ? obj : null)).LogFile.LogFilePlaybackStateUpdateEvent -= new LogFilePlaybackStateUpdateEventHandler(LogFile_LogFilePlaybackStateUpdateEvent);
			}
		}
		activeChannels = SapiManager.GlobalInstance.ActiveChannels;
		if (activeChannels == null)
		{
			return;
		}
		activeChannels.ConnectCompleteEvent2 += channels_ConnectCompleteEvent;
		activeChannels.DisconnectCompleteEvent += new DisconnectCompleteEventHandler(OnDisconnectCompleteEvent);
		activeChannels.ConnectProgressEvent += new ConnectProgressEventHandler(OnConnectUpdateEvent);
		foreach (Channel activeChannel2 in activeChannels)
		{
			AddChannel(activeChannel2);
		}
		ChannelBaseCollection obj2 = activeChannels;
		LogFileChannelCollection val = (LogFileChannelCollection)(object)((obj2 is LogFileChannelCollection) ? obj2 : null);
		if (val != null)
		{
			val.LogFile.LogFilePlaybackStateUpdateEvent += new LogFilePlaybackStateUpdateEventHandler(LogFile_LogFilePlaybackStateUpdateEvent);
			headerLabel.Text = (val.LogFile.Playing ? Resources.EcuStatus_LoggedConnectionsHeading : Resources.EcuStatus_LoggedConnectionsPausedHeading);
		}
		else
		{
			headerLabel.Text = Resources.EcuStatus_ConnectionsHeading;
		}
	}

	private void AddChannel(Channel channel)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Invalid comparison between Unknown and I4
		channel.CommunicationsStateValueUpdateEvent += new CommunicationsStateValueUpdateEventHandler(OnCommunicationsStateValueUpdateEvent);
		channel.InitCompleteEvent += new InitCompleteEventHandler(OnInitCompleteEvent);
		channel.FaultCodes.FaultCodesUpdateEvent += new FaultCodesUpdateEventHandler(OnFaultCodeCollectionUpdateEvent);
		channel.FlashAreas.FlashProgressUpdateEvent += new FlashProgressUpdateEventHandler(FlashAreas_FlashProgressUpdateEvent);
		channel.Parameters.ParameterUpdateEvent += new ParameterUpdateEventHandler(Parameters_ParameterUpdateEvent);
		AddOrUpdateItem(string.Empty, ConnectionIcon.Yellow, channel.Ecu, channel.Identifier);
		UpdateChannelState(channel);
		if (!channel.Ecu.IsRollCall && SapiManager.GlobalInstance.CoalesceFaultCodes && channel.Ecu.RelatedEcus.Any())
		{
			SetTableLayoutRows();
			Update();
		}
		if (!channel.IsRollCall)
		{
			CommunicationsStateValue current = channel.CommunicationsStateValues.Current;
			if (current != null && (int)current.Value != 4)
			{
				CheckCompatibility();
			}
			else
			{
				pendingCompatibilityCheck.Add(channel);
			}
			CheckEthernetConnection();
		}
	}

	private static void CheckEthernetConnection()
	{
		List<Channel> source = ((IEnumerable<Channel>)SapiManager.GlobalInstance.ActiveChannels).Where((Channel c) => c.Ecu.IsMcd && c.Online && ((IEnumerable<EcuInterface>)c.Ecu.Interfaces).Any((EcuInterface i) => i.IsEthernet)).ToList();
		List<Channel> list = (from channel in source
			let resource = SapiExtensions.GetActiveConnectionResource(channel)
			where resource != null && resource.IsEthernet
			select channel).ToList();
		if (list.Count == 0 && source.Any((Channel c) => SapiExtensions.IsGateway(c.Ecu)))
		{
			WarningsPanel.GlobalInstance.Add("NOTETHERNET", MessageBoxIcon.Exclamation, (string)null, Resources.Format_RequiresEthernetWarning, (EventHandler)null);
		}
		else
		{
			WarningsPanel.GlobalInstance.Remove("NOTETHERNET");
		}
	}

	private void RemoveChannel(Channel channel)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		channel.InitCompleteEvent -= new InitCompleteEventHandler(OnInitCompleteEvent);
		channel.CommunicationsStateValueUpdateEvent -= new CommunicationsStateValueUpdateEventHandler(OnCommunicationsStateValueUpdateEvent);
		channel.FaultCodes.FaultCodesUpdateEvent -= new FaultCodesUpdateEventHandler(OnFaultCodeCollectionUpdateEvent);
		channel.FlashAreas.FlashProgressUpdateEvent -= new FlashProgressUpdateEventHandler(FlashAreas_FlashProgressUpdateEvent);
		channel.Parameters.ParameterUpdateEvent -= new ParameterUpdateEventHandler(Parameters_ParameterUpdateEvent);
		EcuStatusItem ecuStatusItem = FindItemByTag(channel.Identifier);
		if (ecuStatusItem != null)
		{
			RemoveFromTableLayout(ecuStatusItem);
		}
		if (!channel.IsRollCall)
		{
			CheckCompatibility();
			pendingCompatibilityCheck.Remove(channel);
			CheckEthernetConnection();
		}
		WarningsPanel.GlobalInstance.Remove(string.Format(CultureInfo.InvariantCulture, "BASE{0}", channel.Ecu.Name));
		WarningsPanel.GlobalInstance.Remove(string.Format(CultureInfo.InvariantCulture, "INVALIDVARIANT{0}", channel.Ecu.Name));
		WarningsPanel.GlobalInstance.Remove(string.Format(CultureInfo.InvariantCulture, "PENDINGVARIANT{0}", channel.Ecu.Name));
	}

	private void UpdateStatusWithPercentage(Channel channel, double percent)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		EcuStatusItem ecuStatusItem = FindItemByTag(channel.Identifier);
		if (ecuStatusItem != null)
		{
			CommunicationsState communicationsState = channel.CommunicationsState;
			string text = (((int)communicationsState == 5) ? Resources.EcuStatusView_Reading : (((int)communicationsState != 6) ? SapiExtensions.FormatDisplayString(channel.CommunicationsState) : Resources.EcuStatusView_Writing));
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			string ecuStatus_Format_OtherStatusWithPercentage = Resources.EcuStatus_Format_OtherStatusWithPercentage;
			object[] obj = new object[4]
			{
				channel.Ecu.Name,
				text,
				percent,
				null
			};
			ConnectionResource activeConnectionResource = SapiExtensions.GetActiveConnectionResource(channel);
			obj[3] = ((activeConnectionResource != null) ? SapiExtensions.ToDisplayString(activeConnectionResource) : null);
			ecuStatusItem.StatusText = string.Format(currentCulture, ecuStatus_Format_OtherStatusWithPercentage, obj);
		}
	}

	private void FlashAreas_FlashProgressUpdateEvent(object sender, ProgressEventArgs e)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		FlashAreaCollection val = (FlashAreaCollection)((sender is FlashAreaCollection) ? sender : null);
		if (val != null && (int)val.Channel.CommunicationsState == 7)
		{
			UpdateStatusWithPercentage(val.Channel, e.PercentComplete);
		}
	}

	private void Parameters_ParameterUpdateEvent(object sender, ResultEventArgs e)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Invalid comparison between Unknown and I4
		Parameter val = (Parameter)((sender is Parameter) ? sender : null);
		if (val != null && ((int)val.Channel.CommunicationsState == 5 || (int)val.Channel.CommunicationsState == 6))
		{
			UpdateStatusWithPercentage(val.Channel, val.Channel.Parameters.Progress);
		}
	}

	private void OnInitCompleteEvent(object sender, EventArgs e)
	{
		Channel val = (Channel)((sender is Channel) ? sender : null);
		if (val == null || val.IsRollCall)
		{
			return;
		}
		if (val.DiagnosisVariant.IsBase)
		{
			if (!SapiExtensions.IsDataSourceEdex(val.Ecu) || !SapiManager.GetBootModeStatus(val))
			{
				IEnumerable<Ecu> ecusForIdentifier = GetEcusForIdentifier(val.Identifier);
				string text = string.Join(Resources.EcuStatus_Separator, ecusForIdentifier.Select((Ecu x) => x.Name).ToArray());
				string text2 = ((ecusForIdentifier.Count() == 1) ? val.Ecu.DescriptionDataVersion : string.Empty);
				string text3 = SapiManager.GetSoftwareVersion(val) ?? string.Empty;
				if (text3.StartsWith("Security access denied", StringComparison.OrdinalIgnoreCase))
				{
					text3 = string.Empty;
				}
				string text4 = string.Format(CultureInfo.CurrentCulture, (ApplicationInformation.ProductAccessLevel == 3) ? Resources.FormatBaseVariantWarning_Engineering : Resources.FormatBaseVariantWarning, text, text3, text2, ApplicationInformation.ProductName);
				WarningsPanel.GlobalInstance.Add(string.Format(CultureInfo.InvariantCulture, "BASE{0}", val.Ecu.Name), MessageBoxIcon.Hand, string.Empty, text4, (ApplicationInformation.ProductAccessLevel == 3) ? null : new EventHandler(OnMenuUpdateClick));
			}
		}
		else if (ApplicationInformation.CheckValidVariants)
		{
			if (!SapiExtensions.GetValidDiagnosisVariants(val.Ecu).Contains(val.DiagnosisVariant) && !val.DiagnosisVariant.IsBoot)
			{
				WarningsPanel.GlobalInstance.Add(string.Format(CultureInfo.InvariantCulture, "INVALIDVARIANT{0}", val.Ecu.Name), MessageBoxIcon.Hand, (string)null, string.Format(CultureInfo.CurrentCulture, Resources.Format_InvalidVariantWarning, val.Ecu.Name, val.DiagnosisVariant.Name, ApplicationInformation.ProductName), (EventHandler)null);
			}
			else if (SapiExtensions.IsPendingDiagnosisVariant(val.DiagnosisVariant))
			{
				WarningsPanel.GlobalInstance.Add(string.Format(CultureInfo.InvariantCulture, "PENDINGVARIANT{0}", val.Ecu.Name), MessageBoxIcon.Exclamation, (string)null, string.Format(CultureInfo.CurrentCulture, Resources.Format_PendingVariantWarning, val.Ecu.Name, val.DiagnosisVariant.Name, ApplicationInformation.ProductName), (EventHandler)null);
			}
		}
	}

	public static void OnMenuUpdateClick(object sender, EventArgs e)
	{
		ServerClient.GlobalInstance.Go((Collection<UnitInformation>)null, (Collection<UnitInformation>)null);
	}

	private static void ListIncompatibilites(string message, UnitInformation unit, bool depotEcusCompatible, bool edexEcusCompatible)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Expected O, but got Unknown
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Expected O, but got Unknown
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Expected O, but got Unknown
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Expected O, but got Unknown
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Expected O, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Expected O, but got Unknown
		bool flag = false;
		bool flag2 = false;
		CompatibilityWarningDialog val = new CompatibilityWarningDialog(message);
		List<CompatibilityWarningMissingReason> list = new List<CompatibilityWarningMissingReason>();
		if (!edexEcusCompatible)
		{
			if (!ApplicationInformation.CanReprogramEdexUnits)
			{
				list.Add(new CompatibilityWarningMissingReason(Resources.EcuStatusView_NoEdexReprogramming, (DeviceDataSource)2));
			}
			else if (unit != null)
			{
				bool flag3 = false;
				foreach (EdexCompatibilityEcuItem ecu in EdexCompatibilityTable.GetCurrentConnectedConfiguration().Ecus)
				{
					EdexCompatibilityConfigurationCollection val2 = ServerDataManager.GlobalInstance.EdexCompatibilityTable.CreateCompatibleList(ecu);
					if (!((IEnumerable<EdexCompatibilityConfiguration>)val2).Any())
					{
						list.Add(new CompatibilityWarningMissingReason(Resources.EcuStatusView_TargetNotInCompat, ecu));
						continue;
					}
					EdexCompatibilityConfigurationCollection val3 = val2.FilterForOnlineDevices(ecu);
					if (((IEnumerable<EdexCompatibilityConfiguration>)val3).Any())
					{
						EdexCompatibilityConfigurationCollection val4 = val3.FilterForUnit(unit);
						if (!((IEnumerable<EdexCompatibilityConfiguration>)val4).Any())
						{
							flag3 = true;
						}
						else if (((IEnumerable<EdexCompatibilityConfiguration>)val4).Any((EdexCompatibilityConfiguration cs) => cs.Ecus.Any()))
						{
							val.AddCompatibilityInfo(ecu, Resources.EcuStatusView_Compat_TargetDevice, val4, Resources.EcuStatusView_Compat_CompatibleOptions);
							flag = true;
						}
					}
				}
				if (flag)
				{
					list.RemoveAll((CompatibilityWarningMissingReason x) => (int)x.DataSource == 2);
				}
				else if (flag3)
				{
					IEnumerable<EdexCompatibilityEcuItem> enumerable = default(IEnumerable<EdexCompatibilityEcuItem>);
					EdexCompatibilityConfigurationCollection source = ServerDataManager.GlobalInstance.EdexCompatibilityTable.CreateCompatibleList(unit, ref enumerable);
					if (!((IEnumerable<EdexCompatibilityConfiguration>)source).Any())
					{
						foreach (EdexCompatibilityEcuItem item in enumerable)
						{
							list.Add(new CompatibilityWarningMissingReason(Resources.EcuStatusView_UnitDataSoftwareNotInCompat, item));
						}
					}
				}
			}
			else
			{
				list.Add(new CompatibilityWarningMissingReason(Resources.EcuStatusView_NoUnitData, (DeviceDataSource)2));
			}
		}
		if (!depotEcusCompatible)
		{
			if (!ApplicationInformation.CanReprogramDepotUnits)
			{
				list.Add(new CompatibilityWarningMissingReason(Resources.EcuStatusView_NoDepotReprogramming, (DeviceDataSource)1));
			}
			else if (unit != null)
			{
				foreach (Software item2 in (ReadOnlyCollection<Software>)(object)ServerDataManager.GlobalInstance.CompatibilityTable.CurrentSoftware)
				{
					CompatibleSoftwareCollection val5 = ServerDataManager.GlobalInstance.CompatibilityTable.CreateCompatibleList(item2);
					if (!((IEnumerable<SoftwareCollection>)val5).Any())
					{
						list.Add(new CompatibilityWarningMissingReason(Resources.EcuStatusView_TargetNotInCompat, item2));
						continue;
					}
					CompatibleSoftwareCollection val6 = val5.FilterForUnit(unit);
					if (!((IEnumerable<SoftwareCollection>)val6).Any())
					{
						list.Add(new CompatibilityWarningMissingReason(Resources.EcuStatusView_UnitDataSoftwareSetNotInCompat, item2));
						continue;
					}
					CompatibleSoftwareCollection val7 = val6.FilterForOnlineOptions(item2.Ecu);
					if (!((IEnumerable<SoftwareCollection>)val7).Any())
					{
						list.Add(new CompatibilityWarningMissingReason(Resources.EcuStatusView_ConnectedDevicesCompatible, item2));
						continue;
					}
					val.AddCompatibilityInfo(item2, Resources.EcuStatusView_Compat_TargetDevice, val7, Resources.EcuStatusView_Compat_CompatibleOptions);
					flag2 = true;
				}
				if (flag2)
				{
					list.RemoveAll((CompatibilityWarningMissingReason x) => (int)x.DataSource == 1);
				}
			}
			else
			{
				list.Add(new CompatibilityWarningMissingReason(Resources.EcuStatusView_NoUnitData, (DeviceDataSource)1));
			}
		}
		val.AddNoCompatibilityInfoAvailableReasons((IEnumerable<CompatibilityWarningMissingReason>)list);
		((Form)(object)val).ShowDialog();
	}

	private void GlobalInstance_DataUpdated(object sender, EventArgs e)
	{
		CheckCompatibility();
		if (!isDesignMode)
		{
			CheckPendingUploadData();
		}
	}

	private void ServerClient_InUseChanged(object sender, EventArgs e)
	{
		if (!isDesignMode)
		{
			CheckPendingUploadData();
		}
	}

	private static void CheckCompatibility()
	{
		if (WarningsPanel.GlobalInstance == null)
		{
			return;
		}
		if (ServerDataManager.GlobalInstance.CompatibilityTable != null && ServerDataManager.GlobalInstance.EdexCompatibilityTable != null)
		{
			WarningsPanel.GlobalInstance.Remove("compatibilitytable");
			bool flag = false;
			bool flag2 = false;
			UnitInformation connectedUnit = ServerDataManager.GlobalInstance.ConnectedUnit;
			if (connectedUnit != null)
			{
				flag = connectedUnit.InAutomaticOperation;
				flag2 = connectedUnit.UnitFixedAtTest;
			}
			bool edexEcusCompatible = true;
			bool depotEcusCompatible = true;
			bool flag3 = true;
			bool flag4 = true;
			if (!flag)
			{
				flag3 = ServerDataManager.GlobalInstance.CompatibilityTable.IsCurrentHardwareCompatibleWithCurrentSoftware();
				flag4 = flag2 || ServerDataManager.GlobalInstance.CompatibilityTable.IsCurrentSoftwareCompatible();
				depotEcusCompatible = flag3 && flag4;
				edexEcusCompatible = ServerDataManager.GlobalInstance.EdexCompatibilityTable.IsCurrentSoftwareCompatible();
				if (!(depotEcusCompatible && edexEcusCompatible))
				{
					string empty = string.Empty;
					EventHandler eventHandler = null;
					string baseMessage;
					empty = (baseMessage = ((!depotEcusCompatible && !edexEcusCompatible) ? ((ApplicationInformation.CanReprogramDepotUnits && ApplicationInformation.CanReprogramEdexUnits) ? Resources.MessageIncompatibleSoftwareReprogrammingAccess : Resources.MessageIncompatibleSoftwareNoReprogrammingAccess) : (depotEcusCompatible ? (ApplicationInformation.CanReprogramEdexUnits ? Resources.MessageIncompatibleEdexSoftwareReprogrammingAccess : Resources.MessageIncompatibleEdexSoftwareNoReprogrammingAccess) : (flag3 ? (ApplicationInformation.CanReprogramDepotUnits ? Resources.MessageIncompatibleDepotSoftwareReprogrammingAccess : Resources.MessageIncompatibleDepotSoftwareNoReprogrammingAccess) : (ApplicationInformation.CanReprogramDepotUnits ? Resources.MessageIncompatibleDepotHardwareReprogrammingAccess : Resources.MessageIncompatibleDepotHardwareNoReprogrammingAccess)))));
					eventHandler = delegate
					{
						ListIncompatibilites(baseMessage, connectedUnit, depotEcusCompatible, edexEcusCompatible);
					};
					empty = empty + " " + Resources.MessageClickHereForMoreInformation;
					WarningsPanel.GlobalInstance.Add("incompatibility", MessageBoxIcon.Exclamation, string.Empty, empty, eventHandler);
				}
			}
			if (flag || (depotEcusCompatible && edexEcusCompatible))
			{
				WarningsPanel.GlobalInstance.Remove("incompatibility");
			}
		}
		else
		{
			WarningsPanel.GlobalInstance.Add("compatibilitytable", MessageBoxIcon.Hand, string.Empty, string.Format(CultureInfo.CurrentCulture, Resources.FormatServerProvidedFileMissing, ApplicationInformation.ProductName), (EventHandler)OnMenuUpdateClick);
		}
	}

	private static void CheckPendingUploadData()
	{
		if (!ServerClient.GlobalInstance.InUse && !ServerDataManager.GlobalInstance.Programming && (ServerDataManager.HaveEventsToUpload || ServerDataManager.HaveTroubleshootingReportFilesToUpload || ServerDataManager.HaveIncidentReportFilesToUpload))
		{
			WarningsPanel.GlobalInstance.Add("serveruploadneeded", MessageBoxIcon.Exclamation, string.Empty, Resources.Message_ServerUploadNeeded, (EventHandler)delegate
			{
				Collection<UnitInformation> collection = new Collection<UnitInformation>();
				if (ApplicationInformation.CanReprogramDepotUnits || ApplicationInformation.CanReprogramEdexUnits)
				{
					ServerDataManager.GlobalInstance.GetUploadUnits(collection, (UploadType)0);
				}
				ServerClient.GlobalInstance.Go((Collection<UnitInformation>)null, collection);
			});
		}
		else
		{
			WarningsPanel.GlobalInstance.Remove("serveruploadneeded");
		}
	}

	public void Undo()
	{
	}

	public void Copy()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		ClipboardData val = new ClipboardData();
		foreach (EcuStatusItem control in tableLayoutPanel.Controls)
		{
			control.Copy(val);
		}
		val.SetDataToClipboard();
	}

	public void Cut()
	{
	}

	public void Delete()
	{
	}

	public void Paste()
	{
	}

	public void SelectAll()
	{
	}

	private void item_DoubleClick(object sender, EventArgs e)
	{
		if (this.ItemActivate != null)
		{
			this.ItemActivate(this, e);
		}
	}

	private void OnClearConnectionErrorsClick(object sender, EventArgs e)
	{
		if (selectedIdentifier == null || GetChannelForIdentifier(selectedIdentifier, SapiManager.GlobalInstance.ActiveChannels) != null)
		{
			CheckOrRemoveErrorItem(remove: true);
			return;
		}
		EcuStatusItem ecuStatusItem = FindItemByTag(selectedIdentifier);
		if (ecuStatusItem != null)
		{
			RemoveFromTableLayout(ecuStatusItem);
		}
	}

	private bool CheckOrRemoveErrorItem(bool remove)
	{
		IEnumerable<EcuStatusItem> enumerable = (from i in tableLayoutPanel.Controls.OfType<EcuStatusItem>()
			where i.Icon == ConnectionIcon.Red
			select i).ToList();
		if (remove)
		{
			foreach (EcuStatusItem item in enumerable)
			{
				RemoveFromTableLayout(item);
			}
		}
		return enumerable.Any();
	}

	private void OnCloseAllConnectionsClick(object sender, EventArgs e)
	{
		SapiManager.GlobalInstance.CloseAllConnections();
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		Rectangle clientRectangle = headerPanel.ClientRectangle;
		ThemeProvider.GlobalInstance.ActiveTheme.DrawBackground(e.Graphics, clientRectangle, SystemColors.Window, false);
		e.Graphics.DrawLine(SystemPens.ControlDark, new Point(clientRectangle.Left, clientRectangle.Bottom - 1), new Point(clientRectangle.Right, clientRectangle.Bottom - 1));
	}

	private void SetTableLayoutRows()
	{
		bool flag = false;
		bool flag2 = false;
		int num = 0;
		IEnumerable<EcuStatusItem> orderedControls = (from esi in tableLayoutPanel.Controls.OfType<EcuStatusItem>()
			orderby esi.Ecu.Priority
			select esi).ToList();
		foreach (EcuStatusItem item in orderedControls)
		{
			if (!item.Ecu.IsVirtual)
			{
				if (item.Ecu.IsRollCall)
				{
					item.Visible = !SapiManager.GlobalInstance.CoalesceFaultCodes || !item.Ecu.RelatedEcus.Any((Ecu e) => orderedControls.Any((EcuStatusItem esi) => esi.Ecu == e && SapiExtensions.GetItem(activeChannels, esi.Ecu) != null));
					if (flag2 && item.Visible && !flag)
					{
						tableLayoutPanel.SetRow((Control)(object)separatorLine, num++);
						flag = (((Control)(object)separatorLine).Visible = true);
					}
				}
				else
				{
					item.Visible = !SapiManager.GlobalInstance.CoalesceFaultCodes || SapiExtensions.GetItem(activeChannels, item.Ecu) != null || !item.Ecu.RelatedEcus.Any((Ecu e) => orderedControls.Any((EcuStatusItem esi) => esi.Ecu == e));
					if (item.Visible)
					{
						flag2 = true;
					}
				}
			}
			else
			{
				item.Visible = false;
			}
			tableLayoutPanel.SetRow(item, num++);
			item.TabIndex = num;
		}
		if (!flag)
		{
			((Control)(object)separatorLine).Visible = false;
		}
	}

	private void AddToTableLayout(EcuStatusItem item)
	{
		tableLayoutPanel.SuspendLayout();
		tableLayoutPanel.Controls.Add(item);
		tableLayoutPanel.RowCount++;
		tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
		tableLayoutPanel.SetColumn(item, 0);
		SetTableLayoutRows();
		tableLayoutPanel.ResumeLayout();
		item.MouseUp += control_MouseUp;
		item.DoubleClick += item_DoubleClick;
	}

	private void RemoveFromTableLayout(EcuStatusItem item)
	{
		item.MouseUp -= control_MouseUp;
		item.DoubleClick -= item_DoubleClick;
		tableLayoutPanel.SuspendLayout();
		int row = tableLayoutPanel.GetRow(item);
		tableLayoutPanel.RowCount--;
		tableLayoutPanel.RowStyles.RemoveAt(row);
		tableLayoutPanel.Controls.Remove(item);
		SetTableLayoutRows();
		tableLayoutPanel.ResumeLayout();
		if (item.Identifier == selectedIdentifier)
		{
			selectedIdentifier = null;
		}
		Update();
	}

	private void ClearTableLayout()
	{
		tableLayoutPanel.Controls.Clear();
		tableLayoutPanel.RowStyles.Clear();
		tableLayoutPanel.RowCount = 0;
		selectedIdentifier = null;
		((Control)(object)separatorLine).Visible = false;
		tableLayoutPanel.Controls.Add((Control)(object)separatorLine, 0, 0);
		tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
	}

	private void control_MouseUp(object sender, MouseEventArgs e)
	{
		selectedIdentifier = (sender as EcuStatusItem)?.Identifier;
		if (e.Button == MouseButtons.Right)
		{
			connectionContextMenu.Show(sender as Control, new Point(e.X, e.Y));
		}
	}

	private void LogFile_LogFilePlaybackStateUpdateEvent(object sender, EventArgs e)
	{
		LogFile val = (LogFile)((sender is LogFile) ? sender : null);
		headerLabel.Text = (val.Playing ? Resources.EcuStatus_LoggedConnectionsHeading : Resources.EcuStatus_LoggedConnectionsPausedHeading);
	}
}
