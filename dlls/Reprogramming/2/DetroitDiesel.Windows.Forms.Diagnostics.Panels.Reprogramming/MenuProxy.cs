using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public sealed class MenuProxy : IMenuProxy, IDisposable
{
	private static MenuProxy globalInstance = new MenuProxy();

	private ToolStrip toolStrip;

	private ToolStripMenuItem menuTools;

	private ToolStripMenuItem menuViewStationLog;

	private IContainerApplication containerApplication;

	private IPlace place;

	public static MenuProxy GlobalInstance => globalInstance;

	public ReprogrammingView ReprogrammingView { get; set; }

	public IPlace Place
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

	public IContainerApplication ContainerApplication
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
		menuTools = new ToolStripMenuItem(Resources.CommandTools);
		menuViewStationLog = new ToolStripMenuItem(Resources.CommandViewStationLog);
		menuViewStationLog.Click += menuViewStationLog_Click;
		menuTools.DropDownItems.Add(new ToolStripSeparator());
		menuTools.DropDownItems.Add(menuViewStationLog);
		toolStrip = new ToolStrip();
		toolStrip.Items.Add(menuTools);
		menuTools.MergeAction = MergeAction.MatchOnly;
		menuViewStationLog.OwnerChanged += menuViewStationLog_OwnerChanged;
		ServerDataManager.GlobalInstance.DataUpdated += GlobalInstance_DataUpdated;
		SapiManager.GlobalInstance.ChannelIdentificationChanged += SapiManager_ChannelIdentificationChanged;
	}

	private void menuViewStationLog_OwnerChanged(object sender, EventArgs e)
	{
		if (menuViewStationLog.OwnerItem is ToolStripMenuItem toolStripMenuItem)
		{
			toolStripMenuItem.DropDownOpening += menuTools_DropDownOpening;
		}
	}

	private void menuViewStationLog_Click(object sender, EventArgs e)
	{
		CustomMessageBox.Show((IWin32Window)null, StationLogManager.Read(), Resources.CaptionStationLog, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, (CustomMessageBoxOptions)2);
	}

	private void menuTools_DropDownOpening(object sender, EventArgs e)
	{
		menuViewStationLog.Enabled = File.Exists(Directories.StationLogFile);
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
		if (disposing)
		{
			menuViewStationLog.Dispose();
			menuTools.Dispose();
			toolStrip.Dispose();
		}
	}

	private void GlobalInstance_DataUpdated(object sender, EventArgs e)
	{
		CheckAutomaticOperations();
	}

	private void SapiManager_ChannelIdentificationChanged(object sender, ChannelIdentificationChangedEventArgs e)
	{
		CheckAutomaticOperations();
	}

	internal void CheckAutomaticOperations()
	{
		if (WarningsPanel.GlobalInstance != null)
		{
			UnitInformation connectedUnitInformation = ProgrammingData.ConnectedUnitInformation;
			if (connectedUnitInformation != null && connectedUnitInformation.InAutomaticOperation && !ServerDataManager.GlobalInstance.Programming)
			{
				string empty = string.Empty;
				AutomaticOperation currentAutomaticOperation = connectedUnitInformation.CurrentAutomaticOperation;
				empty = ((currentAutomaticOperation.Channel != null) ? ((connectedUnitInformation.AutomaticOperations.Count <= 1) ? string.Format(CultureInfo.CurrentCulture, Resources.WarningPanelMessageFormat_TheAutomaticReprogrammingOperation0IsRequiredForTheConnectedVehicleClickHereToStartTheOperationForTheDevice1, currentAutomaticOperation.Reason, currentAutomaticOperation.Device) : string.Format(CultureInfo.CurrentCulture, Resources.WarningPanelMessageFormat_TheAutomaticReprogrammingOperation0IsRequiredForTheConnectedVehicleClickHereToStartTheOperationForTheDevice1Step2Of3, currentAutomaticOperation.Reason, currentAutomaticOperation.Device, currentAutomaticOperation.Index + 1, connectedUnitInformation.AutomaticOperations.Count)) : ((connectedUnitInformation.AutomaticOperations.Count <= 1) ? string.Format(CultureInfo.CurrentCulture, Resources.WarningPanelMessageFormat_TheAutomaticReprogrammingOperation0IsRequiredForTheConnectedVehicleConnectTheDevice1ToProceed, currentAutomaticOperation.Reason, currentAutomaticOperation.Device) : string.Format(CultureInfo.CurrentCulture, Resources.WarningPanelMessageFormat_TheAutomaticReprogrammingOperation0IsRequiredForTheConnectedVehicleConnectTheDevice1ToProceedStep2Of3, currentAutomaticOperation.Reason, currentAutomaticOperation.Device, currentAutomaticOperation.Index + 1, connectedUnitInformation.AutomaticOperations.Count)));
				WarningsPanel.GlobalInstance.Add("automaticoperation", MessageBoxIcon.Exclamation, string.Empty, empty, (EventHandler)AutomaticProgramDeviceClickHandler);
			}
			else
			{
				WarningsPanel.GlobalInstance.Remove("automaticoperation");
			}
		}
	}

	private void AutomaticProgramDeviceClickHandler(object sender, EventArgs e)
	{
		if (ReprogrammingView == null)
		{
			ContainerApplication.PreloadPlace(Place);
		}
		try
		{
			ProgrammingData programmingData = ProgrammingData.CreateFromAutomaticOperationForConnectedUnit();
			if (programmingData != null)
			{
				ContainerApplication.SelectPlace(Place);
				ReprogrammingView.SelectedUnit = programmingData.Unit;
				ReprogrammingView.ProgrammingData = Enumerable.Repeat(programmingData, 1);
				ReprogrammingView.AdvanceToLastPage();
			}
		}
		catch (DataException ex)
		{
			ControlHelpers.ShowMessageBox((Control)(object)ReprogrammingView, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
		}
	}
}
