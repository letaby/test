using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Security;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties;
using SapiLayer1;

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

	public static MenuProxy GlobalInstance => globalInstance;

	public ParameterView ParameterView
	{
		get
		{
			return parameterView;
		}
		set
		{
			parameterView = value;
			ApplyValidity();
			SapiManager.GlobalInstance.SapiResetCompleted += delegate
			{
				ApplyValidity();
			};
			static void ApplyValidity()
			{
				Sapi sapi = SapiManager.GlobalInstance.Sapi;
				int num = ((!((sapi.HardwareAccess > 8) | (sapi.ReadAccess > 3) | (sapi.WriteAccess > 3))) ? 1 : 0);
				foreach (Ecu ecu in sapi.Ecus)
				{
					int num2 = num;
					if (SapiExtensions.IsDataSourceEdex(ecu) && SapiManager.GlobalInstance.UseNameValuePairsToParameterize)
					{
						num2 = 0;
					}
					ecu.Properties["Validity"] = num2.ToString(CultureInfo.InvariantCulture);
				}
			}
		}
	}

	IPlace IMenuProxy.Place
	{
		get
		{
			return place;
		}
		set
		{
			place = value;
		}
	}

	IContainerApplication IMenuProxy.ContainerApplication
	{
		get
		{
			return containerApplication;
		}
		set
		{
			containerApplication = value;
		}
	}

	ToolStrip IMenuProxy.Menu => toolStrip;

	private MenuProxy()
	{
		SapiManager.GlobalInstance.ActiveChannelsChanged += GlobalInstance_ActiveChannelsChanged;
		passwordManagers = new Dictionary<Channel, PasswordManager>();
		menuParameters = new ToolStripMenuItem(Resources.Parameters);
		menuParametersImport = new ToolStripMenuItem(Resources.Import);
		menuFileImport = new ToolStripMenuItem(Resources.FileImport);
		menuFileImport.Click += OnParametersImportClick;
		menuParametersHistoryImport = new ToolStripMenuItem(Resources.HistoryImport);
		menuParametersHistoryImport.Click += OnParametersHistoryImportClick;
		menuParametersExport = new ToolStripMenuItem(Resources.Export);
		menuExportParametersNoChannels = new ToolStripMenuItem(Resources.NotAvailable);
		menuExportParametersNoChannels.Enabled = false;
		menuAccumulators = new ToolStripMenuItem(Resources.Accumulators);
		menuAccumulatorsImport = new ToolStripMenuItem(Resources.Import);
		menuAccumulatorsImport.Click += OnAccumulatorsImportClick;
		menuAccumulatorsExport = new ToolStripMenuItem(Resources.Export);
		menuExportAccumulatorsNoChannels = new ToolStripMenuItem(Resources.NotAvailable);
		menuExportAccumulatorsNoChannels.Enabled = false;
		menuParametersImport.DropDownItems.AddRange(new ToolStripMenuItem[2] { menuParametersHistoryImport, menuFileImport });
		menuParametersExport.DropDownItems.Add(menuExportParametersNoChannels);
		menuAccumulatorsExport.DropDownItems.Add(menuExportAccumulatorsNoChannels);
		menuConfigurePasswords = new ToolStripMenuItem(Resources.ConfigurePasswords);
		menuPasswordsNoChannels = new ToolStripMenuItem(Resources.NotAvailable);
		menuPasswordsNoChannels.Enabled = false;
		menuConfigurePasswords.DropDownItems.Add(menuPasswordsNoChannels);
		if (ApplicationInformation.CanImportExportParameters)
		{
			menuParameters.DropDownItems.Add(menuParametersImport);
			menuParameters.DropDownItems.Add(menuParametersExport);
		}
		menuParameters.DropDownItems.Add(menuConfigurePasswords);
		menuParametersConnectOffline = new ToolStripMenuItem(Resources.ParameterView_ConnectOffline);
		menuParameters.MergeIndex = 4;
		menuParameters.MergeAction = MergeAction.Insert;
		menuParameters.Overflow = ToolStripItemOverflow.AsNeeded;
		toolStrip = new ToolStrip();
		toolStrip.Items.Add(menuParameters);
		UpdateActiveChannels();
		menuParameters.DropDownOpening += menu_DropDownOpening;
		menuParameters.DropDownClosed += menu_DropDownClosed;
	}

	~MenuProxy()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (!disposing)
		{
			return;
		}
		menuParameters.Dispose();
		menuParametersConnectOffline.Dispose();
		menuFileImport.Dispose();
		menuParametersHistoryImport.Dispose();
		menuParametersImport.Dispose();
		menuAccumulatorsImport.Dispose();
		menuParametersExport.Dispose();
		menuAccumulatorsExport.Dispose();
		menuAccumulators.Dispose();
		menuConfigurePasswords.Dispose();
		menuPasswordsNoChannels.Dispose();
		menuExportAccumulatorsNoChannels.Dispose();
		menuExportParametersNoChannels.Dispose();
		if (activeChannels != null)
		{
			activeChannels.ConnectCompleteEvent -= activeChannels_ConnectCompleteEvent;
			activeChannels.DisconnectCompleteEvent -= activeChannels_DisconnectCompleteEvent;
			foreach (Channel activeChannel in activeChannels)
			{
				activeChannel.CommunicationsStateUpdateEvent -= channel_CommunicationsStateUpdateEvent;
			}
		}
		toolStrip.Dispose();
	}

	private void menu_DropDownOpening(object sender, EventArgs e)
	{
		menuOpen = true;
		UpdateMenus();
	}

	private void menu_DropDownClosed(object sender, EventArgs e)
	{
		menuOpen = false;
	}

	private static void AdjustChannelMenuItems(ToolStripMenuItem parentItem, IList<Channel> currentChannels, EventHandler handler, ToolStripMenuItem noChannelItem)
	{
		foreach (ToolStripMenuItem item in (from i in parentItem.DropDownItems.OfType<ToolStripMenuItem>()
			where i != noChannelItem && !currentChannels.Contains(i.Tag as Channel)
			select i).ToList())
		{
			parentItem.DropDownItems.Remove(item);
		}
		if (currentChannels.Count > 0 && parentItem.DropDownItems.Contains(noChannelItem))
		{
			parentItem.DropDownItems.Remove(noChannelItem);
		}
		foreach (Channel item2 in currentChannels.Where((Channel c) => !parentItem.DropDownItems.OfType<ToolStripMenuItem>().Any((ToolStripMenuItem i) => i.Tag == c)).ToList())
		{
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(item2.Ecu.DisplayName);
			toolStripMenuItem.Tag = item2;
			toolStripMenuItem.Click += handler;
			bool flag = false;
			for (int num = 0; num < parentItem.DropDownItems.Count; num++)
			{
				if (flag)
				{
					break;
				}
				Channel channel = parentItem.DropDownItems[num].Tag as Channel;
				if (channel.Ecu.Priority > item2.Ecu.Priority)
				{
					parentItem.DropDownItems.Insert(num, toolStripMenuItem);
					flag = true;
				}
			}
			if (!flag)
			{
				parentItem.DropDownItems.Add(toolStripMenuItem);
			}
		}
		if (currentChannels.Count == 0 && !parentItem.DropDownItems.Contains(noChannelItem))
		{
			parentItem.DropDownItems.Add(noChannelItem);
		}
	}

	private static bool SupportsAccumulatorTransfer(Ecu ecu)
	{
		bool result = false;
		if (ecu.Properties.ContainsKey("SupportsAccumulatorTransfer") && !bool.TryParse(ecu.Properties["SupportsAccumulatorTransfer"], out result))
		{
			result = false;
		}
		return result;
	}

	private void UpdateMenus()
	{
		if (menuParameters == null || menuParameters.IsDisposed || !menuOpen)
		{
			return;
		}
		Cursor.Current = Cursors.WaitCursor;
		bool flag = true;
		IEnumerable<Channel> source = SapiManager.GlobalInstance.ActiveChannels.ToList();
		if (SapiManager.GlobalInstance.Online)
		{
			flag = source.Any((Channel c) => c.CommunicationsState == CommunicationsState.ReadParameters || c.CommunicationsState == CommunicationsState.WriteParameters || (c.Online && !c.Parameters.HaveBeenReadFromEcu && c.Parameters.Count > 0));
		}
		ToolStripMenuItem toolStripMenuItem = menuFileImport;
		bool enabled = (menuParametersHistoryImport.Enabled = !flag);
		toolStripMenuItem.Enabled = enabled;
		List<Channel> currentChannels = source.Where((Channel c) => (c.CommunicationsState == CommunicationsState.Offline || c.Parameters.HaveBeenReadFromEcu || (c.LogFile != null && c.Parameters.Any((Parameter p) => p.ParameterValues.Any()))) && c.Parameters.Count > 0).ToList();
		AdjustChannelMenuItems(menuParametersExport, currentChannels, parameterExportChannel_Click, menuExportParametersNoChannels);
		List<Channel> currentChannels2 = source.Where((Channel c) => c.CommunicationsState != CommunicationsState.Offline && GetPasswordManager(c) != null).ToList();
		AdjustChannelMenuItems(menuConfigurePasswords, currentChannels2, passwordChannel_Click, menuPasswordsNoChannels);
		Cursor.Current = Cursors.Default;
	}

	private void MenuParametersConnectOffline_Click(object sender, EventArgs e)
	{
		if (parameterView == null)
		{
			containerApplication.PreloadPlace(place);
		}
		VariantSelect variantSelect = new VariantSelect(null, null);
		variantSelect.ShowDialog();
		if (variantSelect.DiagnosisVariant != null)
		{
			SapiManager.GlobalInstance.Sapi.Channels.ConnectOffline(variantSelect.DiagnosisVariant);
			containerApplication.SelectPlace(place);
		}
	}

	private void OnAccumulatorsImportClick(object sender, EventArgs e)
	{
		ImportParameter(ParameterType.Accumulator);
	}

	private void OnParametersImportClick(object sender, EventArgs e)
	{
		ImportParameter(ParameterType.Parameter);
	}

	private void ImportParameter(ParameterType type)
	{
		if (parameterView == null)
		{
			containerApplication.PreloadPlace(place);
		}
		string text = parameterView.ShowFileImportDialog(type);
		if (text == null)
		{
			return;
		}
		if (type == ParameterType.Accumulator)
		{
			TargetEcuDetails targetEcuDetails = ParameterCollection.GetTargetEcuDetails(text, ParameterFileFormat.ParFile);
			if (targetEcuDetails != null && targetEcuDetails.Ecu != null)
			{
				Ecu ecu = SapiManager.GlobalInstance.Sapi.Ecus[targetEcuDetails.Ecu];
				if (ecu != null && SapiExtensions.AccumulatorReadServices(ecu) != null)
				{
					Channel channel = SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault((Channel c) => c.Ecu == ecu);
					if (channel != null)
					{
						ActionsMenuProxy.GlobalInstance.ShowDialog("Accumulator Services", (string)null, (object)new Tuple<Channel, string>(channel, text), true);
					}
					else
					{
						ControlHelpers.ShowMessageBox(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_AccumulatorServiceImportOnlineChannel, ecu.Name), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					}
					return;
				}
			}
		}
		if (parameterView.OnParametersImportClick(text))
		{
			containerApplication.SelectPlace(place);
		}
	}

	private void OnParametersHistoryImportClick(object sender, EventArgs e)
	{
		if (parameterView == null)
		{
			containerApplication.PreloadPlace(place);
		}
		if (parameterView.OnParametersHistoryImportClick(sender, e))
		{
			containerApplication.SelectPlace(place);
		}
	}

	internal PasswordManager GetPasswordManager(Channel c)
	{
		PasswordManager value = null;
		passwordManagers.TryGetValue(c, out value);
		return value;
	}

	private void passwordChannel_Click(object sender, EventArgs e)
	{
		if (parameterView == null)
		{
			containerApplication.PreloadPlace(place);
		}
		Channel c = (sender as ToolStripDropDownItem).Tag as Channel;
		PasswordManager passwordManager = GetPasswordManager(c);
		if (passwordManager != null)
		{
			PasswordConfigureDialog passwordConfigureDialog = new PasswordConfigureDialog(passwordManager);
			passwordConfigureDialog.ShowDialog();
		}
	}

	private void parameterExportChannel_Click(object sender, EventArgs e)
	{
		ExportParameter(sender, ParameterType.Parameter);
	}

	private void accumulatorExportChannel_Click(object sender, EventArgs e)
	{
		ExportParameter(sender, ParameterType.Accumulator);
	}

	private void ExportParameter(object sender, ParameterType type)
	{
		if (parameterView == null)
		{
			containerApplication.PreloadPlace(place);
		}
		Channel channel = (sender as ToolStripDropDownItem).Tag as Channel;
		if (type == ParameterType.Accumulator && SapiExtensions.AccumulatorReadServices(channel.Ecu) != null)
		{
			ActionsMenuProxy.GlobalInstance.ShowDialog("Accumulator Services", (string)null, (object)channel, true);
		}
		else
		{
			parameterView.OnParametersExportClick(channel, type);
		}
	}

	private void GlobalInstance_ActiveChannelsChanged(object sender, EventArgs e)
	{
		UpdateActiveChannels();
	}

	private void UpdateActiveChannels()
	{
		if (activeChannels != null)
		{
			activeChannels.ConnectCompleteEvent -= activeChannels_ConnectCompleteEvent;
			activeChannels.DisconnectCompleteEvent -= activeChannels_DisconnectCompleteEvent;
			foreach (Channel activeChannel in activeChannels)
			{
				RemoveChannel(activeChannel);
			}
		}
		activeChannels = SapiManager.GlobalInstance.ActiveChannels;
		if (activeChannels != null)
		{
			activeChannels.ConnectCompleteEvent += activeChannels_ConnectCompleteEvent;
			activeChannels.DisconnectCompleteEvent += activeChannels_DisconnectCompleteEvent;
			foreach (Channel activeChannel2 in activeChannels)
			{
				AddChannel(activeChannel2);
			}
		}
		UpdateMenus();
	}

	private void AddChannel(Channel channel)
	{
		if (channel == null)
		{
			return;
		}
		channel.CommunicationsStateUpdateEvent += channel_CommunicationsStateUpdateEvent;
		if (!PasswordManager.HasPasswords(channel))
		{
			return;
		}
		PasswordManager val = PasswordManager.Create(channel);
		if (val != null)
		{
			if (val.Valid)
			{
				passwordManagers.Add(channel, val);
			}
			else if (channel.Online && !channel.DiagnosisVariant.IsBase && !channel.DiagnosisVariant.IsBoot && channel.Parameters.Count > 0)
			{
				WarningsPanel.GlobalInstance.Add(string.Format(CultureInfo.InvariantCulture, "PASSWORD{0}", channel.Ecu.Name), MessageBoxIcon.Hand, Resources.WarningPasswordServicesMissingTitle, string.Format(CultureInfo.CurrentCulture, Resources.FormatWarningPasswordServicesMissing, channel.Ecu.Name, channel.Ecu.DescriptionDataVersion), (EventHandler)null);
			}
		}
	}

	private void RemoveChannel(Channel channel)
	{
		if (channel != null)
		{
			channel.CommunicationsStateUpdateEvent -= channel_CommunicationsStateUpdateEvent;
			if (GetPasswordManager(channel) != null)
			{
				passwordManagers.Remove(channel);
			}
			else if (PasswordManager.HasPasswords(channel) && !channel.DiagnosisVariant.IsBase)
			{
				WarningsPanel.GlobalInstance.Remove(string.Format(CultureInfo.InvariantCulture, "PASSWORD{0}", channel.Ecu.Name));
			}
		}
	}

	private void activeChannels_ConnectCompleteEvent(object sender, ResultEventArgs e)
	{
		if (e.Succeeded)
		{
			Channel channel = sender as Channel;
			AddChannel(channel);
		}
		UpdateMenus();
	}

	private void activeChannels_DisconnectCompleteEvent(object sender, EventArgs e)
	{
		Channel channel = sender as Channel;
		RemoveChannel(channel);
		UpdateMenus();
	}

	private void channel_CommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
	{
		UpdateMenus();
	}

	internal void Notify(int notificationCount)
	{
		place.Notify(notificationCount);
	}
}
