// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.MenuProxy
// Assembly: Parameters, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 266306EF-5E5A-4E97-A95E-0BCBE6FD3F76
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Parameters.dll

using DetroitDiesel.Common;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Security;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;

public sealed class MenuProxy : IMenuProxy, IDisposable
{
  private static MenuProxy globalInstance = new MenuProxy();
  private ToolStrip toolStrip;
  private ToolStripMenuItem menuParameters;
  private ToolStripMenuItem menuParametersConnectOffline;
  private ToolStripMenuItem menuParametersImport;
  private ToolStripMenuItem menuParametersExport;
  private ToolStripMenuItem menuFileImport;
  private ToolStripMenuItem menuParametersHistoryImport;
  private ToolStripMenuItem menuExportParametersNoChannels;
  private ToolStripMenuItem menuAccumulators;
  private ToolStripMenuItem menuAccumulatorsImport;
  private ToolStripMenuItem menuAccumulatorsExport;
  private ToolStripMenuItem menuExportAccumulatorsNoChannels;
  private ToolStripMenuItem menuConfigurePasswords;
  private ToolStripMenuItem menuPasswordsNoChannels;
  private Dictionary<Channel, PasswordManager> passwordManagers;
  private ChannelBaseCollection activeChannels;
  private IContainerApplication containerApplication;
  private IPlace place;
  private ParameterView parameterView;
  private bool menuOpen;
  private const string AccumulatorImportExportDialogName = "Accumulator Services";

  public static MenuProxy GlobalInstance => MenuProxy.globalInstance;

  public ParameterView ParameterView
  {
    get => this.parameterView;
    set
    {
      this.parameterView = value;
      ApplyValidity();
      SapiManager.GlobalInstance.SapiResetCompleted += (EventHandler) ((sender, e) => ApplyValidity());

      static void ApplyValidity()
      {
        Sapi sapi = SapiManager.GlobalInstance.Sapi;
        int num1 = sapi.HardwareAccess > 8 | sapi.ReadAccess > 3 | sapi.WriteAccess > 3 ? 0 : 1;
        foreach (Ecu ecu in (ReadOnlyCollection<Ecu>) sapi.Ecus)
        {
          int num2 = num1;
          if (SapiExtensions.IsDataSourceEdex(ecu) && SapiManager.GlobalInstance.UseNameValuePairsToParameterize)
            num2 = 0;
          ecu.Properties["Validity"] = num2.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        }
      }
    }
  }

  private MenuProxy()
  {
    SapiManager.GlobalInstance.ActiveChannelsChanged += new EventHandler(this.GlobalInstance_ActiveChannelsChanged);
    this.passwordManagers = new Dictionary<Channel, PasswordManager>();
    this.menuParameters = new ToolStripMenuItem(Resources.Parameters);
    this.menuParametersImport = new ToolStripMenuItem(Resources.Import);
    this.menuFileImport = new ToolStripMenuItem(Resources.FileImport);
    this.menuFileImport.Click += new EventHandler(this.OnParametersImportClick);
    this.menuParametersHistoryImport = new ToolStripMenuItem(Resources.HistoryImport);
    this.menuParametersHistoryImport.Click += new EventHandler(this.OnParametersHistoryImportClick);
    this.menuParametersExport = new ToolStripMenuItem(Resources.Export);
    this.menuExportParametersNoChannels = new ToolStripMenuItem(Resources.NotAvailable);
    this.menuExportParametersNoChannels.Enabled = false;
    this.menuAccumulators = new ToolStripMenuItem(Resources.Accumulators);
    this.menuAccumulatorsImport = new ToolStripMenuItem(Resources.Import);
    this.menuAccumulatorsImport.Click += new EventHandler(this.OnAccumulatorsImportClick);
    this.menuAccumulatorsExport = new ToolStripMenuItem(Resources.Export);
    this.menuExportAccumulatorsNoChannels = new ToolStripMenuItem(Resources.NotAvailable);
    this.menuExportAccumulatorsNoChannels.Enabled = false;
    this.menuParametersImport.DropDownItems.AddRange((ToolStripItem[]) new ToolStripMenuItem[2]
    {
      this.menuParametersHistoryImport,
      this.menuFileImport
    });
    this.menuParametersExport.DropDownItems.Add((ToolStripItem) this.menuExportParametersNoChannels);
    this.menuAccumulatorsExport.DropDownItems.Add((ToolStripItem) this.menuExportAccumulatorsNoChannels);
    this.menuConfigurePasswords = new ToolStripMenuItem(Resources.ConfigurePasswords);
    this.menuPasswordsNoChannels = new ToolStripMenuItem(Resources.NotAvailable);
    this.menuPasswordsNoChannels.Enabled = false;
    this.menuConfigurePasswords.DropDownItems.Add((ToolStripItem) this.menuPasswordsNoChannels);
    if (ApplicationInformation.CanImportExportParameters)
    {
      this.menuParameters.DropDownItems.Add((ToolStripItem) this.menuParametersImport);
      this.menuParameters.DropDownItems.Add((ToolStripItem) this.menuParametersExport);
    }
    this.menuParameters.DropDownItems.Add((ToolStripItem) this.menuConfigurePasswords);
    this.menuParametersConnectOffline = new ToolStripMenuItem(Resources.ParameterView_ConnectOffline);
    this.menuParameters.MergeIndex = 4;
    this.menuParameters.MergeAction = MergeAction.Insert;
    this.menuParameters.Overflow = ToolStripItemOverflow.AsNeeded;
    this.toolStrip = new ToolStrip();
    this.toolStrip.Items.Add((ToolStripItem) this.menuParameters);
    this.UpdateActiveChannels();
    this.menuParameters.DropDownOpening += new EventHandler(this.menu_DropDownOpening);
    this.menuParameters.DropDownClosed += new EventHandler(this.menu_DropDownClosed);
  }

  ~MenuProxy() => this.Dispose(false);

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  private void Dispose(bool disposing)
  {
    if (!disposing)
      return;
    this.menuParameters.Dispose();
    this.menuParametersConnectOffline.Dispose();
    this.menuFileImport.Dispose();
    this.menuParametersHistoryImport.Dispose();
    this.menuParametersImport.Dispose();
    this.menuAccumulatorsImport.Dispose();
    this.menuParametersExport.Dispose();
    this.menuAccumulatorsExport.Dispose();
    this.menuAccumulators.Dispose();
    this.menuConfigurePasswords.Dispose();
    this.menuPasswordsNoChannels.Dispose();
    this.menuExportAccumulatorsNoChannels.Dispose();
    this.menuExportParametersNoChannels.Dispose();
    if (this.activeChannels != null)
    {
      this.activeChannels.ConnectCompleteEvent -= new ConnectCompleteEventHandler(this.activeChannels_ConnectCompleteEvent);
      this.activeChannels.DisconnectCompleteEvent -= new DisconnectCompleteEventHandler(this.activeChannels_DisconnectCompleteEvent);
      foreach (Channel activeChannel in this.activeChannels)
        activeChannel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.channel_CommunicationsStateUpdateEvent);
    }
    this.toolStrip.Dispose();
  }

  private void menu_DropDownOpening(object sender, EventArgs e)
  {
    this.menuOpen = true;
    this.UpdateMenus();
  }

  private void menu_DropDownClosed(object sender, EventArgs e) => this.menuOpen = false;

  private static void AdjustChannelMenuItems(
    ToolStripMenuItem parentItem,
    IList<Channel> currentChannels,
    EventHandler handler,
    ToolStripMenuItem noChannelItem)
  {
    foreach (ToolStripMenuItem toolStripMenuItem in parentItem.DropDownItems.OfType<ToolStripMenuItem>().Where<ToolStripMenuItem>((Func<ToolStripMenuItem, bool>) (i => i != noChannelItem && !currentChannels.Contains(i.Tag as Channel))).ToList<ToolStripMenuItem>())
      parentItem.DropDownItems.Remove((ToolStripItem) toolStripMenuItem);
    if (currentChannels.Count > 0 && parentItem.DropDownItems.Contains((ToolStripItem) noChannelItem))
      parentItem.DropDownItems.Remove((ToolStripItem) noChannelItem);
    foreach (Channel channel in currentChannels.Where<Channel>((Func<Channel, bool>) (c => !parentItem.DropDownItems.OfType<ToolStripMenuItem>().Any<ToolStripMenuItem>((Func<ToolStripMenuItem, bool>) (i => i.Tag == c)))).ToList<Channel>())
    {
      ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(channel.Ecu.DisplayName);
      toolStripMenuItem.Tag = (object) channel;
      toolStripMenuItem.Click += handler;
      bool flag = false;
      for (int index = 0; index < parentItem.DropDownItems.Count && !flag; ++index)
      {
        if ((parentItem.DropDownItems[index].Tag as Channel).Ecu.Priority > channel.Ecu.Priority)
        {
          parentItem.DropDownItems.Insert(index, (ToolStripItem) toolStripMenuItem);
          flag = true;
        }
      }
      if (!flag)
        parentItem.DropDownItems.Add((ToolStripItem) toolStripMenuItem);
    }
    if (currentChannels.Count != 0 || parentItem.DropDownItems.Contains((ToolStripItem) noChannelItem))
      return;
    parentItem.DropDownItems.Add((ToolStripItem) noChannelItem);
  }

  private static bool SupportsAccumulatorTransfer(Ecu ecu)
  {
    bool result = false;
    if (ecu.Properties.ContainsKey(nameof (SupportsAccumulatorTransfer)) && !bool.TryParse(ecu.Properties[nameof (SupportsAccumulatorTransfer)], out result))
      result = false;
    return result;
  }

  private void UpdateMenus()
  {
    if (this.menuParameters == null || this.menuParameters.IsDisposed || !this.menuOpen)
      return;
    Cursor.Current = Cursors.WaitCursor;
    bool flag = true;
    IEnumerable<Channel> list = (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels.ToList<Channel>();
    if (SapiManager.GlobalInstance.Online)
      flag = list.Any<Channel>((Func<Channel, bool>) (c =>
      {
        if (c.CommunicationsState == CommunicationsState.ReadParameters || c.CommunicationsState == CommunicationsState.WriteParameters)
          return true;
        return c.Online && !c.Parameters.HaveBeenReadFromEcu && c.Parameters.Count > 0;
      }));
    this.menuFileImport.Enabled = this.menuParametersHistoryImport.Enabled = !flag;
    MenuProxy.AdjustChannelMenuItems(this.menuParametersExport, (IList<Channel>) list.Where<Channel>((Func<Channel, bool>) (c => (c.CommunicationsState == CommunicationsState.Offline || c.Parameters.HaveBeenReadFromEcu || c.LogFile != null && c.Parameters.Any<Parameter>((Func<Parameter, bool>) (p => p.ParameterValues.Any<ParameterValue>()))) && c.Parameters.Count > 0)).ToList<Channel>(), new EventHandler(this.parameterExportChannel_Click), this.menuExportParametersNoChannels);
    MenuProxy.AdjustChannelMenuItems(this.menuConfigurePasswords, (IList<Channel>) list.Where<Channel>((Func<Channel, bool>) (c => c.CommunicationsState != CommunicationsState.Offline && this.GetPasswordManager(c) != null)).ToList<Channel>(), new EventHandler(this.passwordChannel_Click), this.menuPasswordsNoChannels);
    Cursor.Current = Cursors.Default;
  }

  private void MenuParametersConnectOffline_Click(object sender, EventArgs e)
  {
    if (this.parameterView == null)
      this.containerApplication.PreloadPlace(this.place);
    VariantSelect variantSelect = new VariantSelect((Ecu) null, (DiagnosisVariant) null);
    int num = (int) variantSelect.ShowDialog();
    if (variantSelect.DiagnosisVariant == null)
      return;
    SapiManager.GlobalInstance.Sapi.Channels.ConnectOffline(variantSelect.DiagnosisVariant);
    this.containerApplication.SelectPlace(this.place);
  }

  private void OnAccumulatorsImportClick(object sender, EventArgs e)
  {
    this.ImportParameter(ParameterType.Accumulator);
  }

  private void OnParametersImportClick(object sender, EventArgs e)
  {
    this.ImportParameter(ParameterType.Parameter);
  }

  private void ImportParameter(ParameterType type)
  {
    if (this.parameterView == null)
      this.containerApplication.PreloadPlace(this.place);
    string path = this.parameterView.ShowFileImportDialog(type);
    if (path == null)
      return;
    if (type == ParameterType.Accumulator)
    {
      TargetEcuDetails targetEcuDetails = ParameterCollection.GetTargetEcuDetails(path, ParameterFileFormat.ParFile);
      if (targetEcuDetails != null && targetEcuDetails.Ecu != null)
      {
        Ecu ecu = SapiManager.GlobalInstance.Sapi.Ecus[targetEcuDetails.Ecu];
        if (ecu != null && SapiExtensions.AccumulatorReadServices(ecu) != null)
        {
          Channel channel = SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault<Channel>((Func<Channel, bool>) (c => c.Ecu == ecu));
          if (channel != null)
          {
            ActionsMenuProxy.GlobalInstance.ShowDialog("Accumulator Services", (string) null, (object) new Tuple<Channel, string>(channel, path), true);
            return;
          }
          int num = (int) ControlHelpers.ShowMessageBox(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_AccumulatorServiceImportOnlineChannel, (object) ecu.Name), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
          return;
        }
      }
    }
    if (!this.parameterView.OnParametersImportClick(path))
      return;
    this.containerApplication.SelectPlace(this.place);
  }

  private void OnParametersHistoryImportClick(object sender, EventArgs e)
  {
    if (this.parameterView == null)
      this.containerApplication.PreloadPlace(this.place);
    if (!this.parameterView.OnParametersHistoryImportClick(sender, e))
      return;
    this.containerApplication.SelectPlace(this.place);
  }

  internal PasswordManager GetPasswordManager(Channel c)
  {
    PasswordManager passwordManager = (PasswordManager) null;
    this.passwordManagers.TryGetValue(c, out passwordManager);
    return passwordManager;
  }

  private void passwordChannel_Click(object sender, EventArgs e)
  {
    if (this.parameterView == null)
      this.containerApplication.PreloadPlace(this.place);
    PasswordManager passwordManager = this.GetPasswordManager((sender as ToolStripDropDownItem).Tag as Channel);
    if (passwordManager == null)
      return;
    int num = (int) new PasswordConfigureDialog(passwordManager).ShowDialog();
  }

  private void parameterExportChannel_Click(object sender, EventArgs e)
  {
    this.ExportParameter(sender, ParameterType.Parameter);
  }

  private void accumulatorExportChannel_Click(object sender, EventArgs e)
  {
    this.ExportParameter(sender, ParameterType.Accumulator);
  }

  private void ExportParameter(object sender, ParameterType type)
  {
    if (this.parameterView == null)
      this.containerApplication.PreloadPlace(this.place);
    Channel tag = (sender as ToolStripDropDownItem).Tag as Channel;
    if (type == ParameterType.Accumulator && SapiExtensions.AccumulatorReadServices(tag.Ecu) != null)
      ActionsMenuProxy.GlobalInstance.ShowDialog("Accumulator Services", (string) null, (object) tag, true);
    else
      this.parameterView.OnParametersExportClick(tag, type);
  }

  private void GlobalInstance_ActiveChannelsChanged(object sender, EventArgs e)
  {
    this.UpdateActiveChannels();
  }

  private void UpdateActiveChannels()
  {
    if (this.activeChannels != null)
    {
      this.activeChannels.ConnectCompleteEvent -= new ConnectCompleteEventHandler(this.activeChannels_ConnectCompleteEvent);
      this.activeChannels.DisconnectCompleteEvent -= new DisconnectCompleteEventHandler(this.activeChannels_DisconnectCompleteEvent);
      foreach (Channel activeChannel in this.activeChannels)
        this.RemoveChannel(activeChannel);
    }
    this.activeChannels = SapiManager.GlobalInstance.ActiveChannels;
    if (this.activeChannels != null)
    {
      this.activeChannels.ConnectCompleteEvent += new ConnectCompleteEventHandler(this.activeChannels_ConnectCompleteEvent);
      this.activeChannels.DisconnectCompleteEvent += new DisconnectCompleteEventHandler(this.activeChannels_DisconnectCompleteEvent);
      foreach (Channel activeChannel in this.activeChannels)
        this.AddChannel(activeChannel);
    }
    this.UpdateMenus();
  }

  IPlace IMenuProxy.Place
  {
    get => this.place;
    set => this.place = value;
  }

  IContainerApplication IMenuProxy.ContainerApplication
  {
    get => this.containerApplication;
    set => this.containerApplication = value;
  }

  ToolStrip IMenuProxy.Menu => this.toolStrip;

  private void AddChannel(Channel channel)
  {
    if (channel == null)
      return;
    channel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.channel_CommunicationsStateUpdateEvent);
    if (!PasswordManager.HasPasswords(channel))
      return;
    PasswordManager passwordManager = PasswordManager.Create(channel);
    if (passwordManager == null)
      return;
    if (passwordManager.Valid)
    {
      this.passwordManagers.Add(channel, passwordManager);
    }
    else
    {
      if (!channel.Online || channel.DiagnosisVariant.IsBase || channel.DiagnosisVariant.IsBoot || channel.Parameters.Count <= 0)
        return;
      WarningsPanel.GlobalInstance.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "PASSWORD{0}", (object) channel.Ecu.Name), MessageBoxIcon.Hand, Resources.WarningPasswordServicesMissingTitle, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatWarningPasswordServicesMissing, (object) channel.Ecu.Name, (object) channel.Ecu.DescriptionDataVersion), (EventHandler) null);
    }
  }

  private void RemoveChannel(Channel channel)
  {
    if (channel == null)
      return;
    channel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.channel_CommunicationsStateUpdateEvent);
    if (this.GetPasswordManager(channel) != null)
    {
      this.passwordManagers.Remove(channel);
    }
    else
    {
      if (!PasswordManager.HasPasswords(channel) || channel.DiagnosisVariant.IsBase)
        return;
      WarningsPanel.GlobalInstance.Remove(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "PASSWORD{0}", (object) channel.Ecu.Name));
    }
  }

  private void activeChannels_ConnectCompleteEvent(object sender, ResultEventArgs e)
  {
    if (e.Succeeded)
      this.AddChannel(sender as Channel);
    this.UpdateMenus();
  }

  private void activeChannels_DisconnectCompleteEvent(object sender, EventArgs e)
  {
    this.RemoveChannel(sender as Channel);
    this.UpdateMenus();
  }

  private void channel_CommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateMenus();
  }

  internal void Notify(int notificationCount) => this.place.Notify(notificationCount);
}
