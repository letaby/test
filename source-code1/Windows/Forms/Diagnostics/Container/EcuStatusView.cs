// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.EcuStatusView
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Common;
using DetroitDiesel.Common.Status;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using DetroitDiesel.Windows.Forms.Themed;
using Microsoft.Win32;
using SapiLayer1;
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
using System.Windows.Forms.Layout;

#nullable disable
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

  protected override void Dispose(bool disposing)
  {
    this.bodyPanel.MouseUp -= new MouseEventHandler(this.control_MouseUp);
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (EcuStatusView));
    this.menuItemConnect = new MenuItem();
    this.menuItemRefreshAutoConnect = new MenuItem();
    this.menuItemCloseAllConnections = new MenuItem();
    this.menuItemSeparator2 = new MenuItem();
    this.menuItemClearConnectionErrors = new MenuItem();
    this.tableLayoutPanel = new TableLayoutPanel();
    this.connectionContextMenu = new ContextMenu();
    this.headerLabel = new Label();
    this.headerPanel = new Panel();
    this.bodyPanel = new Panel();
    this.separatorLine = new EtchedLine();
    MenuItem menuItem = new MenuItem();
    this.tableLayoutPanel.SuspendLayout();
    this.headerPanel.SuspendLayout();
    this.bodyPanel.SuspendLayout();
    this.SuspendLayout();
    menuItem.Index = 1;
    componentResourceManager.ApplyResources((object) menuItem, "menuItemSeparator");
    this.menuItemConnect.Index = 0;
    componentResourceManager.ApplyResources((object) this.menuItemConnect, "menuItemConnect");
    this.menuItemConnect.Click += new EventHandler(this.OnConnectClick);
    this.menuItemRefreshAutoConnect.Index = 2;
    componentResourceManager.ApplyResources((object) this.menuItemRefreshAutoConnect, "menuItemRefreshAutoConnect");
    this.menuItemRefreshAutoConnect.Click += new EventHandler(this.OnRefreshAutoConnectClick);
    this.menuItemCloseAllConnections.Index = 3;
    componentResourceManager.ApplyResources((object) this.menuItemCloseAllConnections, "menuItemCloseAllConnections");
    this.menuItemCloseAllConnections.Click += new EventHandler(this.OnCloseAllConnectionsClick);
    this.menuItemSeparator2.Index = 4;
    componentResourceManager.ApplyResources((object) this.menuItemSeparator2, "menuItemSeparator2");
    this.menuItemClearConnectionErrors.Index = 5;
    componentResourceManager.ApplyResources((object) this.menuItemClearConnectionErrors, "menuItemClearConnectionErrors");
    this.menuItemClearConnectionErrors.Click += new EventHandler(this.OnClearConnectionErrorsClick);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    this.tableLayoutPanel.BackColor = SystemColors.Window;
    this.tableLayoutPanel.Controls.Add((Control) this.separatorLine, 0, 0);
    this.tableLayoutPanel.Name = "tableLayoutPanel";
    this.connectionContextMenu.MenuItems.AddRange(new MenuItem[6]
    {
      this.menuItemConnect,
      menuItem,
      this.menuItemRefreshAutoConnect,
      this.menuItemCloseAllConnections,
      this.menuItemSeparator2,
      this.menuItemClearConnectionErrors
    });
    this.connectionContextMenu.Popup += new EventHandler(this.ConnectionMenuPopup);
    componentResourceManager.ApplyResources((object) this.headerLabel, "headerLabel");
    this.headerLabel.BackColor = System.Drawing.Color.Transparent;
    this.headerLabel.Name = "headerLabel";
    componentResourceManager.ApplyResources((object) this.headerPanel, "headerPanel");
    this.headerPanel.BackColor = System.Drawing.Color.Transparent;
    this.headerPanel.Controls.Add((Control) this.headerLabel);
    this.headerPanel.Name = "headerPanel";
    componentResourceManager.ApplyResources((object) this.bodyPanel, "bodyPanel");
    this.bodyPanel.BackColor = SystemColors.Window;
    this.bodyPanel.Controls.Add((Control) this.tableLayoutPanel);
    this.bodyPanel.Name = "bodyPanel";
    this.bodyPanel.TabStop = true;
    componentResourceManager.ApplyResources((object) this.separatorLine, "separatorLine");
    ((Control) this.separatorLine).Name = "separatorLine";
    this.BackColor = SystemColors.Window;
    this.BorderStyle = BorderStyle.FixedSingle;
    this.Controls.Add((Control) this.bodyPanel);
    this.Controls.Add((Control) this.headerPanel);
    this.Cursor = Cursors.Arrow;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.Name = nameof (EcuStatusView);
    this.tableLayoutPanel.ResumeLayout(false);
    this.headerPanel.ResumeLayout(false);
    this.headerPanel.PerformLayout();
    this.bodyPanel.ResumeLayout(false);
    this.bodyPanel.PerformLayout();
    this.ResumeLayout(false);
    this.PerformLayout();
  }

  public event EventHandler ItemActivate;

  public EcuStatusView()
  {
    this.isDesignMode = LicenseManager.UsageMode == LicenseUsageMode.Designtime;
    SapiManager.GlobalInstance.ActiveChannelsChanged += new EventHandler(this.GlobalInstanceActiveChannelsChanged);
    this.InitializeComponent();
    this.ClearTableLayout();
    this.bodyPanel.MouseUp += new MouseEventHandler(this.control_MouseUp);
    this.ForeColor = SystemColors.WindowText;
    this.BackColor = SystemColors.Window;
  }

  private EcuStatusItem FindItemByTag(string identifierSought)
  {
    return this.tableLayoutPanel.Controls.OfType<EcuStatusItem>().Where<EcuStatusItem>((Func<EcuStatusItem, bool>) (item => item.Identifier == identifierSought)).FirstOrDefault<EcuStatusItem>();
  }

  private void OnConnectUpdateEvent(object sender, ProgressEventArgs e)
  {
    if (this.IsDisposed || !(sender is ConnectionResource connectionResource) || SapiManager.GlobalInstance.Sapi.Ecus.GetConnectedCountForIdentifier(connectionResource.Identifier) != 0 || this.activeChannels is LogFileChannelCollection)
      return;
    this.AddOrUpdateItem(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.EcuStatus_Format_Connecting, (object) connectionResource.Ecu.Name, (object) e.PercentComplete.ToString((IFormatProvider) CultureInfo.CurrentCulture), (object) SapiExtensions.ToDisplayString(connectionResource)), ConnectionIcon.Yellow, connectionResource.Ecu, connectionResource.Identifier);
  }

  private void AddOrUpdateItem(
    string statusText,
    ConnectionIcon imageIndex,
    Ecu ecu,
    string identifier)
  {
    EcuStatusItem ecuStatusItem = this.FindItemByTag(identifier);
    if (ecuStatusItem == null)
    {
      ecuStatusItem = new EcuStatusItem(identifier);
      ecuStatusItem.Ecu = ecu;
      this.AddToTableLayout(ecuStatusItem);
    }
    else if (ecuStatusItem.Ecu != ecu)
    {
      ecuStatusItem.Ecu = ecu;
      this.SetTableLayoutRows();
    }
    ecuStatusItem.StatusText = statusText;
    ecuStatusItem.Icon = imageIndex;
    ecuStatusItem.Notification.Count = 0;
  }

  private static bool IsFirewallConfigurationIncorrect
  {
    get
    {
      if (!EcuStatusView.isFirewallConfigurationIncorrect.HasValue)
      {
        bool flag1 = false;
        bool flag2 = false;
        try
        {
          RegistryKey registryKey1 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\WindowsFirewall\\PublicProfile");
          if (registryKey1 != null)
          {
            object obj = registryKey1.GetValue("AllowLocalPolicyMerge");
            if (obj != null && (int) obj == 0)
              flag1 = true;
          }
          if (flag1)
          {
            RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\WindowsFirewall\\FirewallRules");
            if (registryKey2 != null)
            {
              foreach (string valueName in registryKey2.GetValueNames())
              {
                object obj = registryKey2.GetValue(valueName);
                if (obj != null && obj is string str && str.IndexOf("Action=Allow", StringComparison.OrdinalIgnoreCase) != -1 && (str.IndexOf("LPort=13400", StringComparison.Ordinal) != -1 || str.IndexOf("LPort2_10=13400", StringComparison.Ordinal) != -1))
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
          StatusLog.Add(new StatusMessage("SecurityException in IsFirewallConfigurationIncorrect:" + ex.Message, (StatusMessageType) 1, (object) typeof (EcuStatusView)));
        }
        catch (UnauthorizedAccessException ex)
        {
          StatusLog.Add(new StatusMessage("UnauthorizedAccesException in IsFirewallConfigurationIncorrect:" + ex.Message, (StatusMessageType) 1, (object) typeof (EcuStatusView)));
        }
        EcuStatusView.isFirewallConfigurationIncorrect = new bool?(flag1 && !flag2);
      }
      return EcuStatusView.isFirewallConfigurationIncorrect.Value;
    }
  }

  private static void CheckForFirewallConfigurationIssue()
  {
    if (!EcuStatusView.IsFirewallConfigurationIncorrect)
      return;
    List<Channel> list = ((IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels).Where<Channel>((Func<Channel, bool>) (c => c.ConnectionResource != null && c.ConnectionResource.IsEthernet)).ToList<Channel>();
    if (list.Any<Channel>())
      WarningsPanel.GlobalInstance.Add("firewallconfigincorrect", MessageBoxIcon.Exclamation, (string) null, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.EcuStatusView_FormatFirewallConfigurationIncorrect, (object) ApplicationInformation.ProductName, (object) string.Join(" / ", list.Select<Channel, string>((Func<Channel, string>) (me => me.Ecu.Name)).ToArray<string>())), (EventHandler) null);
    else
      WarningsPanel.GlobalInstance.Remove("firewallconfigincorrect");
  }

  private void CheckForCertificateManagementError(Ecu ecu, string message)
  {
    if (message != null)
    {
      if (message.IndexOf("localhost/0:0:0:0:0:0:0:1:61000", StringComparison.OrdinalIgnoreCase) != -1)
        WarningsPanel.GlobalInstance.Add("wrongcertsystem" + ecu.Name, MessageBoxIcon.Hand, (string) null, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.EcuStatusView_WrongCertificateManagementServerReference, (object) ecu.Name), (EventHandler) null);
      else if (message.IndexOf("localhost/0:0:0:0:0:0:0:1:62000", StringComparison.OrdinalIgnoreCase) != -1)
      {
        ServiceController serviceController = ((IEnumerable<ServiceController>) ServiceController.GetServices()).FirstOrDefault<ServiceController>((Func<ServiceController, bool>) (s => s.ServiceName == "ZenZefiT"));
        bool flag = serviceController != null;
        string format = flag && serviceController.Status == ServiceControllerStatus.Running ? Resources.EcuStatusView_CertificateManagementServerNotResponding : (flag ? Resources.EcuStatusView_CertificateManagementServerNotRunning : Resources.EcuStatusView_CertificateManagementServerNotInstalled);
        if (!this.certManagerUnavailableEcus.Contains(ecu))
          this.certManagerUnavailableEcus.Add(ecu);
        this.certificatesUnavailableEcus.Clear();
        WarningsPanel.GlobalInstance.Add("certsunavailable", MessageBoxIcon.Exclamation, (string) null, string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, (object) string.Join(" / ", this.certManagerUnavailableEcus.Select<Ecu, string>((Func<Ecu, string>) (me => me.Name)).ToArray<string>())), (EventHandler) null);
      }
      else
      {
        if (message.IndexOf("No Certificate was found matching the filter criteria", StringComparison.OrdinalIgnoreCase) == -1)
          return;
        string certificatesNotAvailable = Resources.EcuStatusView_CertificatesNotAvailable;
        if (!this.certificatesUnavailableEcus.Contains(ecu))
          this.certificatesUnavailableEcus.Add(ecu);
        this.certManagerUnavailableEcus.Clear();
        WarningsPanel.GlobalInstance.Add("certsunavailable", MessageBoxIcon.Exclamation, (string) null, string.Format((IFormatProvider) CultureInfo.CurrentCulture, certificatesNotAvailable, (object) string.Join(" / ", this.certificatesUnavailableEcus.Select<Ecu, string>((Func<Ecu, string>) (me => me.Name)).ToArray<string>())), (EventHandler) null);
      }
    }
    else
    {
      if (this.certManagerUnavailableEcus.Contains(ecu))
        this.certManagerUnavailableEcus.Clear();
      if (this.certificatesUnavailableEcus.Contains(ecu))
        this.certificatesUnavailableEcus.Remove(ecu);
      if (this.certificatesUnavailableEcus.Count != 0 || this.certManagerUnavailableEcus.Count != 0)
        return;
      WarningsPanel.GlobalInstance.Remove("certsunavailable");
    }
  }

  private void channels_ConnectCompleteEvent(object sender, ConnectCompleteEventArgs e)
  {
    if (this.IsDisposed)
      return;
    if (sender is Channel channel1)
    {
      if (!((ResultEventArgs) e).Succeeded)
        return;
      this.AddChannel(channel1);
      if (!channel1.Ecu.IsMcd)
        return;
      this.CheckForCertificateManagementError(channel1.Ecu, (string) null);
      if (channel1.ConnectionResource == null || !channel1.ConnectionResource.IsEthernet)
        return;
      EcuStatusView.CheckForFirewallConfigurationIssue();
    }
    else
    {
      ConnectionResource connectionResource = sender as ConnectionResource;
      if (connectionResource == null)
        return;
      if (connectionResource.Ecu.IsRollCall)
      {
        EcuStatusItem itemByTag = this.FindItemByTag(connectionResource.Identifier);
        if (itemByTag == null)
          return;
        this.RemoveFromTableLayout(itemByTag);
      }
      else
      {
        CaesarException exception = ((ResultEventArgs) e).Exception as CaesarException;
        if (exception.ErrorNumber == 6611L || this.activeChannels is LogFileChannelCollection)
          return;
        Channel channel = ((IEnumerable<Channel>) this.activeChannels).FirstOrDefault<Channel>((Func<Channel, bool>) (c => c.Identifier == connectionResource.Identifier && c.Online));
        if (channel == null)
        {
          this.AddOrUpdateItem(string.Format((IFormatProvider) CultureInfo.CurrentCulture, e.AutoConnect ? Resources.FormatAutoConnectFailed : Resources.FormatConnectFailed, (object) connectionResource.Ecu.Name, (object) ((ResultEventArgs) e).Exception.Message), ConnectionIcon.Red, connectionResource.Ecu, connectionResource.Identifier);
          this.lastFailedResource[connectionResource.Identifier] = Tuple.Create<ConnectionResource, ChannelOptions>(connectionResource, e.ChannelOptions);
          if (!connectionResource.Ecu.IsMcd)
            return;
          this.CheckForCertificateManagementError(connectionResource.Ecu, ((Exception) exception).Message);
        }
        else
          StatusLog.Add(new StatusMessage($"Ignoring {connectionResource.Ecu.Name} connection failure ({((Exception) exception).Message}), because we're connected to {channel.Ecu.Name}", (StatusMessageType) 2, (object) this));
      }
    }
  }

  private void OnDisconnectCompleteEvent(object sender, EventArgs e)
  {
    Channel channel = sender as Channel;
    if (EcuStatusView.GetChannelForIdentifier(channel.Identifier, this.activeChannels) == null)
    {
      if (SapiManager.GlobalInstance.IsManualConnection(channel.Identifier) && channel.DisconnectionException != null && ((IEnumerable<CommunicationsStateValue>) channel.CommunicationsStateValues).Any<CommunicationsStateValue>((Func<CommunicationsStateValue, bool>) (csv => csv.Value == 2)))
        this.lastFailedResource[channel.Identifier] = Tuple.Create<ConnectionResource, ChannelOptions>(channel.ConnectionResource, channel.ChannelOptions);
      else
        this.RemoveChannel(channel);
    }
    if (!channel.Ecu.IsMcd || channel.ConnectionResource == null || !channel.ConnectionResource.IsEthernet)
      return;
    EcuStatusView.CheckForFirewallConfigurationIssue();
  }

  private void UpdateChannelState(Channel channel, CommunicationsStateValue current = null)
  {
    EcuStatusItem itemByTag = this.FindItemByTag(channel.Identifier);
    if (itemByTag == null)
      return;
    string empty = string.Empty;
    CommunicationsState communicationsState;
    string str1;
    if (current != null)
    {
      communicationsState = current.Value;
      str1 = SapiExtensions.FormatDisplayString(current, false);
      if ((current.Value == 3 || current.Value == 2) && channel.DisconnectionException != null)
        str1 = $"{str1} - {((Exception) channel.DisconnectionException).Message}";
    }
    else if (channel.CommunicationsStateValues.Current != null)
    {
      communicationsState = channel.CommunicationsStateValues.Current.Value;
      str1 = SapiExtensions.FormatDisplayString(channel.CommunicationsStateValues.Current, false);
    }
    else if (channel.LogFile != null && ((ReadOnlyCollection<CommunicationsStateValue>) channel.CommunicationsStateValues).Count > 0 && channel.LogFile.CurrentTime < ((ReadOnlyCollection<CommunicationsStateValue>) channel.CommunicationsStateValues)[0].Time)
    {
      communicationsState = ((ReadOnlyCollection<CommunicationsStateValue>) channel.CommunicationsStateValues)[0].Value;
      str1 = SapiExtensions.FormatDisplayString(((ReadOnlyCollection<CommunicationsStateValue>) channel.CommunicationsStateValues)[0], false);
    }
    else
    {
      communicationsState = channel.CommunicationsState;
      str1 = SapiExtensions.FormatDisplayString(channel.CommunicationsState);
    }
    itemByTag.Icon = communicationsState == 1 ? ConnectionIcon.Green : (communicationsState - 2 <= 1 ? ConnectionIcon.Red : ConnectionIcon.Yellow);
    List<string> source = new List<string>();
    if (communicationsState == 1)
    {
      source.AddRange((IEnumerable<string>) EcuStatusView.ActionableFaultCodes((IEnumerable) channel.FaultCodes.Current, (FaultCodeDisplayIncludeInfo) 1));
      if (!channel.IsRollCall && SapiManager.GlobalInstance.CoalesceFaultCodes)
      {
        IEnumerable<FaultCode> faultCodes1 = channel.RelatedChannels.SelectMany<Channel, FaultCode>((Func<Channel, IEnumerable<FaultCode>>) (rc => rc.FaultCodes.Current));
        IEnumerable<FaultCode> second = faultCodes1.Where<FaultCode>((Func<FaultCode, bool>) (rcf => rcf.Channel.RelatedChannels.SelectMany<Channel, FaultCode>((Func<Channel, IEnumerable<FaultCode>>) (rrc => rrc.FaultCodes.Current)).Any<FaultCode>((Func<FaultCode, bool>) (rrcf => rrcf.IsRelated(rcf)))));
        IEnumerable<FaultCode> faultCodes2 = faultCodes1.Except<FaultCode>(second);
        source.AddRange((IEnumerable<string>) EcuStatusView.ActionableFaultCodes((IEnumerable) faultCodes2, (FaultCodeDisplayIncludeInfo) 3));
      }
    }
    int num = source.Count<string>();
    if (num > 0)
    {
      EcuStatusItem ecuStatusItem = itemByTag;
      CultureInfo currentCulture = CultureInfo.CurrentCulture;
      string formatActiveFaults = Resources.EcuStatus_Format_ActiveFaults;
      object[] objArray = new object[4]
      {
        (object) channel.Ecu.Name,
        (object) str1,
        null,
        null
      };
      ConnectionResource connectionResource = SapiExtensions.GetActiveConnectionResource(channel);
      objArray[2] = (object) (connectionResource != null ? SapiExtensions.ToDisplayString(connectionResource) : (string) null);
      objArray[3] = (object) string.Join(Environment.NewLine, source.ToArray());
      string str2 = string.Format((IFormatProvider) currentCulture, formatActiveFaults, objArray);
      ecuStatusItem.StatusText = str2;
    }
    else
    {
      EcuStatusItem ecuStatusItem = itemByTag;
      CultureInfo currentCulture = CultureInfo.CurrentCulture;
      string formatOtherState = Resources.EcuStatus_Format_OtherState;
      string name = channel.Ecu.Name;
      string str3 = str1;
      ConnectionResource connectionResource = SapiExtensions.GetActiveConnectionResource(channel);
      string displayString = connectionResource != null ? SapiExtensions.ToDisplayString(connectionResource) : (string) null;
      string str4 = string.Format((IFormatProvider) currentCulture, formatOtherState, (object) name, (object) str3, (object) displayString);
      ecuStatusItem.StatusText = str4;
    }
    itemByTag.Notification.Count = num;
  }

  private static List<string> ActionableFaultCodes(
    IEnumerable faultCodes,
    FaultCodeDisplayIncludeInfo format)
  {
    List<string> stringList = new List<string>();
    foreach (FaultCode faultCode in faultCodes)
    {
      FaultCodeIncident current = faultCode.FaultCodeIncidents.Current;
      if (current != null && SapiManager.IsFaultActionable(current))
        stringList.Add(SapiExtensions.FormatDisplayString(current.FaultCode, format));
    }
    return stringList;
  }

  private void OnCommunicationsStateValueUpdateEvent(
    object sender,
    CommunicationsStateValueEventArgs e)
  {
    Channel channel = sender as Channel;
    this.UpdateChannelState(channel, e.CommunicationsStateValue);
    if (!this.pendingCompatibilityCheck.Contains(channel) || e.CommunicationsStateValue.Value == 4)
      return;
    this.pendingCompatibilityCheck.Remove(channel);
    EcuStatusView.CheckCompatibility();
  }

  private void OnFaultCodeCollectionUpdateEvent(object sender, ResultEventArgs e)
  {
    FaultCodeCollection faultCodes = sender as FaultCodeCollection;
    this.UpdateChannelState(faultCodes.Channel);
    if (!SapiManager.GlobalInstance.CoalesceFaultCodes)
      return;
    if (faultCodes.Channel.IsRollCall)
    {
      foreach (Channel relatedChannel in faultCodes.Channel.RelatedChannels)
        this.UpdateChannelState(relatedChannel);
    }
    else
    {
      foreach (Channel channel in faultCodes.Channel.RelatedChannels.SelectMany<Channel, Channel>((Func<Channel, IEnumerable<Channel>>) (rc => rc.RelatedChannels)).Where<Channel>((Func<Channel, bool>) (rrc => rrc != faultCodes.Channel)))
        this.UpdateChannelState(channel);
    }
  }

  private void OnConnectClick(object sender, EventArgs e)
  {
    if (this.selectedIdentifier == null)
    {
      using (ConnectionDialog connectionDialog = new ConnectionDialog())
      {
        int num = (int) connectionDialog.ShowDialog();
      }
    }
    else
    {
      Channel channelForIdentifier = EcuStatusView.GetChannelForIdentifier(this.selectedIdentifier, SapiManager.GlobalInstance.ActiveChannels);
      if (channelForIdentifier == null)
      {
        Tuple<ConnectionResource, ChannelOptions> tuple;
        if (!this.lastFailedResource.TryGetValue(this.selectedIdentifier, out tuple))
          return;
        SapiManager.GlobalInstance.OpenConnection(tuple.Item1, tuple.Item2);
      }
      else
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
        return channel;
    }
    return (Channel) null;
  }

  private static IEnumerable<Ecu> GetEcusForIdentifier(string identifier)
  {
    return SapiManager.GlobalInstance != null && SapiManager.GlobalInstance.Sapi != null ? (IEnumerable<Ecu>) ((IEnumerable) SapiManager.GlobalInstance.Sapi.Ecus).Cast<Ecu>().Where<Ecu>((Func<Ecu, bool>) (ecu => ecu.Identifier == identifier)).ToList<Ecu>().AsReadOnly() : Enumerable.Empty<Ecu>();
  }

  private void ConnectionMenuPopup(object sender, EventArgs e)
  {
    string str1 = Resources.EcuStatusMenu_Connect;
    string str2 = Resources.EcuStatusMenu_ClearAllConnectionErrors;
    bool flag1 = false;
    bool flag2 = false;
    bool online = SapiManager.GlobalInstance.Online;
    if (this.CheckOrRemoveErrorItem(false))
      flag2 = true;
    if (this.selectedIdentifier == null)
    {
      flag1 = true;
    }
    else
    {
      Channel channelForIdentifier = EcuStatusView.GetChannelForIdentifier(this.selectedIdentifier, SapiManager.GlobalInstance.ActiveChannels);
      if (channelForIdentifier != null)
      {
        if (channelForIdentifier.Online)
        {
          if (channelForIdentifier.IsRollCall)
            flag1 = false;
          else if (channelForIdentifier.CommunicationsState == 1 || channelForIdentifier.CommunicationsState == 4 || channelForIdentifier.CommunicationsState == 5)
            flag1 = online;
          str1 = Resources.EcuStatusMenu_Disconnect;
        }
        else if (channelForIdentifier.LogFile == null)
        {
          flag1 = true;
          str1 = Resources.EcuStatusMenu_Close;
        }
      }
      else
      {
        str2 = Resources.EcuStatusMenu_ClearConnectionError;
        if (SapiManager.GlobalInstance.IsManualConnection(this.selectedIdentifier) || !SapiManager.GlobalInstance.IsSetForAutoConnect(this.selectedIdentifier))
        {
          str1 = Resources.EcuStatusMenu_Reconnect;
          if (this.lastFailedResource.ContainsKey(this.selectedIdentifier))
            flag1 = online;
        }
        else
        {
          str1 = Resources.EcuStatusMenu_RetryAutomaticConnection;
          flag1 = online;
        }
      }
    }
    this.menuItemConnect.Text = str1;
    this.menuItemConnect.Enabled = flag1 && !AdapterInformation.AdapterProhibited;
    this.menuItemRefreshAutoConnect.Enabled = SapiManager.GlobalInstance.AutoConnectNeedsRefresh && !AdapterInformation.AdapterProhibited;
    this.menuItemClearConnectionErrors.Text = str2;
    this.menuItemClearConnectionErrors.Enabled = flag2;
    if (SapiManager.GlobalInstance.Online)
    {
      this.menuItemCloseAllConnections.Text = Resources.CommandCloseConnections;
      this.menuItemCloseAllConnections.Enabled = ((ReadOnlyCollection<Channel>) SapiManager.GlobalInstance.Sapi.Channels).Count > 0;
    }
    else
    {
      this.menuItemCloseAllConnections.Text = Resources.CommandCloseLog;
      this.menuItemCloseAllConnections.Enabled = true;
    }
  }

  protected override void OnLoad(EventArgs e)
  {
    if (!this.isDesignMode)
    {
      EcuStatusView.CheckPendingUploadData();
      ServerDataManager.GlobalInstance.DataUpdated += new EventHandler(this.GlobalInstance_DataUpdated);
      ServerClient.GlobalInstance.InUseChanged += new EventHandler(this.ServerClient_InUseChanged);
    }
    SapiManager.GlobalInstance.TranslatorConnectedChanged += new EventHandler(this.GlobalInstance_TranslatorConnectedChanged);
    SapiManager.GlobalInstance.CoalesceFaultCodesChanged += new EventHandler(this.GlobalInstance_CoalesceFaultCodesChanged);
    this.UpdateActiveChannels();
    base.OnLoad(e);
  }

  private void GlobalInstance_CoalesceFaultCodesChanged(object sender, EventArgs e)
  {
    this.UpdateActiveChannels();
  }

  private void GlobalInstance_TranslatorConnectedChanged(object sender, EventArgs e)
  {
    IRollCall manager = ChannelCollection.GetManager((Protocol) 71993);
    if (manager != null && AdapterInformation.SupportsNativeIso == 1 && manager.DeviceSupportedProtocols != null && ((IEnumerable<string>) manager.DeviceSupportedProtocols.Split(",".ToCharArray())).Contains<string>("ISO15765"))
      WarningsPanel.GlobalInstance.Add("nativeisonotrecommended", MessageBoxIcon.Exclamation, string.Empty, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_NativeISONotRecommended, (object) manager.DeviceName), (EventHandler) ((snd, ev) =>
      {
        Program.RunSidConfigure(false);
        SapiManager.GlobalInstance.ResetSapi();
      }));
    else
      WarningsPanel.GlobalInstance.Remove("nativeisonotrecommended");
  }

  private void GlobalInstanceActiveChannelsChanged(object sender, EventArgs e)
  {
    this.UpdateActiveChannels();
  }

  private void UpdateActiveChannels()
  {
    this.ClearTableLayout();
    if (this.activeChannels != null)
    {
      this.activeChannels.ConnectCompleteEvent2 -= new EventHandler<ConnectCompleteEventArgs>(this.channels_ConnectCompleteEvent);
      this.activeChannels.DisconnectCompleteEvent -= new DisconnectCompleteEventHandler(this.OnDisconnectCompleteEvent);
      this.activeChannels.ConnectProgressEvent -= new ConnectProgressEventHandler(this.OnConnectUpdateEvent);
      foreach (Channel activeChannel in this.activeChannels)
        this.RemoveChannel(activeChannel);
      if (this.activeChannels is LogFileChannelCollection)
        (this.activeChannels as LogFileChannelCollection).LogFile.LogFilePlaybackStateUpdateEvent -= new LogFilePlaybackStateUpdateEventHandler(this.LogFile_LogFilePlaybackStateUpdateEvent);
    }
    this.activeChannels = SapiManager.GlobalInstance.ActiveChannels;
    if (this.activeChannels == null)
      return;
    this.activeChannels.ConnectCompleteEvent2 += new EventHandler<ConnectCompleteEventArgs>(this.channels_ConnectCompleteEvent);
    this.activeChannels.DisconnectCompleteEvent += new DisconnectCompleteEventHandler(this.OnDisconnectCompleteEvent);
    this.activeChannels.ConnectProgressEvent += new ConnectProgressEventHandler(this.OnConnectUpdateEvent);
    foreach (Channel activeChannel in this.activeChannels)
      this.AddChannel(activeChannel);
    if (this.activeChannels is LogFileChannelCollection activeChannels)
    {
      activeChannels.LogFile.LogFilePlaybackStateUpdateEvent += new LogFilePlaybackStateUpdateEventHandler(this.LogFile_LogFilePlaybackStateUpdateEvent);
      this.headerLabel.Text = activeChannels.LogFile.Playing ? Resources.EcuStatus_LoggedConnectionsHeading : Resources.EcuStatus_LoggedConnectionsPausedHeading;
    }
    else
      this.headerLabel.Text = Resources.EcuStatus_ConnectionsHeading;
  }

  private void AddChannel(Channel channel)
  {
    channel.CommunicationsStateValueUpdateEvent += new CommunicationsStateValueUpdateEventHandler(this.OnCommunicationsStateValueUpdateEvent);
    channel.InitCompleteEvent += new InitCompleteEventHandler(this.OnInitCompleteEvent);
    channel.FaultCodes.FaultCodesUpdateEvent += new FaultCodesUpdateEventHandler(this.OnFaultCodeCollectionUpdateEvent);
    channel.FlashAreas.FlashProgressUpdateEvent += new FlashProgressUpdateEventHandler(this.FlashAreas_FlashProgressUpdateEvent);
    channel.Parameters.ParameterUpdateEvent += new ParameterUpdateEventHandler(this.Parameters_ParameterUpdateEvent);
    this.AddOrUpdateItem(string.Empty, ConnectionIcon.Yellow, channel.Ecu, channel.Identifier);
    this.UpdateChannelState(channel);
    if (!channel.Ecu.IsRollCall && SapiManager.GlobalInstance.CoalesceFaultCodes && channel.Ecu.RelatedEcus.Any<Ecu>())
    {
      this.SetTableLayoutRows();
      this.Update();
    }
    if (channel.IsRollCall)
      return;
    CommunicationsStateValue current = channel.CommunicationsStateValues.Current;
    if (current != null && current.Value != 4)
      EcuStatusView.CheckCompatibility();
    else
      this.pendingCompatibilityCheck.Add(channel);
    EcuStatusView.CheckEthernetConnection();
  }

  private static void CheckEthernetConnection()
  {
    List<Channel> list = ((IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels).Where<Channel>((Func<Channel, bool>) (c => c.Ecu.IsMcd && c.Online && ((IEnumerable<EcuInterface>) c.Ecu.Interfaces).Any<EcuInterface>((Func<EcuInterface, bool>) (i => i.IsEthernet)))).ToList<Channel>();
    if (list.Select(channel => new
    {
      channel = channel,
      resource = SapiExtensions.GetActiveConnectionResource(channel)
    }).Where(_param1 => _param1.resource != null && _param1.resource.IsEthernet).Select(_param1 => _param1.channel).ToList<Channel>().Count == 0 && list.Any<Channel>((Func<Channel, bool>) (c => SapiExtensions.IsGateway(c.Ecu))))
      WarningsPanel.GlobalInstance.Add("NOTETHERNET", MessageBoxIcon.Exclamation, (string) null, Resources.Format_RequiresEthernetWarning, (EventHandler) null);
    else
      WarningsPanel.GlobalInstance.Remove("NOTETHERNET");
  }

  private void RemoveChannel(Channel channel)
  {
    channel.InitCompleteEvent -= new InitCompleteEventHandler(this.OnInitCompleteEvent);
    channel.CommunicationsStateValueUpdateEvent -= new CommunicationsStateValueUpdateEventHandler(this.OnCommunicationsStateValueUpdateEvent);
    channel.FaultCodes.FaultCodesUpdateEvent -= new FaultCodesUpdateEventHandler(this.OnFaultCodeCollectionUpdateEvent);
    channel.FlashAreas.FlashProgressUpdateEvent -= new FlashProgressUpdateEventHandler(this.FlashAreas_FlashProgressUpdateEvent);
    channel.Parameters.ParameterUpdateEvent -= new ParameterUpdateEventHandler(this.Parameters_ParameterUpdateEvent);
    EcuStatusItem itemByTag = this.FindItemByTag(channel.Identifier);
    if (itemByTag != null)
      this.RemoveFromTableLayout(itemByTag);
    if (!channel.IsRollCall)
    {
      EcuStatusView.CheckCompatibility();
      this.pendingCompatibilityCheck.Remove(channel);
      EcuStatusView.CheckEthernetConnection();
    }
    WarningsPanel.GlobalInstance.Remove(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "BASE{0}", (object) channel.Ecu.Name));
    WarningsPanel.GlobalInstance.Remove(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "INVALIDVARIANT{0}", (object) channel.Ecu.Name));
    WarningsPanel.GlobalInstance.Remove(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "PENDINGVARIANT{0}", (object) channel.Ecu.Name));
  }

  private void UpdateStatusWithPercentage(Channel channel, double percent)
  {
    EcuStatusItem itemByTag = this.FindItemByTag(channel.Identifier);
    if (itemByTag == null)
      return;
    CommunicationsState communicationsState = channel.CommunicationsState;
    string str1 = communicationsState == 5 ? Resources.EcuStatusView_Reading : (communicationsState == 6 ? Resources.EcuStatusView_Writing : SapiExtensions.FormatDisplayString(channel.CommunicationsState));
    EcuStatusItem ecuStatusItem = itemByTag;
    CultureInfo currentCulture = CultureInfo.CurrentCulture;
    string statusWithPercentage = Resources.EcuStatus_Format_OtherStatusWithPercentage;
    object[] objArray = new object[4]
    {
      (object) channel.Ecu.Name,
      (object) str1,
      (object) percent,
      null
    };
    ConnectionResource connectionResource = SapiExtensions.GetActiveConnectionResource(channel);
    objArray[3] = (object) (connectionResource != null ? SapiExtensions.ToDisplayString(connectionResource) : (string) null);
    string str2 = string.Format((IFormatProvider) currentCulture, statusWithPercentage, objArray);
    ecuStatusItem.StatusText = str2;
  }

  private void FlashAreas_FlashProgressUpdateEvent(object sender, ProgressEventArgs e)
  {
    if (!(sender is FlashAreaCollection flashAreaCollection) || flashAreaCollection.Channel.CommunicationsState != 7)
      return;
    this.UpdateStatusWithPercentage(flashAreaCollection.Channel, e.PercentComplete);
  }

  private void Parameters_ParameterUpdateEvent(object sender, ResultEventArgs e)
  {
    if (!(sender is Parameter parameter) || parameter.Channel.CommunicationsState != 5 && parameter.Channel.CommunicationsState != 6)
      return;
    this.UpdateStatusWithPercentage(parameter.Channel, (double) parameter.Channel.Parameters.Progress);
  }

  private void OnInitCompleteEvent(object sender, EventArgs e)
  {
    if (!(sender is Channel channel) || channel.IsRollCall)
      return;
    if (channel.DiagnosisVariant.IsBase)
    {
      if (SapiExtensions.IsDataSourceEdex(channel.Ecu) && SapiManager.GetBootModeStatus(channel))
        return;
      IEnumerable<Ecu> ecusForIdentifier = EcuStatusView.GetEcusForIdentifier(channel.Identifier);
      string str1 = string.Join(Resources.EcuStatus_Separator, ecusForIdentifier.Select<Ecu, string>((Func<Ecu, string>) (x => x.Name)).ToArray<string>());
      string str2 = ecusForIdentifier.Count<Ecu>() == 1 ? channel.Ecu.DescriptionDataVersion : string.Empty;
      string str3 = SapiManager.GetSoftwareVersion(channel) ?? string.Empty;
      if (str3.StartsWith("Security access denied", StringComparison.OrdinalIgnoreCase))
        str3 = string.Empty;
      string str4 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, ApplicationInformation.ProductAccessLevel == 3 ? Resources.FormatBaseVariantWarning_Engineering : Resources.FormatBaseVariantWarning, (object) str1, (object) str3, (object) str2, (object) ApplicationInformation.ProductName);
      WarningsPanel.GlobalInstance.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "BASE{0}", (object) channel.Ecu.Name), MessageBoxIcon.Hand, string.Empty, str4, ApplicationInformation.ProductAccessLevel == 3 ? (EventHandler) null : new EventHandler(EcuStatusView.OnMenuUpdateClick));
    }
    else
    {
      if (!ApplicationInformation.CheckValidVariants)
        return;
      if (!SapiExtensions.GetValidDiagnosisVariants(channel.Ecu).Contains<DiagnosisVariant>(channel.DiagnosisVariant) && !channel.DiagnosisVariant.IsBoot)
      {
        WarningsPanel.GlobalInstance.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "INVALIDVARIANT{0}", (object) channel.Ecu.Name), MessageBoxIcon.Hand, (string) null, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Format_InvalidVariantWarning, (object) channel.Ecu.Name, (object) channel.DiagnosisVariant.Name, (object) ApplicationInformation.ProductName), (EventHandler) null);
      }
      else
      {
        if (!SapiExtensions.IsPendingDiagnosisVariant(channel.DiagnosisVariant))
          return;
        WarningsPanel.GlobalInstance.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "PENDINGVARIANT{0}", (object) channel.Ecu.Name), MessageBoxIcon.Exclamation, (string) null, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Format_PendingVariantWarning, (object) channel.Ecu.Name, (object) channel.DiagnosisVariant.Name, (object) ApplicationInformation.ProductName), (EventHandler) null);
      }
    }
  }

  public static void OnMenuUpdateClick(object sender, EventArgs e)
  {
    ServerClient.GlobalInstance.Go((Collection<UnitInformation>) null, (Collection<UnitInformation>) null);
  }

  private static void ListIncompatibilites(
    string message,
    UnitInformation unit,
    bool depotEcusCompatible,
    bool edexEcusCompatible)
  {
    bool flag1 = false;
    bool flag2 = false;
    CompatibilityWarningDialog compatibilityWarningDialog = new CompatibilityWarningDialog(message);
    List<CompatibilityWarningMissingReason> warningMissingReasonList = new List<CompatibilityWarningMissingReason>();
    if (!edexEcusCompatible)
    {
      if (!ApplicationInformation.CanReprogramEdexUnits)
        warningMissingReasonList.Add(new CompatibilityWarningMissingReason(Resources.EcuStatusView_NoEdexReprogramming, (DeviceInformation.DeviceDataSource) 2));
      else if (unit != null)
      {
        bool flag3 = false;
        foreach (EdexCompatibilityEcuItem ecu in EdexCompatibilityTable.GetCurrentConnectedConfiguration().Ecus)
        {
          EdexCompatibilityConfigurationCollection compatibleList = ServerDataManager.GlobalInstance.EdexCompatibilityTable.CreateCompatibleList(ecu);
          if (!((IEnumerable<EdexCompatibilityConfiguration>) compatibleList).Any<EdexCompatibilityConfiguration>())
          {
            warningMissingReasonList.Add(new CompatibilityWarningMissingReason(Resources.EcuStatusView_TargetNotInCompat, ecu));
          }
          else
          {
            EdexCompatibilityConfigurationCollection source1 = compatibleList.FilterForOnlineDevices(ecu);
            if (((IEnumerable<EdexCompatibilityConfiguration>) source1).Any<EdexCompatibilityConfiguration>())
            {
              EdexCompatibilityConfigurationCollection source2 = source1.FilterForUnit(unit);
              if (!((IEnumerable<EdexCompatibilityConfiguration>) source2).Any<EdexCompatibilityConfiguration>())
                flag3 = true;
              else if (((IEnumerable<EdexCompatibilityConfiguration>) source2).Any<EdexCompatibilityConfiguration>((Func<EdexCompatibilityConfiguration, bool>) (cs => cs.Ecus.Any<EdexCompatibilityEcuItem>())))
              {
                compatibilityWarningDialog.AddCompatibilityInfo(ecu, Resources.EcuStatusView_Compat_TargetDevice, source2, Resources.EcuStatusView_Compat_CompatibleOptions);
                flag1 = true;
              }
            }
          }
        }
        if (flag1)
        {
          warningMissingReasonList.RemoveAll((Predicate<CompatibilityWarningMissingReason>) (x => x.DataSource == 2));
        }
        else
        {
          IEnumerable<EdexCompatibilityEcuItem> compatibilityEcuItems;
          if (flag3 && !((IEnumerable<EdexCompatibilityConfiguration>) ServerDataManager.GlobalInstance.EdexCompatibilityTable.CreateCompatibleList(unit, ref compatibilityEcuItems)).Any<EdexCompatibilityConfiguration>())
          {
            foreach (EdexCompatibilityEcuItem compatibilityEcuItem in compatibilityEcuItems)
              warningMissingReasonList.Add(new CompatibilityWarningMissingReason(Resources.EcuStatusView_UnitDataSoftwareNotInCompat, compatibilityEcuItem));
          }
        }
      }
      else
        warningMissingReasonList.Add(new CompatibilityWarningMissingReason(Resources.EcuStatusView_NoUnitData, (DeviceInformation.DeviceDataSource) 2));
    }
    if (!depotEcusCompatible)
    {
      if (!ApplicationInformation.CanReprogramDepotUnits)
        warningMissingReasonList.Add(new CompatibilityWarningMissingReason(Resources.EcuStatusView_NoDepotReprogramming, (DeviceInformation.DeviceDataSource) 1));
      else if (unit != null)
      {
        foreach (Software software in (ReadOnlyCollection<Software>) ServerDataManager.GlobalInstance.CompatibilityTable.CurrentSoftware)
        {
          CompatibleSoftwareCollection compatibleList = ServerDataManager.GlobalInstance.CompatibilityTable.CreateCompatibleList(software);
          if (!((IEnumerable<SoftwareCollection>) compatibleList).Any<SoftwareCollection>())
          {
            warningMissingReasonList.Add(new CompatibilityWarningMissingReason(Resources.EcuStatusView_TargetNotInCompat, software));
          }
          else
          {
            CompatibleSoftwareCollection source3 = compatibleList.FilterForUnit(unit);
            if (!((IEnumerable<SoftwareCollection>) source3).Any<SoftwareCollection>())
            {
              warningMissingReasonList.Add(new CompatibilityWarningMissingReason(Resources.EcuStatusView_UnitDataSoftwareSetNotInCompat, software));
            }
            else
            {
              CompatibleSoftwareCollection source4 = source3.FilterForOnlineOptions(software.Ecu);
              if (!((IEnumerable<SoftwareCollection>) source4).Any<SoftwareCollection>())
              {
                warningMissingReasonList.Add(new CompatibilityWarningMissingReason(Resources.EcuStatusView_ConnectedDevicesCompatible, software));
              }
              else
              {
                compatibilityWarningDialog.AddCompatibilityInfo(software, Resources.EcuStatusView_Compat_TargetDevice, source4, Resources.EcuStatusView_Compat_CompatibleOptions);
                flag2 = true;
              }
            }
          }
        }
        if (flag2)
          warningMissingReasonList.RemoveAll((Predicate<CompatibilityWarningMissingReason>) (x => x.DataSource == 1));
      }
      else
        warningMissingReasonList.Add(new CompatibilityWarningMissingReason(Resources.EcuStatusView_NoUnitData, (DeviceInformation.DeviceDataSource) 1));
    }
    compatibilityWarningDialog.AddNoCompatibilityInfoAvailableReasons((IEnumerable<CompatibilityWarningMissingReason>) warningMissingReasonList);
    int num = (int) ((Form) compatibilityWarningDialog).ShowDialog();
  }

  private void GlobalInstance_DataUpdated(object sender, EventArgs e)
  {
    EcuStatusView.CheckCompatibility();
    if (this.isDesignMode)
      return;
    EcuStatusView.CheckPendingUploadData();
  }

  private void ServerClient_InUseChanged(object sender, EventArgs e)
  {
    if (this.isDesignMode)
      return;
    EcuStatusView.CheckPendingUploadData();
  }

  private static void CheckCompatibility()
  {
    if (WarningsPanel.GlobalInstance == null)
      return;
    if (ServerDataManager.GlobalInstance.CompatibilityTable != null && ServerDataManager.GlobalInstance.EdexCompatibilityTable != null)
    {
      WarningsPanel.GlobalInstance.Remove("compatibilitytable");
      bool flag1 = false;
      bool flag2 = false;
      UnitInformation connectedUnit = ServerDataManager.GlobalInstance.ConnectedUnit;
      if (connectedUnit != null)
      {
        flag1 = connectedUnit.InAutomaticOperation;
        flag2 = connectedUnit.UnitFixedAtTest;
      }
      bool edexEcusCompatible = true;
      bool depotEcusCompatible = true;
      if (!flag1)
      {
        bool flag3 = ServerDataManager.GlobalInstance.CompatibilityTable.IsCurrentHardwareCompatibleWithCurrentSoftware();
        bool flag4 = flag2 || ServerDataManager.GlobalInstance.CompatibilityTable.IsCurrentSoftwareCompatible();
        depotEcusCompatible = flag3 & flag4;
        edexEcusCompatible = ServerDataManager.GlobalInstance.EdexCompatibilityTable.IsCurrentSoftwareCompatible();
        if (!(depotEcusCompatible & edexEcusCompatible))
        {
          string empty = string.Empty;
          string str1 = depotEcusCompatible || edexEcusCompatible ? (depotEcusCompatible ? (ApplicationInformation.CanReprogramEdexUnits ? Resources.MessageIncompatibleEdexSoftwareReprogrammingAccess : Resources.MessageIncompatibleEdexSoftwareNoReprogrammingAccess) : (flag3 ? (ApplicationInformation.CanReprogramDepotUnits ? Resources.MessageIncompatibleDepotSoftwareReprogrammingAccess : Resources.MessageIncompatibleDepotSoftwareNoReprogrammingAccess) : (ApplicationInformation.CanReprogramDepotUnits ? Resources.MessageIncompatibleDepotHardwareReprogrammingAccess : Resources.MessageIncompatibleDepotHardwareNoReprogrammingAccess))) : (!ApplicationInformation.CanReprogramDepotUnits || !ApplicationInformation.CanReprogramEdexUnits ? Resources.MessageIncompatibleSoftwareNoReprogrammingAccess : Resources.MessageIncompatibleSoftwareReprogrammingAccess);
          string baseMessage = str1;
          EventHandler eventHandler = (EventHandler) ((e, args) => EcuStatusView.ListIncompatibilites(baseMessage, connectedUnit, depotEcusCompatible, edexEcusCompatible));
          string str2 = $"{str1} {Resources.MessageClickHereForMoreInformation}";
          WarningsPanel.GlobalInstance.Add("incompatibility", MessageBoxIcon.Exclamation, string.Empty, str2, eventHandler);
        }
      }
      if (!flag1 && !(depotEcusCompatible & edexEcusCompatible))
        return;
      WarningsPanel.GlobalInstance.Remove("incompatibility");
    }
    else
      WarningsPanel.GlobalInstance.Add("compatibilitytable", MessageBoxIcon.Hand, string.Empty, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatServerProvidedFileMissing, (object) ApplicationInformation.ProductName), new EventHandler(EcuStatusView.OnMenuUpdateClick));
  }

  private static void CheckPendingUploadData()
  {
    if (!ServerClient.GlobalInstance.InUse && !ServerDataManager.GlobalInstance.Programming && (ServerDataManager.HaveEventsToUpload || ServerDataManager.HaveTroubleshootingReportFilesToUpload || ServerDataManager.HaveIncidentReportFilesToUpload))
      WarningsPanel.GlobalInstance.Add("serveruploadneeded", MessageBoxIcon.Exclamation, string.Empty, Resources.Message_ServerUploadNeeded, (EventHandler) ((sender, e) =>
      {
        Collection<UnitInformation> collection = new Collection<UnitInformation>();
        if (ApplicationInformation.CanReprogramDepotUnits || ApplicationInformation.CanReprogramEdexUnits)
          ServerDataManager.GlobalInstance.GetUploadUnits(collection, (ServerDataManager.UploadType) 0);
        ServerClient.GlobalInstance.Go((Collection<UnitInformation>) null, collection);
      }));
    else
      WarningsPanel.GlobalInstance.Remove("serveruploadneeded");
  }

  public bool CanUndo => false;

  public void Undo()
  {
  }

  public bool CanCopy => true;

  public void Copy()
  {
    ClipboardData data = new ClipboardData();
    foreach (EcuStatusItem control in (ArrangedElementCollection) this.tableLayoutPanel.Controls)
      control.Copy(data);
    data.SetDataToClipboard();
  }

  public bool CanDelete => false;

  public bool CanPaste => false;

  public void Cut()
  {
  }

  public bool CanSelectAll => false;

  public void Delete()
  {
  }

  public bool CanCut => false;

  public void Paste()
  {
  }

  public void SelectAll()
  {
  }

  private void item_DoubleClick(object sender, EventArgs e)
  {
    if (this.ItemActivate == null)
      return;
    this.ItemActivate((object) this, e);
  }

  private void OnClearConnectionErrorsClick(object sender, EventArgs e)
  {
    if (this.selectedIdentifier == null || EcuStatusView.GetChannelForIdentifier(this.selectedIdentifier, SapiManager.GlobalInstance.ActiveChannels) != null)
    {
      this.CheckOrRemoveErrorItem(true);
    }
    else
    {
      EcuStatusItem itemByTag = this.FindItemByTag(this.selectedIdentifier);
      if (itemByTag == null)
        return;
      this.RemoveFromTableLayout(itemByTag);
    }
  }

  private bool CheckOrRemoveErrorItem(bool remove)
  {
    IEnumerable<EcuStatusItem> list = (IEnumerable<EcuStatusItem>) this.tableLayoutPanel.Controls.OfType<EcuStatusItem>().Where<EcuStatusItem>((Func<EcuStatusItem, bool>) (i => i.Icon == ConnectionIcon.Red)).ToList<EcuStatusItem>();
    if (remove)
    {
      foreach (EcuStatusItem ecuStatusItem in list)
        this.RemoveFromTableLayout(ecuStatusItem);
    }
    return list.Any<EcuStatusItem>();
  }

  private void OnCloseAllConnectionsClick(object sender, EventArgs e)
  {
    SapiManager.GlobalInstance.CloseAllConnections();
  }

  protected override void OnPaint(PaintEventArgs e)
  {
    Rectangle clientRectangle = this.headerPanel.ClientRectangle;
    ThemeProvider.GlobalInstance.ActiveTheme.DrawBackground(e.Graphics, clientRectangle, SystemColors.Window, false);
    e.Graphics.DrawLine(SystemPens.ControlDark, new Point(clientRectangle.Left, clientRectangle.Bottom - 1), new Point(clientRectangle.Right, clientRectangle.Bottom - 1));
  }

  private void SetTableLayoutRows()
  {
    bool flag1 = false;
    bool flag2 = false;
    int num = 0;
    IEnumerable<EcuStatusItem> orderedControls = (IEnumerable<EcuStatusItem>) this.tableLayoutPanel.Controls.OfType<EcuStatusItem>().OrderBy<EcuStatusItem, int>((Func<EcuStatusItem, int>) (esi => esi.Ecu.Priority)).ToList<EcuStatusItem>();
    foreach (EcuStatusItem ecuStatusItem in orderedControls)
    {
      if (!ecuStatusItem.Ecu.IsVirtual)
      {
        if (ecuStatusItem.Ecu.IsRollCall)
        {
          ecuStatusItem.Visible = !SapiManager.GlobalInstance.CoalesceFaultCodes || !ecuStatusItem.Ecu.RelatedEcus.Any<Ecu>((Func<Ecu, bool>) (e => orderedControls.Any<EcuStatusItem>((Func<EcuStatusItem, bool>) (esi => esi.Ecu == e && SapiExtensions.GetItem(this.activeChannels, esi.Ecu) != null))));
          if (flag2 && ecuStatusItem.Visible && !flag1)
          {
            this.tableLayoutPanel.SetRow((Control) this.separatorLine, num++);
            ((Control) this.separatorLine).Visible = flag1 = true;
          }
        }
        else
        {
          ecuStatusItem.Visible = !SapiManager.GlobalInstance.CoalesceFaultCodes || SapiExtensions.GetItem(this.activeChannels, ecuStatusItem.Ecu) != null || !ecuStatusItem.Ecu.RelatedEcus.Any<Ecu>((Func<Ecu, bool>) (e => orderedControls.Any<EcuStatusItem>((Func<EcuStatusItem, bool>) (esi => esi.Ecu == e))));
          if (ecuStatusItem.Visible)
            flag2 = true;
        }
      }
      else
        ecuStatusItem.Visible = false;
      this.tableLayoutPanel.SetRow((Control) ecuStatusItem, num++);
      ecuStatusItem.TabIndex = num;
    }
    if (flag1)
      return;
    ((Control) this.separatorLine).Visible = false;
  }

  private void AddToTableLayout(EcuStatusItem item)
  {
    this.tableLayoutPanel.SuspendLayout();
    this.tableLayoutPanel.Controls.Add((Control) item);
    ++this.tableLayoutPanel.RowCount;
    this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
    this.tableLayoutPanel.SetColumn((Control) item, 0);
    this.SetTableLayoutRows();
    this.tableLayoutPanel.ResumeLayout();
    item.MouseUp += new MouseEventHandler(this.control_MouseUp);
    item.DoubleClick += new EventHandler(this.item_DoubleClick);
  }

  private void RemoveFromTableLayout(EcuStatusItem item)
  {
    item.MouseUp -= new MouseEventHandler(this.control_MouseUp);
    item.DoubleClick -= new EventHandler(this.item_DoubleClick);
    this.tableLayoutPanel.SuspendLayout();
    int row = this.tableLayoutPanel.GetRow((Control) item);
    --this.tableLayoutPanel.RowCount;
    this.tableLayoutPanel.RowStyles.RemoveAt(row);
    this.tableLayoutPanel.Controls.Remove((Control) item);
    this.SetTableLayoutRows();
    this.tableLayoutPanel.ResumeLayout();
    if (item.Identifier == this.selectedIdentifier)
      this.selectedIdentifier = (string) null;
    this.Update();
  }

  private void ClearTableLayout()
  {
    this.tableLayoutPanel.Controls.Clear();
    this.tableLayoutPanel.RowStyles.Clear();
    this.tableLayoutPanel.RowCount = 0;
    this.selectedIdentifier = (string) null;
    ((Control) this.separatorLine).Visible = false;
    this.tableLayoutPanel.Controls.Add((Control) this.separatorLine, 0, 0);
    this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
  }

  private void control_MouseUp(object sender, MouseEventArgs e)
  {
    this.selectedIdentifier = sender is EcuStatusItem ecuStatusItem ? ecuStatusItem.Identifier : (string) null;
    if (e.Button != MouseButtons.Right)
      return;
    this.connectionContextMenu.Show(sender as Control, new Point(e.X, e.Y));
  }

  private void LogFile_LogFilePlaybackStateUpdateEvent(object sender, EventArgs e)
  {
    this.headerLabel.Text = (sender as LogFile).Playing ? Resources.EcuStatus_LoggedConnectionsHeading : Resources.EcuStatus_LoggedConnectionsPausedHeading;
  }
}
