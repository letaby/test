using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Common.Status;
using DetroitDiesel.CrashHandling;
using DetroitDiesel.DataHub;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Net;
using DetroitDiesel.Reflection;
using DetroitDiesel.Settings;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Troubleshooting;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal sealed class MainForm : Form, IContainerApplication, ISearchable, IRefreshable, IProgressBar, ISupportExpandCollapseAll
{
	private delegate void VoidArgsDelegate();

	private enum HelpAction
	{
		BestAvailable,
		Main,
		Context
	}

	private const int ServerStatusMessageDisplayTime = 5000;

	private Timer serverProgressTimer = new Timer();

	private string overallStatusText;

	private string componentStatusText;

	private IContainer components;

	private TabbedView mainTabView;

	private ToolStripMenuItem menuView;

	private ToolStripMenuItem menuConnect;

	private ToolStripMenuItem menuClose;

	private ToolStripMenuItem menuLogPlayPause;

	private ToolStripMenuItem menuLogStop;

	private ToolStripMenuItem menuLogSeekStart;

	private ToolStripMenuItem menuLogSeekEnd;

	private ToolStripMenuItem menuLogSpeedX1;

	private ToolStripMenuItem menuLogSpeedX2;

	private ToolStripMenuItem menuLogSpeedX4;

	private ToolStripMenuItem menuLogSpeedX8;

	private ToolStripMenuItem menuLogSpeedX1B;

	private ToolStripMenuItem menuLogSpeedX2B;

	private ToolStripMenuItem menuLogSpeedX4B;

	private ToolStripMenuItem menuLogSpeedX8B;

	private StatusStrip statusStrip;

	private ToolStripStatusLabel statusStripPanelLogTime;

	private ToolStripStatusLabel statusStripPanelServerConnection;

	private ToolStripButton toolBarButtonFastForward;

	private ToolStripButton toolBarButtonOpen;

	private ToolStripButton toolBarButtonStop;

	private ToolStripButton toolBarButtonSeekEnd;

	private ToolStripButton toolBarButtonSeekStart;

	private ToolStripButton toolBarButtonPlayPause;

	private ToolStripButton toolBarButtonRewind;

	private ToolStripTrackBar toolBarTrackBarSeek;

	private ToolStripSplitButton toolBarButtonUserEvent;

	private ToolStripButton toolBarButtonLogUpload;

	private ToolStripMenuItem toolBarMenuItemLabel;

	private ToolStripMenuItem toolBarMenuItemEditableLabel;

	private ImageList imageListToolStrip;

	private ToolStripMenuItem menuLogLabel;

	private ToolStripMenuItem menuLogEditableLabel;

	private ToolStripMenuItem menuLogUpload;

	private ToolStripMenuItem logFileInfo;

	private ToolStripMenuItem menuLogSeekLabel;

	private ToolStripMenuItem menuNextView;

	private ToolStripMenuItem menuPreviousView;

	private FindBar findBar;

	private ToolStripButton toolBarButtonCut;

	private ToolStripButton toolBarButtonCopy;

	private ToolStripButton toolBarButtonPaste;

	private ToolStripButton toolBarButtonUndo;

	private ToolStripMenuItem menuUndo;

	private ToolStripMenuItem menuCut;

	private ToolStripMenuItem menuCopy;

	private ToolStripMenuItem menuPaste;

	private ToolStripMenuItem menuDelete;

	private ToolStripMenuItem menuSelectAll;

	private ToolStripSeparator menuViewSplit1;

	private ToolStripButton toolStripButtonRetryAutoConnect;

	private ToolStripMenuItem menuPrint;

	private ToolStripMenuItem menuPrintPreview;

	private ToolStripButton toolStripButtonPrint;

	private ToolStripButton menuFullscreen;

	private ToolStrip toolStrip;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripSeparator toolStripSeparator2;

	private ToolStripSeparator toolStripSeparator3;

	private ToolStripSeparator toolStripSeparator4;

	private MenuStrip menuStrip1;

	private ToolStrip fullScreenStrip;

	private Panel toolStripContainer;

	private Panel toolStripContainer2;

	private ConnectionStatusButton connectionsButton;

	private ToolStripSeparator statusSeparator1;

	private ToolStripButton toolStripButtonSeekTime;

	private ToolStripSeparator infoLogSplitter;

	private ToolStripMenuItem menuTroubleshootingGuides;

	private ToolStripMenuItem menuItemHelp;

	private ToolStripMenuItem contextHelpToolStripMenuItem;

	private ToolStripMenuItem menuUpdate;

	private ToolStripSeparator menuToolsSeperator1;

	private ToolStripSeparator menuViewSplit2;

	private ToolStripMenuItem refreshToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator7;

	private ToolStripButton toolBarButtonRefresh;

	private ToolStripMenuItem menuItemReferences;

	private ToolStripMenuItem menuItemDtnaConnect;

	private ToolStripMenuItem menuItemCatalog;

	private ToolStripSeparator toolStripSeparator8;

	private ToolStripMenuItem menuItemAandIManual;

	private ToolStripMenuItem menuItemDDECVIAI;

	private ToolStripMenuItem menuItemDDEC10AI;

	private ToolStripMenuItem menuItemFcccEngines;

	private ToolStripMenuItem menuItemFcccEngineSupport;

	private ToolStripMenuItem menuItemFcccOasis;

	private ToolStripMenuItem menuItemFcccRVChassis;

	private ToolStripMenuItem menuItemFcccS2B2Chassis;

	private ToolStripMenuItem menuItemFcccEconic;

	private ToolStripMenuItem menuItemDDEC13AI;

	private ToolStripMenuItem menuItemEuro4AI;

	private ToolStripMenuItem menuItemDetroitTransmissionsAI;

	private ToolStripProgressBar statusStripProgressBarServerConnection;

	private ToolStripStatusLabel statusStripPanelEcuConnection;

	private ToolStrip navigationToolStrip;

	private ToolStripSplitButton toolStripButtonBack;

	private ToolStripSplitButton toolStripButtonForward;

	private ToolStripSeparator toolStripSeparator9;

	private ToolStripMenuItem gotoToolStripMenuItem;

	private ToolStripMenuItem backToolStripMenuItem;

	private ToolStripMenuItem forwardToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator10;

	private ToolStripSeparator toolStripSeparator11;

	private ToolStripMenuItem menuItemReferenceGuide;

	private ToolStripMenuItem menuItemDDEC10RefrenceGuide;

	private ToolStripMenuItem menuItemDDEC13RefrenceGuide;

	private ToolStripMenuItem menuFeedBack;

	private ToolStripSeparator toolStripSeparator12;

	private ToolStripMenuItem menuItemDataMiningReports;

	private ToolStripMenuItem switchDataMiningProcessTagToolStripMenuItem;

	private ToolStripButton toolStripButtonDataMiningTag;

	private ToolStripMenuItem menuRecentFiles;

	private ToolStripMenuItem menuItemVehicleInformation;

	private ToolStripButton toolStripButtonFilter;

	private ToolStripSeparator toolStripSeparator16;

	private ToolStripSeparator toolStripSeparator14;

	private ToolStripMenuItem filterToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator6;

	private ToolStripButton toolStripButtonExpandAll;

	private ToolStripButton toolStripButtonCollapseAll;

	private ToolStripMenuItem menuItemDetroitTransmissionsRG;

	private ToolStripMenuItem menuFile;

	private ToolStripButton toolStripButtonUnitsSystem;

	private ToolStripSeparator toolStripSeparator17;

	private ToolStripButton toolStripButtonBusMonitor;

	private ToolStripMenuItem busMonitoringToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator15;

	private ToolStripButton toolStripButtonMute;

	private ToolStripMenuItem toolStripMenuItemMute;

	private ToolStripMenuItem menuPrintFaults;

	private ToolStripMenuItem menuItemZenZefiT;

	private RecentFiles recentFiles;

	private const int MaximumRecentLogs = 10;

	private FormBorderStyle regularFormBorderStyle;

	private FormWindowState regularFormWindowState;

	private Size lastKnownGoodSize;

	private Point lastKnownGoodLocation;

	private SapiManager sapiManager;

	private LogFile logFile;

	private EditSupportHelper editSupport = new EditSupportHelper();

	private SplashScreen splashScreen;

	private string logFileToLoad;

	private string title;

	private bool dialoginuse;

	private int lastUpdate;

	private uint labelNumber;

	private Dictionary<FilterTypes, bool> activeFilterList = new Dictionary<FilterTypes, bool>();

	int IProgressBar.Maximum
	{
		get
		{
			if (statusStripProgressBarServerConnection == null)
			{
				return 0;
			}
			return statusStripProgressBarServerConnection.Maximum;
		}
		set
		{
			if (statusStripProgressBarServerConnection != null)
			{
				if (value > 0)
				{
					statusStripProgressBarServerConnection.Maximum = value;
				}
				statusStripProgressBarServerConnection.Visible = value > 0;
			}
		}
	}

	int IProgressBar.Value
	{
		get
		{
			if (statusStripProgressBarServerConnection == null)
			{
				return 0;
			}
			return statusStripProgressBarServerConnection.Value;
		}
		set
		{
			if (statusStripProgressBarServerConnection != null && value <= statusStripProgressBarServerConnection.Maximum)
			{
				statusStripProgressBarServerConnection.Value = value;
				statusStrip.Update();
			}
		}
	}

	public string OverallStatusText
	{
		get
		{
			return overallStatusText;
		}
		set
		{
			overallStatusText = value;
			UpdateProgressFromStatus();
		}
	}

	public string ComponentStatusText
	{
		get
		{
			return componentStatusText;
		}
		set
		{
			componentStatusText = value;
			UpdateProgressFromStatus();
		}
	}

	public Control MarshalControl => statusStripPanelServerConnection.GetCurrentParent();

	ITroubleshooting IContainerApplication.Troubleshooting => TroubleshootingMenuProxy.GlobalInstance.Troubleshooting;

	ToolStrip IContainerApplication.Menu => menuStrip1;

	private bool CanPrint
	{
		get
		{
			if (!PrintHelper.Busy && mainTabView != null)
			{
				return mainTabView.CanProvideHtml;
			}
			return false;
		}
	}

	private bool CanLabel
	{
		get
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Invalid comparison between Unknown and I4
			bool result = false;
			if (sapiManager != null && sapiManager.Sapi != null)
			{
				foreach (Channel item in (ChannelBaseCollection)sapiManager.Sapi.Channels)
				{
					if (item.Online && (int)item.CommunicationsState != 4)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}
	}

	public bool CanSearch => mainTabView.CanSearch;

	public bool CanRefreshView => mainTabView.CanRefreshView;

	public bool CanExpandAllItems => mainTabView.CanExpandAllItems;

	public bool CanCollapseAllItems => mainTabView.CanCollapseAllItems;

	public void AllDone(bool success, bool canceled, string message)
	{
		componentStatusText = string.Empty;
		overallStatusText = message;
		if (statusStripProgressBarServerConnection != null)
		{
			statusStripProgressBarServerConnection.Visible = false;
		}
		UpdateProgressFromStatus();
		serverProgressTimer.Tick += serverProgressTimer_Tick;
		serverProgressTimer.Interval = 5000;
		serverProgressTimer.Enabled = true;
	}

	private void serverProgressTimer_Tick(object sender, EventArgs e)
	{
		serverProgressTimer.Tick -= serverProgressTimer_Tick;
		serverProgressTimer.Enabled = false;
		statusStripPanelServerConnection.Text = string.Empty;
	}

	private void UpdateProgressFromStatus()
	{
		if (!string.IsNullOrEmpty(componentStatusText))
		{
			statusStripPanelServerConnection.Text = string.Format(CultureInfo.CurrentCulture, Resources.FormatConnectProgressWithCompStatus, NetworkSettings.GlobalInstance.Server, overallStatusText, componentStatusText);
		}
		else
		{
			statusStripPanelServerConnection.Text = string.Format(CultureInfo.CurrentCulture, Resources.FormatConnectProgressWithoutCompStatus, NetworkSettings.GlobalInstance.Server, overallStatusText);
		}
		statusStrip.Update();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			while (toolStrip.Items.Count > 0)
			{
				toolStrip.Items.RemoveAt(0);
			}
			while (fullScreenStrip.Items.Count > 0)
			{
				fullScreenStrip.Items.RemoveAt(0);
			}
			while (navigationToolStrip.Items.Count > 0)
			{
				navigationToolStrip.Items.RemoveAt(0);
			}
			PrintHelper.DisposeInstance();
			if (components != null)
			{
				components.Dispose();
			}
		}
		base.Dispose(disposing);
		if (disposing)
		{
			sapiManager.SaveSettings();
			sapiManager.Dispose();
		}
	}

	private void InitializeComponent()
	{
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Expected O, but got Unknown
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Expected O, but got Unknown
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Container.MainForm));
		this.menuUndo = new System.Windows.Forms.ToolStripMenuItem();
		this.menuCut = new System.Windows.Forms.ToolStripMenuItem();
		this.menuCopy = new System.Windows.Forms.ToolStripMenuItem();
		this.menuPaste = new System.Windows.Forms.ToolStripMenuItem();
		this.menuDelete = new System.Windows.Forms.ToolStripMenuItem();
		this.menuSelectAll = new System.Windows.Forms.ToolStripMenuItem();
		this.menuUpdate = new System.Windows.Forms.ToolStripMenuItem();
		this.menuToolsSeperator1 = new System.Windows.Forms.ToolStripSeparator();
		this.menuLogPlayPause = new System.Windows.Forms.ToolStripMenuItem();
		this.menuLogStop = new System.Windows.Forms.ToolStripMenuItem();
		this.menuLogSpeedX1 = new System.Windows.Forms.ToolStripMenuItem();
		this.menuLogSpeedX2 = new System.Windows.Forms.ToolStripMenuItem();
		this.menuLogSpeedX4 = new System.Windows.Forms.ToolStripMenuItem();
		this.menuLogSpeedX8 = new System.Windows.Forms.ToolStripMenuItem();
		this.menuLogSpeedX1B = new System.Windows.Forms.ToolStripMenuItem();
		this.menuLogSpeedX2B = new System.Windows.Forms.ToolStripMenuItem();
		this.menuLogSpeedX4B = new System.Windows.Forms.ToolStripMenuItem();
		this.menuLogSpeedX8B = new System.Windows.Forms.ToolStripMenuItem();
		this.menuLogSeekStart = new System.Windows.Forms.ToolStripMenuItem();
		this.menuLogSeekEnd = new System.Windows.Forms.ToolStripMenuItem();
		this.menuLogSeekLabel = new System.Windows.Forms.ToolStripMenuItem();
		this.menuLogLabel = new System.Windows.Forms.ToolStripMenuItem();
		this.menuLogEditableLabel = new System.Windows.Forms.ToolStripMenuItem();
		this.menuLogUpload = new System.Windows.Forms.ToolStripMenuItem();
		this.switchDataMiningProcessTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.infoLogSplitter = new System.Windows.Forms.ToolStripSeparator();
		this.logFileInfo = new System.Windows.Forms.ToolStripMenuItem();
		this.menuTroubleshootingGuides = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
		this.contextHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemReferences = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemDtnaConnect = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemCatalog = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemVehicleInformation = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemDataMiningReports = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemAandIManual = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemDDECVIAI = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemDDEC10AI = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemDDEC13AI = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemEuro4AI = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemDetroitTransmissionsAI = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemFcccEngines = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemFcccEngineSupport = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemFcccOasis = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemFcccRVChassis = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemFcccS2B2Chassis = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemFcccEconic = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemReferenceGuide = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemDDEC10RefrenceGuide = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemDDEC13RefrenceGuide = new System.Windows.Forms.ToolStripMenuItem();
		this.menuItemDetroitTransmissionsRG = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
		this.menuFeedBack = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
		this.menuConnect = new System.Windows.Forms.ToolStripMenuItem();
		this.menuClose = new System.Windows.Forms.ToolStripMenuItem();
		this.menuPrint = new System.Windows.Forms.ToolStripMenuItem();
		this.menuPrintPreview = new System.Windows.Forms.ToolStripMenuItem();
		this.menuRecentFiles = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStrip = new System.Windows.Forms.ToolStrip();
		this.imageListToolStrip = new System.Windows.Forms.ImageList(this.components);
		this.toolStripButtonRetryAutoConnect = new System.Windows.Forms.ToolStripButton();
		this.toolStripButtonBusMonitor = new System.Windows.Forms.ToolStripButton();
		this.toolStripButtonMute = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.toolBarButtonOpen = new System.Windows.Forms.ToolStripButton();
		this.toolBarButtonPlayPause = new System.Windows.Forms.ToolStripButton();
		this.toolBarButtonStop = new System.Windows.Forms.ToolStripButton();
		this.toolBarButtonSeekStart = new System.Windows.Forms.ToolStripButton();
		this.toolBarButtonRewind = new System.Windows.Forms.ToolStripButton();
		this.toolBarTrackBarSeek = new ToolStripTrackBar();
		this.toolBarButtonFastForward = new System.Windows.Forms.ToolStripButton();
		this.toolBarButtonSeekEnd = new System.Windows.Forms.ToolStripButton();
		this.toolStripButtonSeekTime = new System.Windows.Forms.ToolStripButton();
		this.toolBarButtonUserEvent = new System.Windows.Forms.ToolStripSplitButton();
		this.toolBarMenuItemLabel = new System.Windows.Forms.ToolStripMenuItem();
		this.toolBarMenuItemEditableLabel = new System.Windows.Forms.ToolStripMenuItem();
		this.toolBarButtonLogUpload = new System.Windows.Forms.ToolStripButton();
		this.toolStripButtonDataMiningTag = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		this.toolStripButtonPrint = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
		this.toolBarButtonCut = new System.Windows.Forms.ToolStripButton();
		this.toolBarButtonCopy = new System.Windows.Forms.ToolStripButton();
		this.toolBarButtonPaste = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
		this.toolBarButtonUndo = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
		this.toolStripButtonFilter = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
		this.toolBarButtonRefresh = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
		this.toolStripButtonUnitsSystem = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
		this.toolStripButtonExpandAll = new System.Windows.Forms.ToolStripButton();
		this.toolStripButtonCollapseAll = new System.Windows.Forms.ToolStripButton();
		this.menuStrip1 = new System.Windows.Forms.MenuStrip();
		this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
		this.menuPrintFaults = new System.Windows.Forms.ToolStripMenuItem();
		this.menuView = new System.Windows.Forms.ToolStripMenuItem();
		this.menuViewSplit2 = new System.Windows.Forms.ToolStripSeparator();
		this.menuNextView = new System.Windows.Forms.ToolStripMenuItem();
		this.menuPreviousView = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
		this.gotoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.backToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.forwardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
		this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
		this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
		this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.menuViewSplit1 = new System.Windows.Forms.ToolStripSeparator();
		this.busMonitoringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItemMute = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
		this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.fullScreenStrip = new System.Windows.Forms.ToolStrip();
		this.menuFullscreen = new System.Windows.Forms.ToolStripButton();
		this.findBar = new FindBar();
		this.statusStrip = new System.Windows.Forms.StatusStrip();
		this.connectionsButton = new DetroitDiesel.Windows.Forms.Diagnostics.Container.ConnectionStatusButton();
		this.statusSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.statusStripPanelLogTime = new System.Windows.Forms.ToolStripStatusLabel();
		this.statusStripPanelEcuConnection = new System.Windows.Forms.ToolStripStatusLabel();
		this.statusStripPanelServerConnection = new System.Windows.Forms.ToolStripStatusLabel();
		this.statusStripProgressBarServerConnection = new System.Windows.Forms.ToolStripProgressBar();
		this.toolStripContainer = new System.Windows.Forms.Panel();
		this.toolStripContainer2 = new System.Windows.Forms.Panel();
		this.navigationToolStrip = new System.Windows.Forms.ToolStrip();
		this.toolStripButtonBack = new System.Windows.Forms.ToolStripSplitButton();
		this.toolStripButtonForward = new System.Windows.Forms.ToolStripSplitButton();
		this.mainTabView = new DetroitDiesel.Windows.Forms.Diagnostics.Container.TabbedView();
		this.menuItemZenZefiT = new System.Windows.Forms.ToolStripMenuItem();
		System.Windows.Forms.ToolStripMenuItem toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		System.Windows.Forms.ToolStripSeparator toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
		System.Windows.Forms.ToolStripSeparator toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		System.Windows.Forms.ToolStripSeparator toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
		System.Windows.Forms.ToolStripSeparator toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
		System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
		System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
		System.Windows.Forms.ToolStripSeparator toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
		System.Windows.Forms.ToolStripSeparator toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
		System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
		System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
		System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
		System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
		System.Windows.Forms.ToolStripSeparator toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
		System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
		System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
		System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
		System.Windows.Forms.ToolStripMenuItem toolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
		System.Windows.Forms.ToolStripMenuItem toolStripMenuItem12 = new System.Windows.Forms.ToolStripMenuItem();
		System.Windows.Forms.ToolStripMenuItem toolStripMenuItem13 = new System.Windows.Forms.ToolStripMenuItem();
		System.Windows.Forms.ToolStripMenuItem toolStripMenuItem14 = new System.Windows.Forms.ToolStripMenuItem();
		System.Windows.Forms.ToolStripMenuItem toolStripMenuItem15 = new System.Windows.Forms.ToolStripMenuItem();
		System.Windows.Forms.ToolStripSeparator toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
		System.Windows.Forms.ToolStripMenuItem toolStripMenuItem16 = new System.Windows.Forms.ToolStripMenuItem();
		System.Windows.Forms.ToolStripSeparator toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
		System.Windows.Forms.ToolStripSeparator toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
		System.Windows.Forms.ToolStripMenuItem toolStripMenuItem17 = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStrip.SuspendLayout();
		this.menuStrip1.SuspendLayout();
		this.fullScreenStrip.SuspendLayout();
		this.statusStrip.SuspendLayout();
		this.toolStripContainer.SuspendLayout();
		this.toolStripContainer2.SuspendLayout();
		this.navigationToolStrip.SuspendLayout();
		base.SuspendLayout();
		toolStripMenuItem.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.open_log;
		toolStripMenuItem.Name = "menuLogOpen";
		resources.ApplyResources(toolStripMenuItem, "menuLogOpen");
		toolStripMenuItem.Click += new System.EventHandler(menuLogOpen_Click);
		toolStripSeparator.Name = "menuFileSplit1";
		resources.ApplyResources(toolStripSeparator, "menuFileSplit1");
		toolStripSeparator2.Name = "menuFileSplit2";
		resources.ApplyResources(toolStripSeparator2, "menuFileSplit2");
		toolStripSeparator3.Name = "toolStripSeparator13";
		resources.ApplyResources(toolStripSeparator3, "toolStripSeparator13");
		toolStripSeparator4.Name = "menuFileSplit3";
		resources.ApplyResources(toolStripSeparator4, "menuFileSplit3");
		toolStripMenuItem2.Name = "menuExit";
		resources.ApplyResources(toolStripMenuItem2, "menuExit");
		toolStripMenuItem2.Click += new System.EventHandler(OnMenuExitClick);
		toolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[11]
		{
			this.menuUndo, toolStripSeparator5, this.menuCut, this.menuCopy, this.menuPaste, this.menuDelete, toolStripSeparator6, this.menuSelectAll, toolStripMenuItem4, toolStripMenuItem5,
			toolStripMenuItem6
		});
		toolStripMenuItem3.Name = "menuEdit";
		toolStripMenuItem3.Overflow = System.Windows.Forms.ToolStripItemOverflow.AsNeeded;
		resources.ApplyResources(toolStripMenuItem3, "menuEdit");
		resources.ApplyResources(this.menuUndo, "menuUndo");
		this.menuUndo.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.undo;
		this.menuUndo.Name = "menuUndo";
		this.menuUndo.Click += new System.EventHandler(menuUndo_Click);
		toolStripSeparator5.Name = "editMenuSplit1";
		resources.ApplyResources(toolStripSeparator5, "editMenuSplit1");
		resources.ApplyResources(this.menuCut, "menuCut");
		this.menuCut.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.cut;
		this.menuCut.Name = "menuCut";
		this.menuCut.Click += new System.EventHandler(menuCut_Click);
		resources.ApplyResources(this.menuCopy, "menuCopy");
		this.menuCopy.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.copy;
		this.menuCopy.Name = "menuCopy";
		this.menuCopy.Click += new System.EventHandler(menuCopy_Click);
		resources.ApplyResources(this.menuPaste, "menuPaste");
		this.menuPaste.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.paste;
		this.menuPaste.Name = "menuPaste";
		this.menuPaste.Click += new System.EventHandler(menuPaste_Click);
		resources.ApplyResources(this.menuDelete, "menuDelete");
		this.menuDelete.Name = "menuDelete";
		this.menuDelete.Click += new System.EventHandler(menuDelete_Click);
		toolStripSeparator6.Name = "editMenuSplit2";
		resources.ApplyResources(toolStripSeparator6, "editMenuSplit2");
		resources.ApplyResources(this.menuSelectAll, "menuSelectAll");
		this.menuSelectAll.Name = "menuSelectAll";
		this.menuSelectAll.Click += new System.EventHandler(menuSelectAll_Click);
		toolStripMenuItem4.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.find_next;
		toolStripMenuItem4.Name = "findNextMenu";
		resources.ApplyResources(toolStripMenuItem4, "findNextMenu");
		toolStripMenuItem4.Click += new System.EventHandler(findNextMenu_Click);
		toolStripMenuItem5.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.find_previous;
		toolStripMenuItem5.Name = "findPreviousMenu";
		resources.ApplyResources(toolStripMenuItem5, "findPreviousMenu");
		toolStripMenuItem5.Click += new System.EventHandler(findPreviousMenu_Click);
		toolStripMenuItem6.Name = "findMenu";
		resources.ApplyResources(toolStripMenuItem6, "findMenu");
		toolStripMenuItem6.Click += new System.EventHandler(findMenu_Click);
		resources.ApplyResources(toolStripMenuItem7, "menuNoViews");
		toolStripMenuItem7.Name = "menuNoViews";
		toolStripSeparator7.Name = "menuLogSplitter";
		resources.ApplyResources(toolStripSeparator7, "menuLogSplitter");
		toolStripMenuItem8.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.menuUpdate, this.menuToolsSeperator1, toolStripMenuItem9 });
		toolStripMenuItem8.Name = "menuTools";
		toolStripMenuItem8.Overflow = System.Windows.Forms.ToolStripItemOverflow.AsNeeded;
		resources.ApplyResources(toolStripMenuItem8, "menuTools");
		this.menuUpdate.Name = "menuUpdate";
		resources.ApplyResources(this.menuUpdate, "menuUpdate");
		this.menuUpdate.Click += new System.EventHandler(OnMenuUpdateClick);
		this.menuToolsSeperator1.Name = "menuToolsSeperator1";
		resources.ApplyResources(this.menuToolsSeperator1, "menuToolsSeperator1");
		toolStripMenuItem9.Name = "menuOptions";
		resources.ApplyResources(toolStripMenuItem9, "menuOptions");
		toolStripMenuItem9.Click += new System.EventHandler(OnMenuOptionsClick);
		toolStripMenuItem10.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[11]
		{
			this.menuLogPlayPause, this.menuLogStop, toolStripMenuItem11, toolStripMenuItem14, toolStripSeparator7, this.menuLogLabel, this.menuLogEditableLabel, this.menuLogUpload, this.switchDataMiningProcessTagToolStripMenuItem, this.infoLogSplitter,
			this.logFileInfo
		});
		toolStripMenuItem10.Name = "menuLog";
		toolStripMenuItem10.Overflow = System.Windows.Forms.ToolStripItemOverflow.AsNeeded;
		resources.ApplyResources(toolStripMenuItem10, "menuLog");
		this.menuLogPlayPause.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.log_play;
		this.menuLogPlayPause.Name = "menuLogPlayPause";
		resources.ApplyResources(this.menuLogPlayPause, "menuLogPlayPause");
		this.menuLogPlayPause.Click += new System.EventHandler(menuLogPlay_Click);
		this.menuLogStop.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.log_stop;
		this.menuLogStop.Name = "menuLogStop";
		resources.ApplyResources(this.menuLogStop, "menuLogStop");
		this.menuLogStop.Click += new System.EventHandler(menuLogStop_Click);
		toolStripMenuItem11.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[2] { toolStripMenuItem12, toolStripMenuItem13 });
		toolStripMenuItem11.Name = "menuLogSpeed";
		resources.ApplyResources(toolStripMenuItem11, "menuLogSpeed");
		toolStripMenuItem12.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.menuLogSpeedX1, this.menuLogSpeedX2, this.menuLogSpeedX4, this.menuLogSpeedX8 });
		toolStripMenuItem12.Name = "menuFastForward";
		resources.ApplyResources(toolStripMenuItem12, "menuFastForward");
		this.menuLogSpeedX1.Name = "menuLogSpeedX1";
		resources.ApplyResources(this.menuLogSpeedX1, "menuLogSpeedX1");
		this.menuLogSpeedX1.Click += new System.EventHandler(menuLogSpeedX1_Click);
		this.menuLogSpeedX2.Name = "menuLogSpeedX2";
		resources.ApplyResources(this.menuLogSpeedX2, "menuLogSpeedX2");
		this.menuLogSpeedX2.Click += new System.EventHandler(menuLogSpeedX2_Click);
		this.menuLogSpeedX4.Name = "menuLogSpeedX4";
		resources.ApplyResources(this.menuLogSpeedX4, "menuLogSpeedX4");
		this.menuLogSpeedX4.Click += new System.EventHandler(menuLogSpeedX4_Click);
		this.menuLogSpeedX8.Name = "menuLogSpeedX8";
		resources.ApplyResources(this.menuLogSpeedX8, "menuLogSpeedX8");
		this.menuLogSpeedX8.Click += new System.EventHandler(menuLogSpeedX8_Click);
		toolStripMenuItem13.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.menuLogSpeedX1B, this.menuLogSpeedX2B, this.menuLogSpeedX4B, this.menuLogSpeedX8B });
		toolStripMenuItem13.Name = "menuRewind";
		resources.ApplyResources(toolStripMenuItem13, "menuRewind");
		this.menuLogSpeedX1B.Name = "menuLogSpeedX1B";
		resources.ApplyResources(this.menuLogSpeedX1B, "menuLogSpeedX1B");
		this.menuLogSpeedX1B.Click += new System.EventHandler(menuLogSpeedX1B_Click);
		this.menuLogSpeedX2B.Name = "menuLogSpeedX2B";
		resources.ApplyResources(this.menuLogSpeedX2B, "menuLogSpeedX2B");
		this.menuLogSpeedX2B.Click += new System.EventHandler(menuLogSpeedX2B_Click);
		this.menuLogSpeedX4B.Name = "menuLogSpeedX4B";
		resources.ApplyResources(this.menuLogSpeedX4B, "menuLogSpeedX4B");
		this.menuLogSpeedX4B.Click += new System.EventHandler(menuLogSpeedX4B_Click);
		this.menuLogSpeedX8B.Name = "menuLogSpeedX8B";
		resources.ApplyResources(this.menuLogSpeedX8B, "menuLogSpeedX8B");
		this.menuLogSpeedX8B.Click += new System.EventHandler(menuLogSpeedX8B_Click);
		toolStripMenuItem14.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.menuLogSeekStart, this.menuLogSeekEnd, this.menuLogSeekLabel });
		toolStripMenuItem14.Name = "menuLogSeek";
		resources.ApplyResources(toolStripMenuItem14, "menuLogSeek");
		this.menuLogSeekStart.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.log_seek_start;
		this.menuLogSeekStart.Name = "menuLogSeekStart";
		resources.ApplyResources(this.menuLogSeekStart, "menuLogSeekStart");
		this.menuLogSeekStart.Click += new System.EventHandler(menuLogSeekStart_Click);
		this.menuLogSeekEnd.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.log_seek_end;
		this.menuLogSeekEnd.Name = "menuLogSeekEnd";
		resources.ApplyResources(this.menuLogSeekEnd, "menuLogSeekEnd");
		this.menuLogSeekEnd.Click += new System.EventHandler(menuLogSeekEnd_Click);
		this.menuLogSeekLabel.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.seek_time;
		this.menuLogSeekLabel.Name = "menuLogSeekLabel";
		resources.ApplyResources(this.menuLogSeekLabel, "menuLogSeekLabel");
		this.menuLogSeekLabel.Click += new System.EventHandler(OnSeekTime);
		this.menuLogLabel.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.add_user_event;
		this.menuLogLabel.Name = "menuLogLabel";
		resources.ApplyResources(this.menuLogLabel, "menuLogLabel");
		this.menuLogLabel.Click += new System.EventHandler(UserEventLabelClick);
		this.menuLogEditableLabel.Name = "menuLogEditableLabel";
		resources.ApplyResources(this.menuLogEditableLabel, "menuLogEditableLabel");
		this.menuLogEditableLabel.Click += new System.EventHandler(EditableUserEventLabelClick);
		this.menuLogUpload.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.upload_log;
		this.menuLogUpload.Name = "menuLogUpload";
		resources.ApplyResources(this.menuLogUpload, "menuLogUpload");
		this.menuLogUpload.Click += new System.EventHandler(MarkLogForUploadClick);
		resources.ApplyResources(this.switchDataMiningProcessTagToolStripMenuItem, "switchDataMiningProcessTagToolStripMenuItem");
		this.switchDataMiningProcessTagToolStripMenuItem.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.summaryfiles_noupload;
		this.switchDataMiningProcessTagToolStripMenuItem.Name = "switchDataMiningProcessTagToolStripMenuItem";
		this.switchDataMiningProcessTagToolStripMenuItem.Click += new System.EventHandler(switchDataMiningProcessTagClick);
		this.infoLogSplitter.Name = "infoLogSplitter";
		resources.ApplyResources(this.infoLogSplitter, "infoLogSplitter");
		resources.ApplyResources(this.logFileInfo, "logFileInfo");
		this.logFileInfo.Name = "logFileInfo";
		this.logFileInfo.Click += new System.EventHandler(ShowLogFileInfo);
		toolStripMenuItem15.Name = "menuItemFullScreen";
		resources.ApplyResources(toolStripMenuItem15, "menuItemFullScreen");
		toolStripMenuItem15.Click += new System.EventHandler(menuItemFullScreen_Click);
		toolStripSeparator8.Name = "toolStripSeparator5";
		resources.ApplyResources(toolStripSeparator8, "toolStripSeparator5");
		toolStripMenuItem16.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[10] { this.menuTroubleshootingGuides, toolStripSeparator9, this.menuItemHelp, this.contextHelpToolStripMenuItem, toolStripSeparator10, this.menuItemReferences, this.toolStripSeparator8, this.menuFeedBack, this.toolStripSeparator12, toolStripMenuItem17 });
		toolStripMenuItem16.Name = "menuHelp";
		toolStripMenuItem16.Overflow = System.Windows.Forms.ToolStripItemOverflow.AsNeeded;
		resources.ApplyResources(toolStripMenuItem16, "menuHelp");
		resources.ApplyResources(this.menuTroubleshootingGuides, "menuTroubleshootingGuides");
		this.menuTroubleshootingGuides.Name = "menuTroubleshootingGuides";
		toolStripSeparator9.Name = "menuHelpSeparator1";
		resources.ApplyResources(toolStripSeparator9, "menuHelpSeparator1");
		this.menuItemHelp.Name = "menuItemHelp";
		resources.ApplyResources(this.menuItemHelp, "menuItemHelp");
		this.menuItemHelp.Click += new System.EventHandler(OnHelpClick);
		this.contextHelpToolStripMenuItem.Name = "contextHelpToolStripMenuItem";
		resources.ApplyResources(this.contextHelpToolStripMenuItem, "contextHelpToolStripMenuItem");
		this.contextHelpToolStripMenuItem.Click += new System.EventHandler(OnContextHelpClick);
		toolStripSeparator10.Name = "menuHelpSeperator2";
		resources.ApplyResources(toolStripSeparator10, "menuHelpSeperator2");
		this.menuItemReferences.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[8] { this.menuItemDtnaConnect, this.menuItemCatalog, this.menuItemVehicleInformation, this.menuItemDataMiningReports, this.menuItemZenZefiT, this.menuItemAandIManual, this.menuItemFcccEngines, this.menuItemReferenceGuide });
		this.menuItemReferences.Name = "menuItemReferences";
		resources.ApplyResources(this.menuItemReferences, "menuItemReferences");
		this.menuItemDtnaConnect.Name = "menuItemDtnaConnect";
		resources.ApplyResources(this.menuItemDtnaConnect, "menuItemDtnaConnect");
		this.menuItemDtnaConnect.Click += new System.EventHandler(menuItemDtnaConnect_Click);
		this.menuItemCatalog.Name = "menuItemCatalog";
		resources.ApplyResources(this.menuItemCatalog, "menuItemCatalog");
		this.menuItemCatalog.Click += new System.EventHandler(OnMenuPartCatalogClick);
		this.menuItemVehicleInformation.Name = "menuItemVehicleInformation";
		resources.ApplyResources(this.menuItemVehicleInformation, "menuItemVehicleInformation");
		this.menuItemVehicleInformation.Click += new System.EventHandler(OnMenuVehicleInformationClick);
		this.menuItemDataMiningReports.Name = "menuItemDataMiningReports";
		resources.ApplyResources(this.menuItemDataMiningReports, "menuItemDataMiningReports");
		this.menuItemDataMiningReports.Click += new System.EventHandler(OnMenuDataMiningReportsClick);
		this.menuItemAandIManual.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.menuItemDDECVIAI, this.menuItemDDEC10AI, this.menuItemDDEC13AI, this.menuItemEuro4AI, this.menuItemDetroitTransmissionsAI });
		this.menuItemAandIManual.Name = "menuItemAandIManual";
		resources.ApplyResources(this.menuItemAandIManual, "menuItemAandIManual");
		this.menuItemDDECVIAI.Name = "menuItemDDECVIAI";
		resources.ApplyResources(this.menuItemDDECVIAI, "menuItemDDECVIAI");
		this.menuItemDDECVIAI.Click += new System.EventHandler(OnMenuItemDDECVIAIClick);
		this.menuItemDDEC10AI.Name = "menuItemDDEC10AI";
		resources.ApplyResources(this.menuItemDDEC10AI, "menuItemDDEC10AI");
		this.menuItemDDEC10AI.Click += new System.EventHandler(OnMenuDDEC10AIClick);
		this.menuItemDDEC13AI.Name = "menuItemDDEC13AI";
		resources.ApplyResources(this.menuItemDDEC13AI, "menuItemDDEC13AI");
		this.menuItemDDEC13AI.Click += new System.EventHandler(OnMenuDDEC13AIClick);
		this.menuItemEuro4AI.Name = "menuItemEuro4AI";
		resources.ApplyResources(this.menuItemEuro4AI, "menuItemEuro4AI");
		this.menuItemEuro4AI.Click += new System.EventHandler(OnMenuEuro4AIClick);
		this.menuItemDetroitTransmissionsAI.Name = "menuItemDetroitTransmissionsAI";
		resources.ApplyResources(this.menuItemDetroitTransmissionsAI, "menuItemDetroitTransmissionsAI");
		this.menuItemDetroitTransmissionsAI.Click += new System.EventHandler(menuItemDetroitTransmissionsAIClick);
		this.menuItemFcccEngines.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.menuItemFcccEngineSupport, this.menuItemFcccOasis, this.menuItemFcccRVChassis, this.menuItemFcccS2B2Chassis, this.menuItemFcccEconic });
		this.menuItemFcccEngines.Name = "menuItemFcccEngines";
		resources.ApplyResources(this.menuItemFcccEngines, "menuItemFcccEngines");
		this.menuItemFcccEngineSupport.Name = "menuItemFcccEngineSupport";
		resources.ApplyResources(this.menuItemFcccEngineSupport, "menuItemFcccEngineSupport");
		this.menuItemFcccEngineSupport.Click += new System.EventHandler(OnMenuItemFcccEngineSupportClick);
		this.menuItemFcccOasis.Name = "menuItemFcccOasis";
		resources.ApplyResources(this.menuItemFcccOasis, "menuItemFcccOasis");
		this.menuItemFcccOasis.Click += new System.EventHandler(OnMenuItemFcccOasisClick);
		this.menuItemFcccRVChassis.Name = "menuItemFcccRVChassis";
		resources.ApplyResources(this.menuItemFcccRVChassis, "menuItemFcccRVChassis");
		this.menuItemFcccRVChassis.Click += new System.EventHandler(OnMenuItemFcccRVChassisClick);
		this.menuItemFcccS2B2Chassis.Name = "menuItemFcccS2B2Chassis";
		resources.ApplyResources(this.menuItemFcccS2B2Chassis, "menuItemFcccS2B2Chassis");
		this.menuItemFcccS2B2Chassis.Click += new System.EventHandler(OnMenuItemFcccS2B2ChassisClick);
		this.menuItemFcccEconic.Name = "menuItemFcccEconic";
		resources.ApplyResources(this.menuItemFcccEconic, "menuItemFcccEconic");
		this.menuItemFcccEconic.Click += new System.EventHandler(OnMenuItemFcccEconicClick);
		this.menuItemReferenceGuide.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.menuItemDDEC10RefrenceGuide, this.menuItemDDEC13RefrenceGuide, this.menuItemDetroitTransmissionsRG });
		this.menuItemReferenceGuide.Name = "menuItemReferenceGuide";
		resources.ApplyResources(this.menuItemReferenceGuide, "menuItemReferenceGuide");
		this.menuItemDDEC10RefrenceGuide.Name = "menuItemDDEC10RefrenceGuide";
		resources.ApplyResources(this.menuItemDDEC10RefrenceGuide, "menuItemDDEC10RefrenceGuide");
		this.menuItemDDEC10RefrenceGuide.Click += new System.EventHandler(OnMenuItemDDEC10ReferenceGuideClick);
		this.menuItemDDEC13RefrenceGuide.Name = "menuItemDDEC13RefrenceGuide";
		resources.ApplyResources(this.menuItemDDEC13RefrenceGuide, "menuItemDDEC13RefrenceGuide");
		this.menuItemDDEC13RefrenceGuide.Click += new System.EventHandler(OnMenuItemDDEC13ReferenceGuideClick);
		this.menuItemDetroitTransmissionsRG.Name = "menuItemDetroitTransmissionsRG";
		resources.ApplyResources(this.menuItemDetroitTransmissionsRG, "menuItemDetroitTransmissionsRG");
		this.menuItemDetroitTransmissionsRG.Click += new System.EventHandler(OnMenuDetroitTransmissionsRGClick);
		this.toolStripSeparator8.Name = "toolStripSeparator8";
		resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
		this.menuFeedBack.Name = "menuFeedBack";
		resources.ApplyResources(this.menuFeedBack, "menuFeedBack");
		this.menuFeedBack.Click += new System.EventHandler(menuFeedBack_Click);
		this.toolStripSeparator12.Name = "toolStripSeparator12";
		resources.ApplyResources(this.toolStripSeparator12, "toolStripSeparator12");
		toolStripMenuItem17.Name = "menuAbout";
		resources.ApplyResources(toolStripMenuItem17, "menuAbout");
		toolStripMenuItem17.Click += new System.EventHandler(menuAbout_Click);
		this.menuConnect.Name = "menuConnect";
		resources.ApplyResources(this.menuConnect, "menuConnect");
		this.menuConnect.Click += new System.EventHandler(OnMenuConnectClick);
		this.menuClose.Name = "menuClose";
		resources.ApplyResources(this.menuClose, "menuClose");
		this.menuClose.Click += new System.EventHandler(OnCloseClick);
		this.menuPrint.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.print;
		this.menuPrint.Name = "menuPrint";
		resources.ApplyResources(this.menuPrint, "menuPrint");
		this.menuPrint.Click += new System.EventHandler(OnMenuPrintClick);
		this.menuPrintPreview.Name = "menuPrintPreview";
		resources.ApplyResources(this.menuPrintPreview, "menuPrintPreview");
		this.menuPrintPreview.Click += new System.EventHandler(OnMenuPrintPreviewClick);
		resources.ApplyResources(this.menuRecentFiles, "menuRecentFiles");
		this.menuRecentFiles.Name = "menuRecentFiles";
		resources.ApplyResources(this.toolStrip, "toolStrip");
		this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		this.toolStrip.ImageList = this.imageListToolStrip;
		this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[34]
		{
			this.toolStripButtonRetryAutoConnect,
			this.toolStripButtonBusMonitor,
			this.toolStripButtonMute,
			this.toolStripSeparator1,
			this.toolBarButtonOpen,
			this.toolBarButtonPlayPause,
			this.toolBarButtonStop,
			this.toolBarButtonSeekStart,
			this.toolBarButtonRewind,
			(System.Windows.Forms.ToolStripItem)(object)this.toolBarTrackBarSeek,
			this.toolBarButtonFastForward,
			this.toolBarButtonSeekEnd,
			this.toolStripButtonSeekTime,
			toolStripSeparator8,
			this.toolBarButtonUserEvent,
			this.toolBarButtonLogUpload,
			this.toolStripButtonDataMiningTag,
			this.toolStripSeparator2,
			this.toolStripButtonPrint,
			this.toolStripSeparator3,
			this.toolBarButtonCut,
			this.toolBarButtonCopy,
			this.toolBarButtonPaste,
			this.toolStripSeparator16,
			this.toolBarButtonUndo,
			this.toolStripSeparator4,
			this.toolStripButtonFilter,
			this.toolStripSeparator7,
			this.toolBarButtonRefresh,
			this.toolStripSeparator6,
			this.toolStripButtonUnitsSystem,
			this.toolStripSeparator17,
			this.toolStripButtonExpandAll,
			this.toolStripButtonCollapseAll
		});
		this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
		this.toolStrip.Name = "toolStrip";
		this.toolStrip.TabStop = true;
		this.imageListToolStrip.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
		resources.ApplyResources(this.imageListToolStrip, "imageListToolStrip");
		this.imageListToolStrip.TransparentColor = System.Drawing.Color.Transparent;
		resources.ApplyResources(this.toolStripButtonRetryAutoConnect, "toolStripButtonRetryAutoConnect");
		this.toolStripButtonRetryAutoConnect.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.retry_autoconnect;
		this.toolStripButtonRetryAutoConnect.Name = "toolStripButtonRetryAutoConnect";
		this.toolStripButtonRetryAutoConnect.Click += new System.EventHandler(toolStripButtonRetryAutoConnect_Click);
		this.toolStripButtonBusMonitor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolStripButtonBusMonitor.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.environment_view;
		resources.ApplyResources(this.toolStripButtonBusMonitor, "toolStripButtonBusMonitor");
		this.toolStripButtonBusMonitor.Name = "toolStripButtonBusMonitor";
		this.toolStripButtonBusMonitor.Click += new System.EventHandler(toolStripButtonBusMonitor_Click);
		this.toolStripButtonMute.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolStripButtonMute.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.cd_pause;
		resources.ApplyResources(this.toolStripButtonMute, "toolStripButtonMute");
		this.toolStripButtonMute.Name = "toolStripButtonMute";
		this.toolStripButtonMute.Click += new System.EventHandler(toolStripButtonMute_Click);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
		resources.ApplyResources(this.toolBarButtonOpen, "toolBarButtonOpen");
		this.toolBarButtonOpen.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.open_log;
		this.toolBarButtonOpen.Name = "toolBarButtonOpen";
		this.toolBarButtonOpen.Click += new System.EventHandler(toolBarButtonOpen_Click);
		resources.ApplyResources(this.toolBarButtonPlayPause, "toolBarButtonPlayPause");
		this.toolBarButtonPlayPause.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.log_play;
		this.toolBarButtonPlayPause.Name = "toolBarButtonPlayPause";
		this.toolBarButtonPlayPause.Click += new System.EventHandler(toolBarButtonPlayPause_Click);
		resources.ApplyResources(this.toolBarButtonStop, "toolBarButtonStop");
		this.toolBarButtonStop.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.log_stop;
		this.toolBarButtonStop.Name = "toolBarButtonStop";
		this.toolBarButtonStop.Click += new System.EventHandler(toolBarButtonStop_Click);
		resources.ApplyResources(this.toolBarButtonSeekStart, "toolBarButtonSeekStart");
		this.toolBarButtonSeekStart.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.log_seek_start;
		this.toolBarButtonSeekStart.Name = "toolBarButtonSeekStart";
		this.toolBarButtonSeekStart.Click += new System.EventHandler(toolBarButtonSeekStart_Click);
		resources.ApplyResources(this.toolBarButtonRewind, "toolBarButtonRewind");
		this.toolBarButtonRewind.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.log_rewind;
		this.toolBarButtonRewind.Name = "toolBarButtonRewind";
		this.toolBarButtonRewind.Click += new System.EventHandler(toolBarButtonRewind_Click);
		((System.Windows.Forms.ToolStripItem)(object)this.toolBarTrackBarSeek).BackColor = System.Drawing.Color.Transparent;
		this.toolBarTrackBarSeek.Maximum = 10.0;
		((System.Windows.Forms.ToolStripItem)(object)this.toolBarTrackBarSeek).Name = "toolBarTrackBarSeek";
		resources.ApplyResources(this.toolBarTrackBarSeek, "toolBarTrackBarSeek");
		this.toolBarTrackBarSeek.ValueChanged += new System.EventHandler<ValueChangedEventArgs>(OnSeekTrackBarValueChanged);
		resources.ApplyResources(this.toolBarButtonFastForward, "toolBarButtonFastForward");
		this.toolBarButtonFastForward.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.log_fast_forward;
		this.toolBarButtonFastForward.Name = "toolBarButtonFastForward";
		this.toolBarButtonFastForward.Click += new System.EventHandler(toolBarButtonFastForward_Click);
		resources.ApplyResources(this.toolBarButtonSeekEnd, "toolBarButtonSeekEnd");
		this.toolBarButtonSeekEnd.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.log_seek_end;
		this.toolBarButtonSeekEnd.Name = "toolBarButtonSeekEnd";
		this.toolBarButtonSeekEnd.Click += new System.EventHandler(toolBarButtonSeekEnd_Click);
		this.toolStripButtonSeekTime.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		resources.ApplyResources(this.toolStripButtonSeekTime, "toolStripButtonSeekTime");
		this.toolStripButtonSeekTime.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.seek_time;
		this.toolStripButtonSeekTime.Name = "toolStripButtonSeekTime";
		this.toolStripButtonSeekTime.Click += new System.EventHandler(OnSeekTime);
		this.toolBarButtonUserEvent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolBarButtonUserEvent.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.toolBarMenuItemLabel, this.toolBarMenuItemEditableLabel });
		resources.ApplyResources(this.toolBarButtonUserEvent, "toolBarButtonUserEvent");
		this.toolBarButtonUserEvent.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.add_user_event;
		this.toolBarButtonUserEvent.Name = "toolBarButtonUserEvent";
		this.toolBarButtonUserEvent.ButtonClick += new System.EventHandler(toolBarButtonUserEventClick);
		this.toolBarMenuItemLabel.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.add_user_event;
		this.toolBarMenuItemLabel.Name = "toolBarMenuItemLabel";
		resources.ApplyResources(this.toolBarMenuItemLabel, "toolBarMenuItemLabel");
		this.toolBarMenuItemLabel.Click += new System.EventHandler(UserEventLabelClick);
		this.toolBarMenuItemEditableLabel.Name = "toolBarMenuItemEditableLabel";
		resources.ApplyResources(this.toolBarMenuItemEditableLabel, "toolBarMenuItemEditableLabel");
		this.toolBarMenuItemEditableLabel.Click += new System.EventHandler(EditableUserEventLabelClick);
		resources.ApplyResources(this.toolBarButtonLogUpload, "toolBarButtonLogUpload");
		this.toolBarButtonLogUpload.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.upload_log;
		this.toolBarButtonLogUpload.Name = "toolBarButtonLogUpload";
		this.toolBarButtonLogUpload.Click += new System.EventHandler(MarkLogForUploadClick);
		resources.ApplyResources(this.toolStripButtonDataMiningTag, "toolStripButtonDataMiningTag");
		this.toolStripButtonDataMiningTag.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.summaryfiles_noupload;
		this.toolStripButtonDataMiningTag.Name = "toolStripButtonDataMiningTag";
		this.toolStripButtonDataMiningTag.Click += new System.EventHandler(switchDataMiningProcessTagClick);
		this.toolStripSeparator2.Name = "toolStripSeparator2";
		resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
		resources.ApplyResources(this.toolStripButtonPrint, "toolStripButtonPrint");
		this.toolStripButtonPrint.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.print;
		this.toolStripButtonPrint.Name = "toolStripButtonPrint";
		this.toolStripButtonPrint.Click += new System.EventHandler(toolStripButtonPrint_Click);
		this.toolStripSeparator3.Name = "toolStripSeparator3";
		resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
		resources.ApplyResources(this.toolBarButtonCut, "toolBarButtonCut");
		this.toolBarButtonCut.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.cut;
		this.toolBarButtonCut.Name = "toolBarButtonCut";
		this.toolBarButtonCut.Click += new System.EventHandler(toolBarButtonCut_Click);
		resources.ApplyResources(this.toolBarButtonCopy, "toolBarButtonCopy");
		this.toolBarButtonCopy.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.copy;
		this.toolBarButtonCopy.Name = "toolBarButtonCopy";
		this.toolBarButtonCopy.Click += new System.EventHandler(toolBarButtonCopy_Click);
		resources.ApplyResources(this.toolBarButtonPaste, "toolBarButtonPaste");
		this.toolBarButtonPaste.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.paste;
		this.toolBarButtonPaste.Name = "toolBarButtonPaste";
		this.toolBarButtonPaste.Click += new System.EventHandler(toolBarButtonPaste_Click);
		this.toolStripSeparator16.Name = "toolStripSeparator16";
		resources.ApplyResources(this.toolStripSeparator16, "toolStripSeparator16");
		resources.ApplyResources(this.toolBarButtonUndo, "toolBarButtonUndo");
		this.toolBarButtonUndo.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.undo;
		this.toolBarButtonUndo.Name = "toolBarButtonUndo";
		this.toolBarButtonUndo.Click += new System.EventHandler(toolBarButtonUndo_Click);
		this.toolStripSeparator4.Name = "toolStripSeparator4";
		resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
		this.toolStripButtonFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolStripButtonFilter.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.funnel;
		resources.ApplyResources(this.toolStripButtonFilter, "toolStripButtonFilter");
		this.toolStripButtonFilter.Name = "toolStripButtonFilter";
		this.toolStripButtonFilter.Click += new System.EventHandler(toolStripButtonFilter_Click);
		this.toolStripSeparator7.Name = "toolStripSeparator7";
		resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
		this.toolBarButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolBarButtonRefresh.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.refresh;
		resources.ApplyResources(this.toolBarButtonRefresh, "toolBarButtonRefresh");
		this.toolBarButtonRefresh.Name = "toolBarButtonRefresh";
		this.toolBarButtonRefresh.Click += new System.EventHandler(toolBarButtonRefresh_Click);
		this.toolStripSeparator6.Name = "toolStripSeparator6";
		resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
		this.toolStripButtonUnitsSystem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolStripButtonUnitsSystem.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.units_system;
		this.toolStripButtonUnitsSystem.Name = "toolStripButtonUnitsSystem";
		resources.ApplyResources(this.toolStripButtonUnitsSystem, "toolStripButtonUnitsSystem");
		this.toolStripButtonUnitsSystem.Click += new System.EventHandler(toolStripButtonUnitsSystem_Click);
		this.toolStripSeparator17.Name = "toolStripSeparator17";
		resources.ApplyResources(this.toolStripSeparator17, "toolStripSeparator17");
		this.toolStripButtonExpandAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolStripButtonExpandAll.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.expandall;
		resources.ApplyResources(this.toolStripButtonExpandAll, "toolStripButtonExpandAll");
		this.toolStripButtonExpandAll.Name = "toolStripButtonExpandAll";
		this.toolStripButtonExpandAll.Click += new System.EventHandler(toolStripButtonExpandAll_Click);
		this.toolStripButtonCollapseAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolStripButtonCollapseAll.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.collapseall;
		resources.ApplyResources(this.toolStripButtonCollapseAll, "toolStripButtonCollapseAll");
		this.toolStripButtonCollapseAll.Name = "toolStripButtonCollapseAll";
		this.toolStripButtonCollapseAll.Click += new System.EventHandler(toolStripButtonCollapseAll_Click);
		resources.ApplyResources(this.menuStrip1, "menuStrip1");
		this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[6] { this.menuFile, toolStripMenuItem3, this.menuView, toolStripMenuItem10, toolStripMenuItem8, toolStripMenuItem16 });
		this.menuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
		this.menuStrip1.Name = "menuStrip1";
		this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[12]
		{
			toolStripMenuItem, this.menuConnect, toolStripSeparator, this.menuClose, toolStripSeparator2, this.menuPrint, this.menuPrintPreview, this.menuPrintFaults, toolStripSeparator3, this.menuRecentFiles,
			toolStripSeparator4, toolStripMenuItem2
		});
		this.menuFile.Name = "menuFile";
		this.menuFile.Overflow = System.Windows.Forms.ToolStripItemOverflow.AsNeeded;
		resources.ApplyResources(this.menuFile, "menuFile");
		this.menuPrintFaults.Name = "menuPrintFaults";
		resources.ApplyResources(this.menuPrintFaults, "menuPrintFaults");
		this.menuPrintFaults.Click += new System.EventHandler(OnMenuPrintFaultsClick);
		this.menuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[15]
		{
			toolStripMenuItem7, this.menuViewSplit2, this.menuNextView, this.menuPreviousView, this.toolStripSeparator9, this.gotoToolStripMenuItem, this.toolStripSeparator11, toolStripMenuItem15, this.toolStripSeparator14, this.filterToolStripMenuItem,
			this.menuViewSplit1, this.busMonitoringToolStripMenuItem, this.toolStripMenuItemMute, this.toolStripSeparator15, this.refreshToolStripMenuItem
		});
		this.menuView.Name = "menuView";
		this.menuView.Overflow = System.Windows.Forms.ToolStripItemOverflow.AsNeeded;
		resources.ApplyResources(this.menuView, "menuView");
		this.menuView.DropDownOpened += new System.EventHandler(menuView_Popup);
		this.menuViewSplit2.Name = "menuViewSplit2";
		resources.ApplyResources(this.menuViewSplit2, "menuViewSplit2");
		resources.ApplyResources(this.menuNextView, "menuNextView");
		this.menuNextView.Name = "menuNextView";
		this.menuNextView.Click += new System.EventHandler(nextMenu_Click);
		resources.ApplyResources(this.menuPreviousView, "menuPreviousView");
		this.menuPreviousView.Name = "menuPreviousView";
		this.menuPreviousView.Click += new System.EventHandler(prevMenu_Click);
		this.toolStripSeparator9.Name = "toolStripSeparator9";
		resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
		this.gotoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.backToolStripMenuItem, this.forwardToolStripMenuItem, this.toolStripSeparator10 });
		this.gotoToolStripMenuItem.Name = "gotoToolStripMenuItem";
		resources.ApplyResources(this.gotoToolStripMenuItem, "gotoToolStripMenuItem");
		this.gotoToolStripMenuItem.DropDownOpening += new System.EventHandler(gotoToolStripMenuItem_DropDownOpening);
		this.gotoToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(gotoToolStripMenuItem_DropDownItemClicked);
		this.backToolStripMenuItem.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.back;
		resources.ApplyResources(this.backToolStripMenuItem, "backToolStripMenuItem");
		this.backToolStripMenuItem.Name = "backToolStripMenuItem";
		this.backToolStripMenuItem.Click += new System.EventHandler(OnHistoryBackClick);
		this.forwardToolStripMenuItem.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.forward;
		resources.ApplyResources(this.forwardToolStripMenuItem, "forwardToolStripMenuItem");
		this.forwardToolStripMenuItem.Name = "forwardToolStripMenuItem";
		this.forwardToolStripMenuItem.Click += new System.EventHandler(OnHistoryForwardClick);
		this.toolStripSeparator10.Name = "toolStripSeparator10";
		resources.ApplyResources(this.toolStripSeparator10, "toolStripSeparator10");
		this.toolStripSeparator11.Name = "toolStripSeparator11";
		resources.ApplyResources(this.toolStripSeparator11, "toolStripSeparator11");
		this.toolStripSeparator14.Name = "toolStripSeparator14";
		resources.ApplyResources(this.toolStripSeparator14, "toolStripSeparator14");
		this.filterToolStripMenuItem.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.funnel;
		this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
		resources.ApplyResources(this.filterToolStripMenuItem, "filterToolStripMenuItem");
		this.filterToolStripMenuItem.Click += new System.EventHandler(toolStripButtonFilter_Click);
		this.menuViewSplit1.Name = "menuViewSplit1";
		resources.ApplyResources(this.menuViewSplit1, "menuViewSplit1");
		this.busMonitoringToolStripMenuItem.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.environment_view;
		this.busMonitoringToolStripMenuItem.Name = "busMonitoringToolStripMenuItem";
		resources.ApplyResources(this.busMonitoringToolStripMenuItem, "busMonitoringToolStripMenuItem");
		this.busMonitoringToolStripMenuItem.Click += new System.EventHandler(busMonitoringToolStripMenuItem_Click);
		this.toolStripMenuItemMute.BackColor = System.Drawing.SystemColors.Control;
		this.toolStripMenuItemMute.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.cd_pause;
		this.toolStripMenuItemMute.Name = "toolStripMenuItemMute";
		resources.ApplyResources(this.toolStripMenuItemMute, "toolStripMenuItemMute");
		this.toolStripMenuItemMute.Click += new System.EventHandler(toolStripMenuItemMute_Click);
		this.toolStripSeparator15.Name = "toolStripSeparator15";
		resources.ApplyResources(this.toolStripSeparator15, "toolStripSeparator15");
		this.refreshToolStripMenuItem.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.refresh;
		this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
		resources.ApplyResources(this.refreshToolStripMenuItem, "refreshToolStripMenuItem");
		this.refreshToolStripMenuItem.Click += new System.EventHandler(refreshToolStripMenuItem_Click);
		resources.ApplyResources(this.fullScreenStrip, "fullScreenStrip");
		this.fullScreenStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		this.fullScreenStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.menuFullscreen });
		this.fullScreenStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
		this.fullScreenStrip.Name = "fullScreenStrip";
		this.menuFullscreen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
		this.menuFullscreen.Name = "menuFullscreen";
		resources.ApplyResources(this.menuFullscreen, "menuFullscreen");
		this.menuFullscreen.Click += new System.EventHandler(menuFullscreen_Click);
		resources.ApplyResources(this.findBar, "findBar");
		((System.Windows.Forms.ToolStrip)(object)this.findBar).GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		((System.Windows.Forms.ToolStrip)(object)this.findBar).LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
		((System.Windows.Forms.Control)(object)this.findBar).Name = "findBar";
		((System.Windows.Forms.ToolStrip)(object)this.findBar).TabStop = true;
		resources.ApplyResources(this.statusStrip, "statusStrip");
		this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[6] { this.connectionsButton, this.statusSeparator1, this.statusStripPanelLogTime, this.statusStripPanelEcuConnection, this.statusStripPanelServerConnection, this.statusStripProgressBarServerConnection });
		this.statusStrip.Name = "statusStrip";
		this.statusStrip.TabStop = true;
		this.connectionsButton.BackColor = System.Drawing.SystemColors.Control;
		this.connectionsButton.Checked = true;
		this.connectionsButton.CheckState = System.Windows.Forms.CheckState.Checked;
		this.connectionsButton.ForeColor = System.Drawing.SystemColors.ControlText;
		this.connectionsButton.Name = "connectionsButton";
		resources.ApplyResources(this.connectionsButton, "connectionsButton");
		this.connectionsButton.Click += new System.EventHandler(OnConnectionsButtonClick);
		this.statusSeparator1.Name = "statusSeparator1";
		resources.ApplyResources(this.statusSeparator1, "statusSeparator1");
		this.statusStripPanelLogTime.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
		this.statusStripPanelLogTime.Name = "statusStripPanelLogTime";
		resources.ApplyResources(this.statusStripPanelLogTime, "statusStripPanelLogTime");
		this.statusStripPanelEcuConnection.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
		this.statusStripPanelEcuConnection.Name = "statusStripPanelEcuConnection";
		resources.ApplyResources(this.statusStripPanelEcuConnection, "statusStripPanelEcuConnection");
		this.statusStripPanelServerConnection.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
		this.statusStripPanelServerConnection.Name = "statusStripPanelServerConnection";
		resources.ApplyResources(this.statusStripPanelServerConnection, "statusStripPanelServerConnection");
		resources.ApplyResources(this.statusStripProgressBarServerConnection, "statusStripProgressBarServerConnection");
		this.statusStripProgressBarServerConnection.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
		this.statusStripProgressBarServerConnection.Name = "statusStripProgressBarServerConnection";
		resources.ApplyResources(this.toolStripContainer, "toolStripContainer");
		this.toolStripContainer.Controls.Add(this.menuStrip1);
		this.toolStripContainer.Controls.Add(this.fullScreenStrip);
		this.toolStripContainer.Name = "toolStripContainer";
		resources.ApplyResources(this.toolStripContainer2, "toolStripContainer2");
		this.toolStripContainer2.Controls.Add(this.toolStrip);
		this.toolStripContainer2.Controls.Add(this.navigationToolStrip);
		this.toolStripContainer2.Controls.Add((System.Windows.Forms.Control)(object)this.findBar);
		this.toolStripContainer2.Name = "toolStripContainer2";
		resources.ApplyResources(this.navigationToolStrip, "navigationToolStrip");
		this.navigationToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		this.navigationToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.toolStripButtonBack, this.toolStripButtonForward });
		this.navigationToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
		this.navigationToolStrip.Name = "navigationToolStrip";
		this.navigationToolStrip.TabStop = true;
		this.toolStripButtonBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		resources.ApplyResources(this.toolStripButtonBack, "toolStripButtonBack");
		this.toolStripButtonBack.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.back;
		this.toolStripButtonBack.Name = "toolStripButtonBack";
		this.toolStripButtonBack.ButtonClick += new System.EventHandler(OnHistoryBackClick);
		this.toolStripButtonBack.DropDownOpening += new System.EventHandler(toolStripButtonBack_DropDownOpening);
		this.toolStripButtonBack.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(toolStripButtonBack_DropDownItemClicked);
		this.toolStripButtonForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		resources.ApplyResources(this.toolStripButtonForward, "toolStripButtonForward");
		this.toolStripButtonForward.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.forward;
		this.toolStripButtonForward.Name = "toolStripButtonForward";
		this.toolStripButtonForward.ButtonClick += new System.EventHandler(OnHistoryForwardClick);
		this.toolStripButtonForward.DropDownOpening += new System.EventHandler(toolStripButtonForward_DropDownOpening);
		this.toolStripButtonForward.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(toolStripButtonForward_DropDownItemClicked);
		this.mainTabView.ContainerApplication = null;
		resources.ApplyResources(this.mainTabView, "mainTabView");
		((System.Windows.Forms.Control)(object)this.mainTabView).Name = "mainTabView";
		this.mainTabView.ShowSidebar = true;
		this.mainTabView.SplashProgressBar = null;
		((System.Windows.Forms.UserControl)(object)this.mainTabView).Load += new System.EventHandler(mainTabView_Load);
		this.menuItemZenZefiT.Name = "menuItemZenZefiT";
		resources.ApplyResources(this.menuItemZenZefiT, "menuItemZenZefiT");
		this.menuItemZenZefiT.Click += new System.EventHandler(menuItemZenZefiT_Click);
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add((System.Windows.Forms.Control)(object)this.mainTabView);
		base.Controls.Add(this.toolStripContainer2);
		base.Controls.Add(this.toolStripContainer);
		base.Controls.Add(this.statusStrip);
		base.Name = "MainForm";
		base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
		this.toolStrip.ResumeLayout(false);
		this.toolStrip.PerformLayout();
		this.menuStrip1.ResumeLayout(false);
		this.menuStrip1.PerformLayout();
		this.fullScreenStrip.ResumeLayout(false);
		this.fullScreenStrip.PerformLayout();
		this.statusStrip.ResumeLayout(false);
		this.statusStrip.PerformLayout();
		this.toolStripContainer.ResumeLayout(false);
		this.toolStripContainer.PerformLayout();
		this.toolStripContainer2.ResumeLayout(false);
		this.toolStripContainer2.PerformLayout();
		this.navigationToolStrip.ResumeLayout(false);
		this.navigationToolStrip.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	void IContainerApplication.SelectPlace(IPlace place)
	{
		mainTabView.SelectPlace(place);
	}

	void IContainerApplication.SelectPlace(PanelIdentifier identifier, string location)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		mainTabView.SelectPlace(identifier, location);
	}

	void IContainerApplication.PreloadPlace(IPlace place)
	{
		mainTabView.EnsurePanelCreated(place);
	}

	void IContainerApplication.DisableForProgramming(bool disabled)
	{
		mainTabView.SidebarEnabled = !disabled;
		((Control)(object)WarningsPanel.GlobalInstance).Enabled = !disabled;
	}

	public MainForm(SplashScreen splashScreen, string logFileToLoad)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Expected O, but got Unknown
		WarningItem.LinkClicked += delegate(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Link.ShowTarget((string)e.Link.LinkData);
		};
		this.splashScreen = splashScreen;
		((Component)(object)this.splashScreen).Disposed += OnSplashScreenDisposed;
		sapiManager = SapiManager.GlobalInstance;
		sapiManager.DesignMode = base.DesignMode;
		AddCaesarComponentInformation();
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		toolStripButtonMute.Visible = false;
		toolStripMenuItemMute.Visible = false;
		using (Graphics graphics = CreateGraphics())
		{
			float num = 20f * graphics.DpiX;
			float num2 = 11.25f * graphics.DpiY;
			MaximumSize = new Size((int)num, (int)num2);
			ToolStrip[] array = new ToolStrip[5] { statusStrip, fullScreenStrip, this.toolStrip, menuStrip1, navigationToolStrip };
			foreach (ToolStrip toolStrip in array)
			{
				toolStrip.AutoSize = false;
				toolStrip.ImageScalingSize = new Size((int)(20f * graphics.DpiX / 96f), (int)(20f * graphics.DpiY / 96f));
				toolStrip.AutoSize = true;
			}
			statusStrip.AutoSize = false;
			statusStrip.Height = menuStrip1.Height;
		}
		mainTabView.SplashProgressBar = (IProgressBar)(object)this.splashScreen;
		mainTabView.ContainerApplication = (IContainerApplication)(object)this;
		mainTabView.SetBranding(ApplicationInformation.Branding);
		base.Icon = ApplicationInformation.Branding.ProductIcon;
		sapiManager.ActiveChannelsChanged += sapiManager_ActiveChannelsChanged;
		sapiManager.LogFileChannelsChanged += sapiManager_LogFileChannelsChanged;
		sapiManager.EquipmentTypeChanged += sapiManager_EquipmentTypeChanged;
		LicenseManager.GlobalInstance.KeysChanged += LicenseKeysChanged;
		AdapterInformation.GlobalInstance.AdapterUpdated += AdaptersUpdated;
		PrintHelper.SetParent((Control)this);
		this.logFileToLoad = logFileToLoad;
		recentFiles = new RecentFiles(menuRecentFiles, "LogFiles", "RecentLogFiles", 10);
		recentFiles.RecentFileClicked += menuRecentFile_Click;
		mainTabView.FilteredContentChanged += mainTabView_FilteredContentChanged;
	}

	private void AddCaesarComponentInformation()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		ComponentInformation val = new ComponentInformation();
		ComponentInformationGroups.GlobalInstance.Add(Components.GroupSupportedDevices, Resources.ComponentGroup_SupportedDevices, (IComponentInformation)(object)val);
		foreach (IGrouping<string, Ecu> item in from e in (IEnumerable<Ecu>)sapiManager.Sapi.Ecus
			group e by e.Name)
		{
			foreach (Ecu item2 in item)
			{
				val.Add(item2.Name + "_" + item2.DiagnosisSource, item2.Name + ((item.Count() > 1) ? (" (" + Path.GetExtension(item2.DescriptionFileName) + ")") : string.Empty), item2.ConfigurationFileVersion.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0} ({1})", item2.DescriptionDataVersion, item2.ConfigurationFileVersion) : item2.DescriptionDataVersion, true);
			}
		}
		if (((ReadOnlyCollection<Ecu>)(object)sapiManager.Sapi.Ecus).Count == 0)
		{
			val.Add(Components.InfoNotFound, Resources.ComponentInfo_NotFound, string.Empty, true);
		}
		UpdateAdapterInformation();
		IComponentInformation componentsVersionInformation = ComponentInformationGroups.GlobalInstance.GetGroupInformation("Application Components");
		componentsVersionInformation.Add(Components.InfoApplicationType, Resources.ComponentInfo_ApplicationType, Environment.Is64BitProcess ? "64-Bit" : "32-Bit", true);
		componentsVersionInformation.Add(Components.InfoSapi, Resources.ComponentInfo_SAPI, ApplicationInformation.GetAssemblyVersion(typeof(Sapi).Assembly), true);
		componentsVersionInformation.Add(Components.InfoCaesar, Resources.ComponentInfo_Caesar, sapiManager.Sapi.CaesarLibraryVersion + " (" + sapiManager.Sapi.CaesarLibraryCompilationDate + ")", true);
		componentsVersionInformation.Add(Components.InfoRP1210, Resources.ComponentInfo_RP1210, string.Empty, false);
		componentsVersionInformation.Add(Components.InfoMvci, Resources.ComponentInfo_MVCI, string.Empty, false);
		componentsVersionInformation.Add(Components.InfoEthernetPduApi, Resources.ComponentInfo_DoIP_PDU_API, string.Empty, false);
		componentsVersionInformation.Add(Components.InfoGlobals, Resources.ComponentInfo_Globals, string.Empty, false);
		UpdateComponentVersionInformation(componentsVersionInformation);
		sapiManager.SapiResetCompleted += delegate
		{
			UpdateComponentVersionInformation(componentsVersionInformation);
		};
		sapiManager.ChannelInitializingEvent += GlobalInstance_ChannelInitializingEvent;
		string sidDll = Program.GetSidDll();
		if (!string.IsNullOrEmpty(sidDll) && File.Exists(sidDll))
		{
			componentsVersionInformation.Add(Components.InfoSid, Resources.ComponentInfo_Sid, FileVersionInfo.GetVersionInfo(sidDll).FileVersion, true);
		}
	}

	private void GlobalInstance_ChannelInitializingEvent(object sender, ChannelInitializingEventArgs e)
	{
		if (toolStripButtonMute.Checked || toolStripMenuItemMute.Checked)
		{
			e.AutoRead = false;
			e.Channel.EcuInfos.AutoRead = false;
			e.Channel.Parameters.AutoReadSummaryParameters = false;
		}
	}

	private void UpdateComponentVersionInformation(IComponentInformation componentsVersionInformation)
	{
		string text = null;
		string text2 = null;
		string text3 = null;
		bool useMcd = sapiManager.Sapi.UseMcd;
		if (useMcd && sapiManager.McdAvailable)
		{
			text = Sapi.McdSystemDescription;
			int num = text.IndexOf("DtsBaseSystem", StringComparison.OrdinalIgnoreCase);
			if (num != -1)
			{
				text = text.Substring(num);
			}
			text2 = ApplicationInformation.GetLoadedModuleVersion("PDUAPI_Bosch.dll");
			DiagnosisProtocol obj = sapiManager.Sapi.DiagnosisProtocols["UDS_CAN_D"];
			text3 = ((obj != null) ? obj.DescriptionDataVersion : null);
		}
		componentsVersionInformation.UpdateValue(Components.InfoMvci, text ?? (useMcd ? Resources.ComponentInfo_NotAvailable : Resources.ComponentInfo_NotEnabled), useMcd);
		componentsVersionInformation.UpdateValue(Components.InfoEthernetPduApi, text2 ?? (useMcd ? Resources.ComponentInfo_NotAvailable : Resources.ComponentInfo_NotEnabled), useMcd);
		componentsVersionInformation.UpdateValue(Components.InfoGlobals, text3 ?? (useMcd ? Resources.ComponentInfo_NotAvailable : Resources.ComponentInfo_NotEnabled), useMcd);
		IRollCall manager = ChannelCollection.GetManager((Protocol)71993);
		componentsVersionInformation.UpdateValue(Components.InfoRP1210, (manager != null) ? string.Join(" - ", new string[2]
		{
			(manager != null) ? manager.DeviceName : null,
			manager.DeviceLibraryVersion
		}.Where((string s) => s != null)) : "n/a", true);
	}

	private void OnSplashScreenDisposed(object sender, EventArgs e)
	{
		((Component)(object)splashScreen).Disposed -= OnSplashScreenDisposed;
		splashScreen = null;
		mainTabView.SplashProgressBar = (IProgressBar)(object)splashScreen;
	}

	private void LicenseKeysChanged(object sender, EventArgs e)
	{
		UpdateViewMenu();
	}

	private void AdaptersUpdated(object sender, EventArgs e)
	{
		CheckForProhibitedAdapters();
		CheckForBluetoothAdapters();
		UpdateAdapterInformation();
		CheckConnectionHardware();
	}

	private static void UpdateAdapterInformation()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		ComponentInformation val = null;
		if (ComponentInformationGroups.GlobalInstance.GroupIdentifiers.Contains(Components.GroupConfiguredTranslators))
		{
			val = (ComponentInformation)ComponentInformationGroups.GlobalInstance.GetGroupInformation(Components.GroupConfiguredTranslators);
			val.Clear();
		}
		else
		{
			val = new ComponentInformation();
			ComponentInformationGroups.GlobalInstance.Add(Components.GroupConfiguredTranslators, Resources.ComponentGroup_ConfiguredTranslators, (IComponentInformation)(object)val);
		}
		if (AdapterInformation.GlobalInstance == null)
		{
			return;
		}
		foreach (string connectedAdapterName in AdapterInformation.GlobalInstance.ConnectedAdapterNames)
		{
			val.Add(connectedAdapterName, connectedAdapterName, string.Empty, true);
		}
	}

	private void SaveSettings(ISettings settings)
	{
		if (base.WindowState != FormWindowState.Minimized)
		{
			settings.SetValue<FormWindowState>("WindowState", "MainWindow", base.WindowState, false);
		}
		settings.SetValue<Point>("Location", "MainWindow", lastKnownGoodLocation, false);
		settings.SetValue<Size>("Size", "MainWindow", lastKnownGoodSize, false);
	}

	private void LoadSettings(ISettings settings)
	{
		base.WindowState = FormWindowState.Normal;
		Point savedLocation = settings.GetValue<Point>("Location", "MainWindow", base.Location);
		if (!Screen.AllScreens.Any((Screen s) => s.Bounds.Contains(savedLocation)))
		{
			savedLocation = Screen.PrimaryScreen.WorkingArea.Location;
		}
		Rectangle rectangle = new Rectangle(savedLocation, settings.GetValue<Size>("Size", "MainWindow", base.Size));
		rectangle.Intersect(Screen.GetWorkingArea(rectangle));
		base.Bounds = rectangle;
		lastKnownGoodLocation = base.Location;
		lastKnownGoodSize = base.Size;
		base.WindowState = settings.GetValue<FormWindowState>("WindowState", "MainWindow", base.WindowState);
		regularFormBorderStyle = base.FormBorderStyle;
		regularFormWindowState = base.WindowState;
	}

	private void OnMenuExitClick(object sender, EventArgs e)
	{
		Close();
	}

	protected override void OnFormClosing(FormClosingEventArgs e)
	{
		base.OnFormClosing(e);
		if (!e.Cancel)
		{
			e.Cancel = CancelDueToFlashing();
		}
		if (e.Cancel)
		{
			return;
		}
		foreach (Control control in base.Controls)
		{
			control.Enabled = false;
		}
		Application.DoEvents();
		CloseAllConnections();
		Application.DoEvents();
		foreach (ToolStrip item in toolStripContainer.Controls.OfType<ToolStrip>().Union(toolStripContainer2.Controls.OfType<ToolStrip>()))
		{
			item.Items.Clear();
		}
	}

	private bool CancelDueToFlashing()
	{
		bool flag = CurrentlyFlashing();
		if (flag)
		{
			MessageBox.Show(Resources.MessageDeviceBeingFlashedCannotBeTerminatedPleaseWait, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, ControlHelpers.IsRightToLeft((Control)this) ? (MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading) : ((MessageBoxOptions)0));
			flag = true;
		}
		return flag;
	}

	private bool CurrentlyFlashing()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Invalid comparison between Unknown and I4
		bool result = false;
		foreach (Channel item in (ChannelBaseCollection)sapiManager.Sapi.Channels)
		{
			if ((int)item.CommunicationsState == 7)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private void OnMenuUpdateClick(object sender, EventArgs e)
	{
		ServerClient.GlobalInstance.Go((Collection<UnitInformation>)null, (Collection<UnitInformation>)null);
	}

	private void OnMenuOptionsClick(object sender, EventArgs e)
	{
		OptionsDialog optionsDialog = new OptionsDialog();
		optionsDialog.ShowDialog();
		optionsDialog.Dispose();
	}

	private void menuView_ChildClick(object sender, EventArgs e)
	{
		if (sender is ToolStripMenuItem toolStripMenuItem)
		{
			TabbedView tabbedView = mainTabView;
			object tag = toolStripMenuItem.Tag;
			tabbedView.SelectPlace((IPlace)((tag is IPlace) ? tag : null));
		}
	}

	private void OnMenuConnectClick(object sender, EventArgs e)
	{
		using ConnectionDialog connectionDialog = new ConnectionDialog();
		connectionDialog.ShowDialog();
	}

	private void menuView_Popup(object sender, EventArgs e)
	{
		UpdateViewRadio();
	}

	private void UpdateViewRadio()
	{
		foreach (object dropDownItem in menuView.DropDownItems)
		{
			if (dropDownItem is ToolStripMenuItem { Checked: not false } toolStripMenuItem)
			{
				toolStripMenuItem.Checked = false;
				break;
			}
		}
		if (mainTabView.SelectedIndex >= 0 && menuView.DropDownItems[mainTabView.SelectedIndex] is ToolStripMenuItem toolStripMenuItem2)
		{
			toolStripMenuItem2.Checked = true;
		}
	}

	private void mainTabView_Load(object sender, EventArgs e)
	{
		UpdateViewMenu();
	}

	private void UpdateViewMenu()
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (mainTabView.Tabs.Count > 0)
		{
			IEnumerable<ToolStripItem> enumerable = menuView.DropDownItems.Cast<ToolStripItem>().TakeWhile((ToolStripItem item) => !(item is ToolStripSeparator)).ToList();
			foreach (ToolStripItem item in enumerable)
			{
				menuView.DropDownItems.Remove(item);
				item.Dispose();
			}
			for (int num = 0; num < mainTabView.Tabs.Count; num++)
			{
				IPlace val = mainTabView.Tabs[num];
				if (ApplicationInformation.IsViewVisible(val.Identifier))
				{
					ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(val.Title, null, menuView_ChildClick);
					toolStripMenuItem.ShortcutKeys = val.Shortcut;
					toolStripMenuItem.Enabled = LicenseManager.GlobalInstance.AccessLevel >= val.MinAccessLevel;
					toolStripMenuItem.Tag = val;
					menuView.DropDownItems.Insert(num, toolStripMenuItem);
				}
			}
			if (mainTabView.Tabs.Count > 1)
			{
				menuNextView.Enabled = true;
				menuPreviousView.Enabled = true;
			}
		}
		UpdateViewRadio();
	}

	private void menuLogOpen_Click(object sender, EventArgs e)
	{
		LogOpen();
	}

	private void menuLogPlay_Click(object sender, EventArgs e)
	{
		PlayOrPauseLog();
	}

	private void menuRecentFile_Click(object sender, EventArgs e)
	{
		if (!sapiManager.RequestOpenLog())
		{
			return;
		}
		string text = ((ToolStripMenuItem)sender).Text;
		string[] source = new string[2]
		{
			SapiManager.GlobalInstance.CurrentLogFileInformation.SummaryFilePath,
			SapiManager.GlobalInstance.CurrentLogFileInformation.LogFilePath
		};
		if (source.Contains(text))
		{
			MessageBox.Show(Resources.MessageCanNotOpenActiveLog, Resources.CaptionFileAccessError, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, ControlHelpers.IsRightToLeft((Control)this) ? (MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading) : ((MessageBoxOptions)0));
		}
		else if (File.Exists(text))
		{
			LogOpen(text);
			if (logFile != null && logFile.Summary)
			{
				ShowLogFileInfoDialog();
			}
		}
	}

	private void PlayOrPauseLog()
	{
		if (logFile != null)
		{
			if (logFile.Playing)
			{
				LogPause();
			}
			else
			{
				LogPlay();
			}
		}
	}

	private void UpdateSapiManagerStatus(string text)
	{
		statusStripPanelEcuConnection.Text = text;
		statusStrip.Update();
	}

	private void CloseAllConnections()
	{
		sapiManager.CloseAllConnections();
	}

	private void OnCloseClick(object sender, EventArgs e)
	{
		CloseAllConnections();
	}

	private void menuLogStop_Click(object sender, EventArgs e)
	{
		LogStop();
	}

	private void menuLogSeekStart_Click(object sender, EventArgs e)
	{
		LogSeekStart();
	}

	private void menuLogSeekEnd_Click(object sender, EventArgs e)
	{
		LogSeekEnd();
	}

	private void OnSeekTime(object sender, EventArgs e)
	{
		LogSeek();
	}

	private void SetPlaybackSpeed(double speed)
	{
		if (logFile != null)
		{
			logFile.Playing = true;
			logFile.PlaybackSpeed = speed;
		}
	}

	private void menuLogSpeedX1_Click(object sender, EventArgs e)
	{
		SetPlaybackSpeed(1.0);
	}

	private void menuLogSpeedX2_Click(object sender, EventArgs e)
	{
		SetPlaybackSpeed(2.0);
	}

	private void menuLogSpeedX4_Click(object sender, EventArgs e)
	{
		SetPlaybackSpeed(4.0);
	}

	private void menuLogSpeedX8_Click(object sender, EventArgs e)
	{
		SetPlaybackSpeed(8.0);
	}

	private void menuLogSpeedX1B_Click(object sender, EventArgs e)
	{
		SetPlaybackSpeed(-1.0);
	}

	private void menuLogSpeedX2B_Click(object sender, EventArgs e)
	{
		SetPlaybackSpeed(-2.0);
	}

	private void menuLogSpeedX4B_Click(object sender, EventArgs e)
	{
		SetPlaybackSpeed(-4.0);
	}

	private void menuLogSpeedX8B_Click(object sender, EventArgs e)
	{
		SetPlaybackSpeed(-8.0);
	}

	private void Cut()
	{
		editSupport.SetTarget((object)base.ActiveControl);
		editSupport.Cut();
	}

	private void Copy()
	{
		editSupport.SetTarget((object)base.ActiveControl);
		editSupport.Copy();
	}

	private void Paste()
	{
		editSupport.SetTarget((object)base.ActiveControl);
		editSupport.Paste();
	}

	private void Undo()
	{
		editSupport.SetTarget((object)base.ActiveControl);
		editSupport.Undo();
	}

	private void Delete()
	{
		editSupport.SetTarget((object)base.ActiveControl);
		editSupport.Delete();
	}

	private void SelectAll()
	{
		editSupport.SetTarget((object)base.ActiveControl);
		editSupport.SelectAll();
	}

	private void Print()
	{
		if (CanPrint)
		{
			PrintHelper.ShowPrintDialog(mainTabView.Tabs[mainTabView.SelectedIndex].Title, (IProvideHtml)(object)mainTabView, (IncludeInfo)3);
		}
	}

	private void PrintPreview()
	{
		if (CanPrint)
		{
			PrintHelper.ShowPrintPreviewDialog(mainTabView.Tabs[mainTabView.SelectedIndex].Title, (IProvideHtml)(object)mainTabView, (IncludeInfo)3);
		}
	}

	private void LogOpen()
	{
		if (!sapiManager.RequestOpenLog())
		{
			return;
		}
		using OpenLogFileForm openLogFileForm = new OpenLogFileForm();
		if (openLogFileForm.ShowDialog() == DialogResult.OK)
		{
			LogOpen(openLogFileForm.FileName);
			if (logFile != null && logFile.Summary)
			{
				ShowLogFileInfoDialog();
			}
		}
	}

	private void LogOpen(string logFileToLoad)
	{
		if (!string.IsNullOrEmpty(logFileToLoad))
		{
			LogFile val = SapiManager.GlobalInstance.TryLoadLogFile(logFileToLoad, (Control)this);
			if (val != null)
			{
				recentFiles.AddRecentFile(logFileToLoad);
				CloseAllConnections();
				sapiManager.LogFileChannels = val.Channels;
				sapiManager.Online = false;
				UpdateLogSeekBar();
			}
		}
	}

	private void LogPlay()
	{
		if (logFile != null)
		{
			logFile.PlaybackSpeed = 1.0;
			logFile.Playing = true;
		}
	}

	private void LogStop()
	{
		LogSeekStart();
	}

	private void LogSeekStart()
	{
		if (logFile != null)
		{
			LogSeek(logFile.StartTime);
		}
	}

	private void LogSeekEnd()
	{
		if (logFile != null)
		{
			LogSeek(logFile.EndTime);
		}
	}

	private void LogSeek()
	{
		if (logFile == null)
		{
			return;
		}
		using SeekTimeDialog seekTimeDialog = new SeekTimeDialog(logFile);
		if (seekTimeDialog.ShowDialog(this) == DialogResult.OK)
		{
			LogSeek(seekTimeDialog.SelectedTime);
		}
	}

	private void LogSeek(DateTime time)
	{
		if (logFile != null && time >= logFile.StartTime && time <= logFile.EndTime)
		{
			logFile.Playing = false;
			logFile.CurrentTime = time;
		}
	}

	private void LogPause()
	{
		logFile.Playing = false;
	}

	private void LogFastForward()
	{
		if (logFile != null)
		{
			if (logFile.Playing)
			{
				double playbackSpeed = logFile.PlaybackSpeed;
				playbackSpeed = ((!(playbackSpeed <= 0.0)) ? (playbackSpeed * 2.0) : 1.0);
				logFile.PlaybackSpeed = playbackSpeed;
			}
			else
			{
				SetPlaybackSpeed(2.0);
			}
		}
	}

	private void LogRewind()
	{
		if (logFile != null)
		{
			if (logFile.Playing)
			{
				double playbackSpeed = logFile.PlaybackSpeed;
				playbackSpeed = ((!(playbackSpeed >= 0.0)) ? (playbackSpeed * 2.0) : (-1.0));
				logFile.PlaybackSpeed = playbackSpeed;
			}
			else
			{
				SetPlaybackSpeed(-1.0);
			}
		}
	}

	public ToolStrip GetLogFileControls()
	{
		Panel panel = this.toolStrip.Parent as Panel;
		panel.MinimumSize = panel.Size;
		ToolStrip toolStrip = new ToolStrip();
		toolStrip.Items.Add(toolBarButtonPlayPause);
		toolStrip.Items.Add(toolBarButtonStop);
		toolStrip.Items.Add(toolBarButtonSeekStart);
		toolStrip.Items.Add(toolBarButtonRewind);
		toolStrip.Items.Add((ToolStripItem)(object)toolBarTrackBarSeek);
		toolStrip.Items.Add(toolBarButtonFastForward);
		toolStrip.Items.Add(toolBarButtonSeekEnd);
		toolStrip.Items.Add(toolStripButtonSeekTime);
		return toolStrip;
	}

	public void ReplaceLogFileControls()
	{
		if (toolStrip != null && !toolStrip.IsDisposed)
		{
			int index = toolStrip.Items.IndexOf(toolBarButtonOpen) + 1;
			toolStrip.Items.Insert(index, toolStripButtonSeekTime);
			toolStrip.Items.Insert(index, toolBarButtonSeekEnd);
			toolStrip.Items.Insert(index, toolBarButtonFastForward);
			toolStrip.Items.Insert(index, (ToolStripItem)(object)toolBarTrackBarSeek);
			toolStrip.Items.Insert(index, toolBarButtonRewind);
			toolStrip.Items.Insert(index, toolBarButtonSeekStart);
			toolStrip.Items.Insert(index, toolBarButtonStop);
			toolStrip.Items.Insert(index, toolBarButtonPlayPause);
			Panel panel = toolStrip.Parent as Panel;
			panel.MinimumSize = new Size(0, 0);
		}
	}

	private void UpdateUIStatus()
	{
		UpdateCommands();
		UpdateTitleStatus();
	}

	private void UpdateCommands()
	{
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Invalid comparison between Unknown and I4
		//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Invalid comparison between Unknown and I4
		//IL_08c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0915: Unknown result type (might be due to invalid IL or missing references)
		//IL_091a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0931: Unknown result type (might be due to invalid IL or missing references)
		//IL_0936: Unknown result type (might be due to invalid IL or missing references)
		//IL_094d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0952: Unknown result type (might be due to invalid IL or missing references)
		//IL_0969: Unknown result type (might be due to invalid IL or missing references)
		//IL_096e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0985: Unknown result type (might be due to invalid IL or missing references)
		//IL_098a: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c2: Unknown result type (might be due to invalid IL or missing references)
		bool online = sapiManager.Online;
		bool flag = logFile == null;
		bool flag2 = !online && !flag && logFile.Playing;
		double num = (flag ? 0.0 : logFile.PlaybackSpeed);
		bool flag3 = !flag && logFile.CurrentTime == logFile.EndTime;
		bool flag4 = !flag && logFile.CurrentTime == logFile.StartTime;
		logFileInfo.Enabled = !flag;
		if (flag)
		{
			if (((ReadOnlyCollection<Channel>)(object)sapiManager.ActiveChannels).Count == 0)
			{
				menuClose.Enabled = false;
			}
			else
			{
				menuClose.Text = Resources.CommandCloseConnections;
				menuClose.Enabled = true;
			}
		}
		else
		{
			menuClose.Text = Resources.CommandCloseLog;
			menuClose.Enabled = true;
		}
		menuConnect.Enabled = (int)SapiManager.GlobalInstance.Sapi.InitState == 1 && !AdapterInformation.AdapterProhibited;
		toolBarButtonOpen.Enabled = true;
		ToolStripSplitButton toolStripSplitButton = toolBarButtonUserEvent;
		ToolStripMenuItem toolStripMenuItem = menuLogEditableLabel;
		bool flag5 = (menuLogLabel.Enabled = CanLabel);
		bool enabled = (toolStripMenuItem.Enabled = flag5);
		toolStripSplitButton.Enabled = enabled;
		if (CanLabel)
		{
			ToolStripMenuItem toolStripMenuItem2 = menuLogUpload;
			enabled = (toolBarButtonLogUpload.Checked = ServerDataManager.GlobalInstance.IsMarkedForUpload(SapiManager.GlobalInstance.CurrentLogFileInformation.LogFilePath));
			toolStripMenuItem2.Checked = enabled;
			ToolStripMenuItem toolStripMenuItem3 = menuLogUpload;
			enabled = (toolBarButtonLogUpload.Enabled = !menuLogUpload.Checked);
			toolStripMenuItem3.Enabled = enabled;
		}
		else
		{
			ToolStripMenuItem toolStripMenuItem4 = menuLogUpload;
			ToolStripMenuItem toolStripMenuItem5 = menuLogUpload;
			ToolStripButton toolStripButton = toolBarButtonLogUpload;
			bool flag9 = (toolBarButtonLogUpload.Checked = false);
			flag5 = (toolStripButton.Enabled = flag9);
			enabled = (toolStripMenuItem5.Enabled = flag5);
			toolStripMenuItem4.Checked = enabled;
		}
		if (CanLabel)
		{
			ToolStripMenuItem toolStripMenuItem6 = switchDataMiningProcessTagToolStripMenuItem;
			enabled = (toolStripButtonDataMiningTag.Enabled = true);
			toolStripMenuItem6.Enabled = enabled;
			ToolStripMenuItem toolStripMenuItem7 = switchDataMiningProcessTagToolStripMenuItem;
			enabled = (toolStripButtonDataMiningTag.Checked = !SapiManager.DataMiningProcessTag);
			toolStripMenuItem7.Checked = enabled;
			SapiManager.UpdateDataMiningWarning(SapiManager.DataMiningProcessTag);
		}
		else
		{
			ToolStripMenuItem toolStripMenuItem8 = switchDataMiningProcessTagToolStripMenuItem;
			enabled = (toolStripButtonDataMiningTag.Enabled = false);
			toolStripMenuItem8.Enabled = enabled;
			ToolStripMenuItem toolStripMenuItem9 = switchDataMiningProcessTagToolStripMenuItem;
			enabled = (toolStripButtonDataMiningTag.Checked = false);
			toolStripMenuItem9.Checked = enabled;
			SapiManager.UpdateDataMiningWarning(true);
		}
		menuLogPlayPause.Text = ((!flag2) ? Resources.CommandPlay : Resources.CommandPause);
		menuLogPlayPause.Enabled = !online && !flag;
		toolBarButtonPlayPause.Enabled = !online && !flag;
		string text = ((!flag2) ? Resources.TooltipPlay : Resources.TooltipPause);
		if (!string.Equals(toolBarButtonPlayPause.ToolTipText, text))
		{
			toolBarButtonPlayPause.ToolTipText = text;
			if (flag2)
			{
				toolBarButtonPlayPause.Image = Resources.log_pause;
				menuLogPlayPause.Image = Resources.log_pause;
			}
			else
			{
				toolBarButtonPlayPause.Image = Resources.log_play;
				menuLogPlayPause.Image = Resources.log_play;
			}
		}
		menuFile.Enabled = !CurrentlyFlashing();
		menuLogStop.Enabled = !online && (flag2 || (!flag4 && !flag3));
		toolBarButtonStop.Enabled = !online && (flag2 || (!flag4 && !flag3));
		menuLogSpeedX1.Checked = flag2 && num == 1.0;
		menuLogSpeedX2.Checked = flag2 && num == 2.0;
		menuLogSpeedX4.Checked = flag2 && num == 4.0;
		menuLogSpeedX8.Checked = flag2 && num == 8.0;
		menuLogSpeedX1B.Checked = flag2 && num == -1.0;
		menuLogSpeedX2B.Checked = flag2 && num == -2.0;
		menuLogSpeedX4B.Checked = flag2 && num == -4.0;
		menuLogSpeedX8B.Checked = flag2 && num == -8.0;
		menuLogSpeedX1.Enabled = !online && !flag && !flag3;
		menuLogSpeedX2.Enabled = !online && !flag && !flag3;
		menuLogSpeedX4.Enabled = !online && !flag && !flag3;
		menuLogSpeedX8.Enabled = !online && !flag && !flag3;
		menuLogSpeedX1B.Enabled = !online && !flag && !flag4;
		menuLogSpeedX2B.Enabled = !online && !flag && !flag4;
		menuLogSpeedX4B.Enabled = !online && !flag && !flag4;
		menuLogSpeedX8B.Enabled = !online && !flag && !flag4;
		toolBarButtonRewind.Enabled = !online && !flag && !flag4;
		toolBarButtonRewind.Checked = flag2 && num < 0.0;
		toolBarButtonSeekStart.Enabled = !online && !flag && !flag4;
		menuLogSeekStart.Enabled = !online && !flag && !flag4;
		menuLogSeekLabel.Enabled = !online && !flag;
		toolStripButtonSeekTime.Enabled = !online && !flag;
		toolBarButtonFastForward.Enabled = !online && !flag && !flag3;
		toolBarButtonFastForward.Checked = flag2 && num > 1.0;
		toolBarButtonSeekEnd.Enabled = !online && !flag && !flag3;
		menuLogSeekEnd.Enabled = !online && !flag && !flag3;
		((ToolStripItem)(object)toolBarTrackBarSeek).Enabled = !online && !flag;
		if (!flag && !toolBarTrackBarSeek.HasCapture)
		{
			TimeSpan timeSpan = logFile.CurrentTime - logFile.StartTime;
			toolBarTrackBarSeek.Value = Math.Round(timeSpan.TotalSeconds, MidpointRounding.AwayFromZero);
		}
		toolStripButtonRetryAutoConnect.Enabled = (int)SapiManager.GlobalInstance.Sapi.InitState == 1 && SapiManager.GlobalInstance.AutoConnectNeedsRefresh;
		editSupport.SetTarget((object)base.ActiveControl);
		ToolStripButton toolStripButton2 = toolBarButtonCut;
		enabled = (menuCut.Enabled = editSupport.CanCut);
		toolStripButton2.Enabled = enabled;
		ToolStripButton toolStripButton3 = toolBarButtonCopy;
		enabled = (menuCopy.Enabled = editSupport.CanCopy);
		toolStripButton3.Enabled = enabled;
		ToolStripButton toolStripButton4 = toolBarButtonPaste;
		enabled = (menuPaste.Enabled = editSupport.CanPaste);
		toolStripButton4.Enabled = enabled;
		ToolStripButton toolStripButton5 = toolBarButtonUndo;
		enabled = (menuUndo.Enabled = editSupport.CanUndo);
		toolStripButton5.Enabled = enabled;
		menuSelectAll.Enabled = editSupport.CanSelectAll;
		menuDelete.Enabled = editSupport.CanDelete;
		ToolStripButton toolStripButton6 = toolStripButtonPrint;
		ToolStripMenuItem toolStripMenuItem10 = menuPrint;
		flag5 = (menuPrintPreview.Enabled = CanPrint);
		enabled = (toolStripMenuItem10.Enabled = flag5);
		toolStripButton6.Enabled = enabled;
		menuPrintFaults.Enabled = ((IEnumerable<Channel>)sapiManager.ActiveChannels).Count() > 0;
		ToolStripMenuItem toolStripMenuItem11 = refreshToolStripMenuItem;
		enabled = (toolBarButtonRefresh.Enabled = CanRefreshView);
		toolStripMenuItem11.Enabled = enabled;
		toolStripButtonExpandAll.Enabled = CanExpandAllItems;
		toolStripButtonCollapseAll.Enabled = CanCollapseAllItems;
		if (ApplicationInformation.Branding.CanViewUnitsSystemToolBarButton)
		{
			toolStripButtonUnitsSystem.Visible = true;
			toolStripSeparator17.Visible = true;
			toolStripButtonUnitsSystem.Enabled = true;
			UpdateToolStripButtonUnitsSystemTooltip();
		}
		else
		{
			toolStripButtonUnitsSystem.Visible = false;
			toolStripSeparator17.Visible = false;
			toolStripButtonUnitsSystem.Enabled = false;
		}
		menuItemHelp.Enabled = CanShowHelp(HelpAction.Main);
		contextHelpToolStripMenuItem.Enabled = CanShowHelp(HelpAction.Context);
		ToolStripMenuItem toolStripMenuItem12 = menuItemVehicleInformation;
		Link val = LinkSupport.VehicleInformationHome;
		toolStripMenuItem12.Enabled = !((Link)(ref val)).IsBroken;
		ToolStripMenuItem toolStripMenuItem13 = menuItemCatalog;
		val = LinkSupport.PartsCatalog;
		toolStripMenuItem13.Enabled = !((Link)(ref val)).IsBroken;
		ToolStripMenuItem toolStripMenuItem14 = menuItemDDECVIAI;
		val = LinkSupport.DDECVIAIManual;
		toolStripMenuItem14.Enabled = !((Link)(ref val)).IsBroken;
		ToolStripMenuItem toolStripMenuItem15 = menuItemDDEC10AI;
		val = LinkSupport.DDEC10AIManual;
		toolStripMenuItem15.Enabled = !((Link)(ref val)).IsBroken;
		ToolStripMenuItem toolStripMenuItem16 = menuItemFcccEngineSupport;
		val = LinkSupport.FcccEngineSupport;
		toolStripMenuItem16.Enabled = !((Link)(ref val)).IsBroken;
		ToolStripMenuItem toolStripMenuItem17 = menuItemFcccOasis;
		val = LinkSupport.FcccOasis;
		toolStripMenuItem17.Enabled = !((Link)(ref val)).IsBroken;
		ToolStripMenuItem toolStripMenuItem18 = menuItemFcccRVChassis;
		val = LinkSupport.FcccRVChassis;
		toolStripMenuItem18.Enabled = !((Link)(ref val)).IsBroken;
		ToolStripMenuItem toolStripMenuItem19 = menuItemFcccS2B2Chassis;
		val = LinkSupport.FcccS2B2Chassis;
		toolStripMenuItem19.Enabled = !((Link)(ref val)).IsBroken;
		ToolStripMenuItem toolStripMenuItem20 = menuItemFcccEconic;
		val = LinkSupport.FcccEconic;
		toolStripMenuItem20.Enabled = !((Link)(ref val)).IsBroken;
		ToolStripMenuItem toolStripMenuItem21 = menuItemDataMiningReports;
		val = LinkSupport.DataMiningReports;
		toolStripMenuItem21.Enabled = !((Link)(ref val)).IsBroken;
		menuItemZenZefiT.Visible = ApplicationInformation.ShowZenZefiTHelpLink;
		menuUpdate.Enabled = !ServerClient.GlobalInstance.InUse;
	}

	private void UpdateTitleStatus()
	{
		string a = (sapiManager.Online ? ApplicationInformation.ProductName : ((logFile == null) ? string.Format(CultureInfo.CurrentCulture, Resources.FormatTitleOfflineNoLog, ApplicationInformation.ProductName) : string.Format(CultureInfo.CurrentCulture, Resources.FormatTitleOfflineWithLogFile, ApplicationInformation.ProductName, Path.GetFileNameWithoutExtension(logFile.Name))));
		if (!string.Equals(a, title))
		{
			Text = (title = a);
		}
	}

	private void UpdateLogTime()
	{
		if (logFile != null)
		{
			statusStripPanelLogTime.Text = string.Format(CultureInfo.CurrentCulture, Resources.FormatLogTimeStatus, logFile.CurrentTime.ToString(CultureInfo.CurrentCulture));
		}
		else
		{
			statusStripPanelLogTime.Text = string.Empty;
		}
	}

	private void logFile_LogFilePlaybackUpdateEvent(object sender, EventArgs e)
	{
		UpdateLogTime();
	}

	private void sapiManager_LogFileChannelsChanged(object sender, EventArgs e)
	{
		UpdateLogFile();
	}

	private void sapiManager_ActiveChannelsChanged(object sender, EventArgs e)
	{
		UpdateLogFile();
	}

	private void sapiManager_EquipmentTypeChanged(object sender, EquipmentTypeChangedEventArgs e)
	{
		ShowEquipmentTypeSpecificWarnings(e.Category);
	}

	private void UpdateLogFile()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		if (logFile != null)
		{
			logFile.LogFilePlaybackUpdateEvent -= new LogFilePlaybackUpdateEventHandler(logFile_LogFilePlaybackUpdateEvent);
		}
		if (sapiManager.Online)
		{
			logFile = null;
		}
		else if (logFile != sapiManager.LogFileChannels.LogFile)
		{
			logFile = sapiManager.LogFileChannels.LogFile;
			UpdateLogSeekBar();
		}
		if (logFile != null)
		{
			logFile.LogFilePlaybackUpdateEvent += new LogFilePlaybackUpdateEventHandler(logFile_LogFilePlaybackUpdateEvent);
		}
		UpdateLogTime();
	}

	private void ShowLogFileInfoDialog()
	{
		using LogFileInfoDialog logFileInfoDialog = new LogFileInfoDialog(logFile);
		logFileInfoDialog.ShowDialog(this);
	}

	private void UpdateLogSeekBar()
	{
		if (logFile != null)
		{
			toolBarTrackBarSeek.Minimum = 0.0;
			TimeSpan timeSpan = logFile.EndTime - logFile.StartTime;
			toolBarTrackBarSeek.Maximum = Math.Round(timeSpan.TotalSeconds, MidpointRounding.AwayFromZero);
			toolBarTrackBarSeek.SmallChange = toolBarTrackBarSeek.Maximum / 10.0;
			toolBarTrackBarSeek.LargeChange = toolBarTrackBarSeek.Maximum / 5.0;
		}
	}

	protected override void OnSizeChanged(EventArgs e)
	{
		if (base.WindowState == FormWindowState.Normal && !menuFullscreen.Checked)
		{
			lastKnownGoodSize = base.Size;
		}
		base.OnSizeChanged(e);
		ArrangeToolbars();
	}

	protected override void OnLocationChanged(EventArgs e)
	{
		if (base.WindowState == FormWindowState.Normal && !menuFullscreen.Checked)
		{
			lastKnownGoodLocation = base.Location;
		}
		base.OnLocationChanged(e);
	}

	protected override void OnLoad(EventArgs e)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		base.OnLoad(e);
		Application.Idle += Application_Idle;
		Converter.GlobalInstance.SelectedUnitSystem = SapiManager.GlobalInstance.UserUnits;
		LoadSettings((ISettings)(object)SettingsManager.GlobalInstance);
		if (ApplicationInformation.ConnectToServerAutomaticallyAtStartup)
		{
			ServerClient.GlobalInstance.Go((Collection<UnitInformation>)null, (Collection<UnitInformation>)null, (IProgressBar)(object)this);
		}
		else
		{
			ServerClient.GlobalInstance.SetProgressBar((IProgressBar)(object)this);
		}
		using (Form form = new CautionDialog())
		{
			form.ShowDialog(this);
		}
		if (base.IsDisposed)
		{
			return;
		}
		CheckRequiredMinimumPerformance();
		if (base.IsDisposed)
		{
			return;
		}
		CheckForProhibitedAdapters();
		CheckForBluetoothAdapters();
		if (!sapiManager.LogFileIsOpen)
		{
			CheckRollCallStatus();
		}
		SapiManager.GlobalInstance.RollCallEnabledChanged += SapiManager_RollCallEnabledChanged;
		CheckAutoBaudRate();
		CheckConnectionHardware();
		CheckSidVersion();
		if (base.IsDisposed)
		{
			return;
		}
		CheckServerRegistration();
		ServerClient.GlobalInstance.Complete += OnServerClientComplete;
		LargeFileDownloadManager.GlobalInstance.DownloadFilesComplete += GlobalInstance_DownloadFilesComplete;
		DisplayProgrammingEventsAvailabiltyWarning();
		UpdateToolLicenseInformation();
		if (base.IsDisposed)
		{
			return;
		}
		ExtractionManager.GlobalInstance.Init();
		if (ApplicationInformation.CanAccessVehicleInformationLink)
		{
			menuItemVehicleInformation.Visible = true;
		}
		menuItemDataMiningReports.Visible = ApplicationInformation.CanSetDataMiningTag;
		switchDataMiningProcessTagToolStripMenuItem.Visible = ApplicationInformation.CanSetDataMiningTag;
		toolStripButtonDataMiningTag.Visible = ApplicationInformation.CanSetDataMiningTag;
		menuTroubleshootingGuides.Text = Resources.TroubleshootingGuides;
		ITroubleshooting troubleshooting = ((IContainerApplication)this).Troubleshooting;
		menuTroubleshootingGuides.Enabled = troubleshooting != null && troubleshooting.CanViewCollections();
		if (troubleshooting != null)
		{
			troubleshooting.ContentUpdated += troubleshooting_ContentUpdated;
		}
		menuTroubleshootingGuides.Click += OnTroubleshootingGuides;
		sapiManager.StatusCallback = UpdateSapiManagerStatus;
		if (string.IsNullOrEmpty(logFileToLoad))
		{
			sapiManager.RefreshAutoConnect();
		}
		LoadSettingsFilterList();
		if (ApplicationForceUpgrade.ProductUpgradeRequired)
		{
			WarningsPanel.GlobalInstance.Add("ApplicationKill", MessageBoxIcon.Hand, string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_ProductUpdateRequired, ApplicationInformation.ProductName), string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_ProductUpdateRequiredContent, ApplicationForceUpgrade.RequiredUpgradeVersion, ApplicationForceUpgrade.RequiredUpgradeOriginalReleaseDate.ToString("Y", CultureInfo.CurrentCulture), ApplicationForceUpgrade.ForceUpgradeDate.ToLongDateString()), (EventHandler)null);
		}
		if (logFile != null && logFile.Summary)
		{
			ShowLogFileInfoDialog();
			if (base.IsDisposed)
			{
				return;
			}
		}
		ViewHistory.GlobalInstance.HistoryChanged += OnHistoryChanged;
		SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
		if (!string.IsNullOrEmpty(logFileToLoad))
		{
			BeginInvoke((Action)delegate
			{
				LogOpen(logFileToLoad);
			});
		}
	}

	private void GlobalInstance_DownloadFilesComplete(object sender, OperationCompleteEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		if ((int)((LargeFileDownload)((sender is LargeFileDownload) ? sender : null)).LargeFileDownloadType == 1)
		{
			Link.ResetIsBroken();
			menuItemHelp.Enabled = CanShowHelp(HelpAction.Main);
			contextHelpToolStripMenuItem.Enabled = CanShowHelp(HelpAction.Context);
			mainTabView.ContextLinkButton.UpdateState();
		}
	}

	private void troubleshooting_ContentUpdated(object sender, EventArgs e)
	{
		ITroubleshooting troubleshooting = ((IContainerApplication)this).Troubleshooting;
		menuTroubleshootingGuides.Enabled = troubleshooting.CanViewCollections();
	}

	private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
	{
		if (SapiManager.GlobalInstance.Online && (e.Mode == PowerModes.Suspend || e.Mode == PowerModes.Resume))
		{
			CloseAllConnections();
		}
	}

	private void CheckRequiredMinimumPerformance()
	{
		ulong num = 128uL;
		ComputerInfo computerInfo = new ComputerInfo();
		if (computerInfo != null)
		{
			ulong num2 = computerInfo.TotalPhysicalMemory / 1048576;
			if (num2 < ApplicationInformation.ProductMinimumRequiredRam - num)
			{
				WarningMessageBox.WarnMinimumRequiredRam((IWin32Window)this, ApplicationInformation.ProductName);
			}
		}
		IComponentInformation groupInformation = ComponentInformationGroups.GlobalInstance.GetGroupInformation(Components.GroupSystemInformation);
		if (groupInformation.Identifiers.Contains(Components.InfoComputerSystemModel) && groupInformation.Identifiers.Contains(Components.InfoComputerSystemManufacturer))
		{
			string makeModel = groupInformation.GetValue(Components.InfoComputerSystemManufacturer) + " " + groupInformation.GetValue(Components.InfoComputerSystemModel);
			if (ApplicationInformation.ProhibitedSystemMakeModels.Any((string psm) => Regex.Match(makeModel, psm).Value == makeModel))
			{
				WarningMessageBox.Show((IWin32Window)this, string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_WarningSystemModel, makeModel, ApplicationInformation.ProductName), ApplicationInformation.ProductName, "WarningSystemModel");
			}
		}
		if (groupInformation.Identifiers.Contains(Components.InfoProcessorName) && groupInformation.Identifiers.Contains(Components.InfoProcessorMaxClockSpeed) && groupInformation.Identifiers.Contains(Components.InfoProcessorNumberOfCores) && ulong.TryParse(groupInformation.GetValue(Components.InfoProcessorMaxClockSpeed), out var result) && ulong.TryParse(groupInformation.GetValue(Components.InfoProcessorNumberOfCores), out var result2))
		{
			string value = groupInformation.GetValue(Components.InfoProcessorName);
			if (!IsProcessorPerformanceAdequate(value, result2, result))
			{
				WarningMessageBox.Show((IWin32Window)this, string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_WarningProcessor, groupInformation.GetValue(Components.InfoProcessorName), ApplicationInformation.ProductName), ApplicationInformation.ProductName, "WarningProcessor");
			}
		}
	}

	private static bool IsProcessorPerformanceAdequate(string processorName, ulong numberCores, ulong maxClockSpeed)
	{
		if (processorName != null)
		{
			string[] source = new string[3] { "Core(TM) i5", "Core(TM) i7", "Core(TM) i9" };
			if (source.Any((string p) => processorName.IndexOf(p, StringComparison.Ordinal) != -1))
			{
				return true;
			}
		}
		if (numberCores > 2)
		{
			return true;
		}
		return maxClockSpeed >= 2000;
	}

	private void CheckForProhibitedAdapters()
	{
		if (AdapterInformation.AdapterProhibited)
		{
			if (AdapterInformation.AdapterProhibitedByVersion)
			{
				WarningsPanel.GlobalInstance.Add("ProhibitedConnectionHardware", MessageBoxIcon.Hand, string.Empty, string.Format(CultureInfo.CurrentCulture, Resources.Format_ProhibitedAdapterDriverVersionWarning, string.Join(", ", AdapterInformation.GlobalInstance.ConnectedAdapterNames), AdapterInformation.GlobalInstance.SelectedLibraryVersion, ApplicationInformation.ProductName), (EventHandler)null);
			}
			else
			{
				WarningsPanel.GlobalInstance.Add("ProhibitedConnectionHardware", MessageBoxIcon.Hand, string.Empty, string.Format(CultureInfo.CurrentCulture, Resources.Format_ProhibitedAdapterWarning, string.Join(", ", AdapterInformation.GlobalInstance.ConnectedAdapterNames), ApplicationInformation.ProductName), (EventHandler)connectionHardwareWarning_Click);
			}
		}
		else
		{
			WarningsPanel.GlobalInstance.Remove("ProhibitedConnectionHardware");
		}
	}

	private void CheckForBluetoothAdapters()
	{
		string adapterBluetoothPerformanceWarning = AdapterInformation.AdapterBluetoothPerformanceWarning;
		if (!string.IsNullOrEmpty(adapterBluetoothPerformanceWarning))
		{
			MessageBoxIcon messageBoxIcon = (MessageBoxIcon)Enum.Parse(typeof(MessageBoxIcon), adapterBluetoothPerformanceWarning, ignoreCase: true);
			WarningsPanel.GlobalInstance.Add("BluetoothPerformanceWarning", messageBoxIcon, string.Empty, string.Format(CultureInfo.CurrentCulture, Resources.Format_BluetoothPerformanceWarning, string.Join(", ", AdapterInformation.GlobalInstance.ConnectedAdapterNames), ApplicationInformation.ProductName), (EventHandler)connectionHardwareWarning_Click);
		}
		else
		{
			WarningsPanel.GlobalInstance.Remove("BluetoothPerformanceWarning");
		}
	}

	private void CheckRollCallStatus()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		if (!ApplicationInformation.Branding.OmitWarnings && !sapiManager.RollCallEnabled && (int)AdapterInformation.SupportsConcurrentProtocols != 1)
		{
			WarningsPanel.GlobalInstance.Add("RollCallDisabled", MessageBoxIcon.Exclamation, string.Empty, Resources.Message_RollCallDisabled, (EventHandler)connectionHardwareWarning_Click);
		}
		else
		{
			WarningsPanel.GlobalInstance.Remove("RollCallDisabled");
		}
	}

	private void SapiManager_RollCallEnabledChanged(object sender, EventArgs e)
	{
		CheckRollCallStatus();
	}

	private void CheckAutoBaudRate()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		if ((int)AdapterInformation.SupportsAutomaticBaudRate == 1)
		{
			WarningMessageBox.WarnAutoBaudRateUnavailable((IWin32Window)this, ApplicationInformation.ProductName, string.Join(", ", AdapterInformation.GlobalInstance.ConnectedAdapterNames));
		}
	}

	private static void CheckSidVersion()
	{
		string sidDll = Program.GetSidDll();
		if (File.Exists(sidDll) && Version.TryParse(FileVersionInfo.GetVersionInfo(sidDll).FileVersion, out var result))
		{
			Version version = Version.Parse(Assembly.GetExecutingAssembly().GetCustomAttribute<SidVersionAttribute>().Version);
			if (result < version)
			{
				WarningsPanel.GlobalInstance.Add("SidVersion", MessageBoxIcon.Exclamation, string.Empty, string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_SidRollback, version, result), (EventHandler)null);
			}
		}
		else
		{
			WarningsPanel.GlobalInstance.Add("SidVersion", MessageBoxIcon.Hand, string.Empty, Resources.MessageFormat_SidNotFound, (EventHandler)null);
		}
	}

	private void CheckConnectionHardware()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Invalid comparison between Unknown and I4
		if (!AdapterInformation.GlobalInstance.ConnectedAdapterNames.Any())
		{
			WarningsPanel.GlobalInstance.Add("NoConnectionHardware", MessageBoxIcon.Exclamation, string.Empty, string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_NoConnectionHardwareWarning, ApplicationInformation.ProductName), (EventHandler)connectionHardwareWarning_Click);
			return;
		}
		WarningsPanel.GlobalInstance.Remove("NoConnectionHardware");
		if ((int)AdapterInformation.SupportsConcurrentProtocols == 1)
		{
			WarningMessageBox.WarnRollCallDisabled((IWin32Window)this, ApplicationInformation.ProductName);
		}
	}

	private void connectionHardwareWarning_Click(object sender, EventArgs e)
	{
		OptionsDialog optionsDialog = new OptionsDialog(new ConnectionOptionsPanel());
		optionsDialog.ShowDialog();
	}

	private static void DisplayProgrammingEventsAvailabiltyWarning()
	{
		int? num = ((ServerDataManager.GlobalInstance.ToolLicenseInformation != null) ? ServerDataManager.GlobalInstance.ToolLicenseInformation.ToolLicenseTerms.ProgrammingEventsLeft : ((int?)null));
		if (num.HasValue)
		{
			if (num == 0)
			{
				WarningsPanel.GlobalInstance.Add("ProgrammingEventsAllowed", MessageBoxIcon.Exclamation, string.Empty, Resources.NoProgrammingEventsLeft, (EventHandler)null);
			}
			else if (num <= 3)
			{
				WarningsPanel.GlobalInstance.Add("ProgrammingEventsAllowed", MessageBoxIcon.Exclamation, string.Empty, string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_ProgrammingEventsLeft, num), (EventHandler)null);
			}
			else
			{
				WarningsPanel.GlobalInstance.Remove("ProgrammingEventsAllowed");
			}
		}
		else
		{
			WarningsPanel.GlobalInstance.Remove("ProgrammingEventsAllowed");
		}
	}

	private static string FormatDaysRemaining(TimeSpan period)
	{
		int num = Convert.ToInt32(period.TotalDays);
		return (num > 1) ? string.Format(CultureInfo.CurrentCulture, Resources.FormatConnectionRequiredWithnXDays, num) : ((num > 0) ? Resources.ConnectionRequiredWithin1Day : Resources.ConnectionRequiredNow);
	}

	private static void UpdateLastUpdateInformation()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		TimeZone currentTimeZone = TimeZone.CurrentTimeZone;
		IComponentInformation val = null;
		if (!ComponentInformationGroups.GlobalInstance.GroupIdentifiers.Contains(Components.GroupLastUpdateInfo))
		{
			val = (IComponentInformation)new ComponentInformation();
			val.Add(Components.InfoLastServerConnection, Resources.LastUpdateCheckInformation_KeyName, string.Empty, true);
			val.Add(Components.InfoServerConnectionRequired, Resources.InfoDaysTilConnectionExpiration, string.Empty, true);
			ComponentInformationGroups.GlobalInstance.Add(Components.GroupLastUpdateInfo, string.Format(CultureInfo.CurrentCulture, Resources.LastUpdateInformation_Groupname), val);
		}
		string text = string.Format(CultureInfo.CurrentCulture, Resources.FormatLastServerUpdateTimeandTimeZone, ServerClient.GlobalInstance.LastUpdateCheck.ToString(CultureInfo.CurrentCulture), currentTimeZone.DaylightName.ToString(CultureInfo.CurrentCulture));
		val = val ?? ComponentInformationGroups.GlobalInstance.GetGroupInformation(Components.GroupLastUpdateInfo);
		val.UpdateValue(Components.InfoLastServerConnection, text, true);
		TimeSpan period = ServerRegistration.GlobalInstance.ExpirationDate - DateTime.Today;
		val.UpdateValue(Components.InfoServerConnectionRequired, FormatDaysRemaining(period), true);
	}

	private static void UpdateToolLicenseInformation()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		if (ServerDataManager.GlobalInstance.ToolLicenseInformation == null)
		{
			return;
		}
		IComponentInformation val = null;
		if (!ComponentInformationGroups.GlobalInstance.GroupIdentifiers.Contains(Components.GroupToolLicenseExpiration))
		{
			val = (IComponentInformation)new ComponentInformation((IList<ICommand>)((IEnumerable<ICommand>)(object)new ICommand[1] { (ICommand)new ToolLicenseInformationExportToolLicenseCommand() }).ToList());
			val.Add(Components.InfoToolLicenseExpirationDate, Resources.ToolLicenseExpirationDate, string.Empty, true);
			val.Add(Components.InfoProgrammingEventsLeft, Resources.RemainingProgrammingEvents, string.Empty, true);
			ComponentInformationGroups.GlobalInstance.Add(Components.GroupToolLicenseExpiration, string.Format(CultureInfo.CurrentCulture, Resources.ToolLicenseInfo_Groupname), val);
		}
		val = val ?? ComponentInformationGroups.GlobalInstance.GetGroupInformation(Components.GroupToolLicenseExpiration);
		val.UpdateValue(Components.InfoToolLicenseExpirationDate, ServerDataManager.GlobalInstance.ToolLicenseInformation.ToolLicenseTerms.ExpirationDate.ToString(CultureInfo.CurrentCulture), true);
		int? programmingEventsLeft = ServerDataManager.GlobalInstance.ToolLicenseInformation.ToolLicenseTerms.ProgrammingEventsLeft;
		if (programmingEventsLeft.HasValue)
		{
			if (programmingEventsLeft == 9999)
			{
				val.UpdateValue(Components.InfoProgrammingEventsLeft, Resources.Unlimited, true);
			}
			else
			{
				val.UpdateValue(Components.InfoProgrammingEventsLeft, programmingEventsLeft.ToString(), true);
			}
		}
		else
		{
			val.UpdateValue(Components.InfoProgrammingEventsLeft, "0", false);
		}
	}

	private void OnServerClientComplete(object sender, ClientConnectionCompleteEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if ((int)e.Status != 0)
		{
			CheckServerRegistration();
			ServerDataManager.GlobalInstance.AddCrashHandlerInfo();
			IComponentInformation groupInformation = ComponentInformationGroups.GlobalInstance.GetGroupInformation(Components.GroupSystemInformation);
			if (groupInformation != null)
			{
				groupInformation.UpdateValue(Components.InfoComputerDescription, ServerRegistration.GlobalInstance.ComputerDescription, true);
			}
			DisplayProgrammingEventsAvailabiltyWarning();
			UpdateToolLicenseInformation();
		}
	}

	private void CheckServerRegistration()
	{
		ShowServerRegistrationMessage();
		if (!ServerRegistration.GlobalInstance.Valid && !dialoginuse)
		{
			dialoginuse = true;
			if (!ServerRegistrationDialog.ShowRegistrationDialog() && !base.IsDisposed && base.IsHandleCreated)
			{
				BeginInvoke((VoidArgsDelegate)delegate
				{
					Close();
				});
			}
			dialoginuse = false;
		}
		UpdateToolLicenseInformation();
		UpdateLastUpdateInformation();
	}

	private static void ShowEquipmentTypeSpecificWarnings(string category)
	{
		string equipmentTypeSpecificWarnings = ApplicationInformation.GetEquipmentTypeSpecificWarnings(category);
		if (!string.IsNullOrEmpty(equipmentTypeSpecificWarnings))
		{
			WarningsPanel.GlobalInstance.Add("EquipmentTypeSpecificWarning" + category, MessageBoxIcon.None, string.Empty, equipmentTypeSpecificWarnings, (EventHandler)null);
		}
		else
		{
			WarningsPanel.GlobalInstance.Remove("EquipmentTypeSpecificWarning" + category);
		}
	}

	private void ShowServerRegistrationMessage()
	{
		if (ServerRegistration.GlobalInstance.Valid)
		{
			TimeSpan timeSpan = ServerRegistration.GlobalInstance.ToolLicenseExpirationDate - DateTime.Today;
			if (timeSpan.TotalDays < (double)ApplicationInformation.SoftwareLicenseExpirationWarningPeriodDays)
			{
				MessageBoxIcon messageBoxIcon = ((timeSpan.TotalDays <= (double)ApplicationInformation.SoftwareLicenseExpirationErrorPeriodDays) ? MessageBoxIcon.Hand : MessageBoxIcon.Exclamation);
				WarningsPanel.GlobalInstance.Add("ServerRegistrationFixedExpiry", messageBoxIcon, string.Empty, string.Format(CultureInfo.CurrentCulture, ApplicationInformation.RegistrationIsManagedByOrderingSite ? Resources.FormatRegistrationSetToExpireFixedDate : Resources.FormatRegistrationSetToExpireFixedDateInternal, ServerRegistration.GlobalInstance.ToolLicenseExpirationDate.ToLongDateString()), (EventHandler)OnMenuUpdateClick);
			}
			else
			{
				WarningsPanel.GlobalInstance.Remove("ServerRegistrationFixedExpiry");
			}
			TimeSpan timeSpan2 = DateTime.Now - ServerClient.GlobalInstance.LastUpdateCheck;
			if (ServerRegistration.GlobalInstance.InLimitedFunctionalityPeriod)
			{
				TimeSpan period = ServerRegistration.GlobalInstance.CompleteLossOfFunctionalityDate - DateTime.Today;
				WarningsPanel.GlobalInstance.Add("ServerRegistration", MessageBoxIcon.Hand, string.Empty, string.Format(CultureInfo.CurrentCulture, Resources.FormatRegistrationInLossOfFunctionalityMode, Convert.ToInt32(timeSpan2.TotalDays), FormatDaysRemaining(period)), (EventHandler)OnMenuUpdateClick);
			}
			else
			{
				int num = 5;
				if (ServerDataManager.GlobalInstance.ToolLicenseInformation != null && ServerDataManager.GlobalInstance.ToolLicenseInformation.ToolLicenseTerms.ShowWarningOnXDaysBeforeExpiration.HasValue)
				{
					num = ServerDataManager.GlobalInstance.ToolLicenseInformation.ToolLicenseTerms.ShowWarningOnXDaysBeforeExpiration.Value;
				}
				TimeSpan period2 = ServerRegistration.GlobalInstance.ExpirationDate - DateTime.Today;
				if (period2.TotalDays >= 0.0 && period2.TotalDays <= (double)num)
				{
					WarningsPanel.GlobalInstance.Add("ServerRegistration", MessageBoxIcon.Exclamation, string.Empty, string.Format(CultureInfo.CurrentCulture, Resources.FormatRegistrationSetToExpire, FormatDaysRemaining(period2)), (EventHandler)OnMenuUpdateClick);
				}
				else if (timeSpan2.TotalDays > 30.0)
				{
					WarningsPanel.GlobalInstance.Add("ServerRegistration", MessageBoxIcon.Exclamation, string.Empty, string.Format(CultureInfo.CurrentCulture, Resources.FormatLastUpdateCheckWasDaysAgoCheckForUpdates, ApplicationInformation.ProductName, Convert.ToInt32(timeSpan2.TotalDays)), (EventHandler)OnMenuUpdateClick);
				}
				else
				{
					WarningsPanel.GlobalInstance.Remove("ServerRegistration");
				}
			}
			if (!string.IsNullOrEmpty(ServerRegistration.GlobalInstance.UserMessage))
			{
				WarningsPanel.GlobalInstance.Add("UserMessage", MessageBoxIcon.Exclamation, string.Empty, string.Format(CultureInfo.CurrentCulture, ServerRegistration.GlobalInstance.UserMessage), (EventHandler)null);
			}
			else
			{
				WarningsPanel.GlobalInstance.Remove("UserMessage");
			}
			if (!string.IsNullOrEmpty(ServerRegistration.GlobalInstance.NewVersion) && new Version(ServerRegistration.GlobalInstance.NewVersion) > Assembly.GetEntryAssembly().GetName().Version)
			{
				string format = ((!ApplicationInformation.NewVersionDownloadIsManagedByOrderingSite) ? Resources.Format_NewVersionAvailable : Resources.Format_NewVersionAvailableService);
				WarningsPanel.GlobalInstance.Add("NewVersion", MessageBoxIcon.Exclamation, (string)null, string.Format(CultureInfo.CurrentCulture, format, ApplicationInformation.ProductName, ServerRegistration.GlobalInstance.NewVersion), (!ApplicationInformation.NewVersionDownloadIsManagedByOrderingSite) ? null : new EventHandler(warningPanel_NewVersionClick));
			}
			else
			{
				WarningsPanel.GlobalInstance.Remove("NewVersion");
			}
		}
		else
		{
			WarningsPanel.GlobalInstance.Remove("ServerRegistration");
		}
	}

	private void warningPanel_NewVersionClick(object sender, EventArgs e)
	{
		Process.Start(ServerRegistration.GlobalInstance.NewVersionLink);
	}

	protected override void OnFormClosed(FormClosedEventArgs e)
	{
		Application.Idle -= Application_Idle;
		SaveSettings((ISettings)(object)SettingsManager.GlobalInstance);
		base.OnFormClosed(e);
	}

	private void menuAbout_Click(object sender, EventArgs e)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		AboutDialog val = new AboutDialog((IComponentInformationGroups)(object)ComponentInformationGroups.GlobalInstance);
		((Form)(object)val).ShowDialog();
		((Component)(object)val).Dispose();
	}

	private void prevMenu_Click(object sender, EventArgs e)
	{
		mainTabView.SelectPrevious();
	}

	private void nextMenu_Click(object sender, EventArgs e)
	{
		mainTabView.SelectNext();
	}

	private void findNextMenu_Click(object sender, EventArgs e)
	{
		findBar.Find((FindMode)0);
	}

	private void findPreviousMenu_Click(object sender, EventArgs e)
	{
		findBar.Find((FindMode)1);
	}

	private void findMenu_Click(object sender, EventArgs e)
	{
		((Control)(object)findBar).Focus();
	}

	private void Application_Idle(object sender, EventArgs e)
	{
		int num = Environment.TickCount & 0x7FFFFFFF;
		if (num > lastUpdate + 100 && !base.IsDisposed && !CrashHandler.Reporting)
		{
			UpdateUIStatus();
			findBar.UpdateUIStatus();
			lastUpdate = num;
		}
	}

	private void OnTroubleshootingGuides(object sender, EventArgs e)
	{
		ITroubleshooting troubleshooting = ((IContainerApplication)this).Troubleshooting;
		if (troubleshooting != null)
		{
			troubleshooting.StartViewingCollections();
		}
	}

	private void OnMenuPowerTrainGuidesClick(object sender, EventArgs e)
	{
		ITroubleshooting troubleshooting = ((IContainerApplication)this).Troubleshooting;
		if (troubleshooting != null)
		{
			troubleshooting.StartViewingCollections();
		}
	}

	private void OnMenuFaultCodesClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.VehicleFaultCodesTroubleshootingManual);
	}

	private void OnMenuElectricalSystemClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.VehicleElectricalSystemTroubleshootingManual);
	}

	private void menuItemDtnaConnect_Click(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.DtnaConnect);
	}

	private void menuItemZenZefiT_Click(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.ZenZefiT);
	}

	private void OnMenuDataLinkClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.VehicleDataLinkTroubleshootingManual);
	}

	private void OnMenuInstrumentClusterClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.VehicleInstrumentTroubleshootingManual);
	}

	private void OnMenuHvacClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.VehicleHvacTroubleshootingManual);
	}

	private void OnMenuLightingClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.VehicleLightingSystemTroubleshootingManual);
	}

	private void OnMenuDomeLightingClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.VehicleDomeLightingTroubleshootingManual);
	}

	private void OnMenuWindshieldClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.VehicleWindshieldTroubleshootingManual);
	}

	private void OnMenuBackupLightsClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.VehicleBackupLightsTroubleshootingManual);
	}

	private void OnMenuAbsClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.VehicleAbsTroubleshootingManual);
	}

	private void OnMenuPowerSteeringClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.VehiclePowerSteeringTroubleshootingManual);
	}

	private void OnMenuStartingClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.VehicleStartingTroubleshootingManual);
	}

	private void OnMenuClutchClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.VehicleClutchTroubleshootingManual);
	}

	private void OnMenuCruiseControlClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.VehicleCruiseControlTroubleshootingManual);
	}

	private void OnMenuWiringDiagramIClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.VehicleWiringDiagramITroubleshootingManual);
	}

	private void OnMenuWiringDiagramIIClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.VehicleWiringDiagramIITroubleshootingManual);
	}

	public bool Search(string searchText, bool caseSensitive, FindMode direction)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return mainTabView.Search(searchText, caseSensitive, direction);
	}

	private void menuUndo_Click(object sender, EventArgs e)
	{
		Undo();
	}

	private void menuCut_Click(object sender, EventArgs e)
	{
		Cut();
	}

	private void menuCopy_Click(object sender, EventArgs e)
	{
		Copy();
	}

	private void menuPaste_Click(object sender, EventArgs e)
	{
		Paste();
	}

	private void menuDelete_Click(object sender, EventArgs e)
	{
		Delete();
	}

	private void menuSelectAll_Click(object sender, EventArgs e)
	{
		SelectAll();
	}

	private void OnMenuPrintClick(object sender, EventArgs e)
	{
		Print();
	}

	private void OnMenuPrintPreviewClick(object sender, EventArgs e)
	{
		PrintPreview();
	}

	private void OnHelpClick(object sender, EventArgs e)
	{
		ShowHelp(HelpAction.Main);
	}

	private void OnContextHelpClick(object sender, EventArgs e)
	{
		ShowHelp(HelpAction.Context);
	}

	private void ShowHelp(HelpAction action)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (!CanShowHelp(action))
		{
			return;
		}
		switch (action)
		{
		case HelpAction.Main:
			Link.ShowTarget(LinkSupport.MainOnlineHelp);
			break;
		case HelpAction.Context:
			Link.ShowTarget(((ContextHelpControl)mainTabView).ContextLink);
			break;
		case HelpAction.BestAvailable:
			if (CanShowHelp(HelpAction.Context))
			{
				ShowHelp(HelpAction.Context);
			}
			else if (CanShowHelp(HelpAction.Main))
			{
				ShowHelp(HelpAction.Main);
			}
			break;
		default:
			throw new NotImplementedException("Unknown enumeration value");
		}
	}

	private bool CanShowHelp(HelpAction action)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Link val;
		switch (action)
		{
		case HelpAction.Main:
			val = LinkSupport.MainOnlineHelp;
			return !((Link)(ref val)).IsBroken;
		case HelpAction.Context:
			val = ((ContextHelpControl)mainTabView).ContextLink;
			return !((Link)(ref val)).IsBroken;
		case HelpAction.BestAvailable:
			return true;
		default:
			throw new NotImplementedException("Unknown enumeration value");
		}
	}

	private void menuFullscreen_Click(object sender, EventArgs e)
	{
		SetFullscreen(!menuFullscreen.Checked);
	}

	private void menuItemFullScreen_Click(object sender, EventArgs e)
	{
		SetFullscreen(!menuFullscreen.Checked);
	}

	private void SetFullscreen(bool value)
	{
		SuspendLayout();
		if (value)
		{
			menuFullscreen.CheckState = CheckState.Checked;
			regularFormBorderStyle = base.FormBorderStyle;
			regularFormWindowState = base.WindowState;
			base.FormBorderStyle = FormBorderStyle.None;
			base.WindowState = FormWindowState.Minimized;
			base.WindowState = FormWindowState.Maximized;
			BringToFront();
			Activate();
		}
		else
		{
			menuFullscreen.CheckState = CheckState.Unchecked;
			base.FormBorderStyle = regularFormBorderStyle;
			base.WindowState = regularFormWindowState;
		}
		mainTabView.ShowSidebar = !value;
		Focus();
		ArrangeToolbars();
		UpdateConnectionPane();
		ResumeLayout();
	}

	private void ArrangeToolbars()
	{
		if (!menuFullscreen.Checked)
		{
			if (toolStripContainer.Contains(navigationToolStrip))
			{
				toolStripContainer.Controls.Remove(navigationToolStrip);
			}
			if (toolStripContainer.Contains(toolStrip))
			{
				toolStripContainer.Controls.Remove(toolStrip);
			}
			if (toolStripContainer.Contains((Control)(object)findBar))
			{
				toolStripContainer.Controls.Remove((Control)(object)findBar);
			}
			bool flag = false;
			if (!toolStripContainer2.Contains(navigationToolStrip))
			{
				toolStripContainer2.Controls.Add(navigationToolStrip);
				toolStrip.Dock = DockStyle.Fill;
				flag = true;
			}
			if (!toolStripContainer2.Contains(toolStrip))
			{
				toolStripContainer2.Controls.Add(toolStrip);
				toolStrip.Dock = DockStyle.Fill;
				flag = true;
			}
			if (!toolStripContainer2.Contains((Control)(object)findBar))
			{
				toolStripContainer2.Controls.Add((Control)(object)findBar);
				toolStrip.Dock = DockStyle.Fill;
				flag = true;
			}
			if (flag)
			{
				((Control)(object)findBar).BringToFront();
				fullScreenStrip.BringToFront();
				navigationToolStrip.BringToFront();
				menuStrip1.BringToFront();
				toolStrip.BringToFront();
			}
			toolStripContainer2.Visible = true;
			connectionsButton.Visible = false;
			statusSeparator1.Visible = false;
		}
		else
		{
			if (toolStripContainer2.Contains(navigationToolStrip))
			{
				toolStripContainer2.Controls.Remove(navigationToolStrip);
			}
			if (toolStripContainer2.Contains(toolStrip))
			{
				toolStripContainer2.Controls.Remove(toolStrip);
			}
			if (toolStripContainer2.Contains((Control)(object)findBar))
			{
				toolStripContainer2.Controls.Remove((Control)(object)findBar);
			}
			bool flag2 = false;
			if (!toolStripContainer.Contains(navigationToolStrip))
			{
				toolStripContainer.Controls.Add(navigationToolStrip);
				toolStrip.Dock = DockStyle.Right;
				flag2 = true;
			}
			if (!toolStripContainer.Contains(toolStrip))
			{
				toolStripContainer.Controls.Add(toolStrip);
				toolStrip.Dock = DockStyle.Right;
				flag2 = true;
			}
			if (!toolStripContainer.Contains((Control)(object)findBar))
			{
				toolStripContainer.Controls.Add((Control)(object)findBar);
				toolStrip.Dock = DockStyle.Right;
				flag2 = true;
			}
			if (flag2)
			{
				fullScreenStrip.BringToFront();
				((Control)(object)findBar).BringToFront();
				navigationToolStrip.BringToFront();
				menuStrip1.BringToFront();
				toolStrip.BringToFront();
			}
			toolStripContainer2.Visible = false;
			connectionsButton.Visible = true;
			statusSeparator1.Visible = true;
		}
	}

	private void toolStripButtonRetryAutoConnect_Click(object sender, EventArgs e)
	{
		SapiManager.GlobalInstance.RefreshAutoConnect();
	}

	private void toolBarButtonOpen_Click(object sender, EventArgs e)
	{
		LogOpen();
	}

	private void toolBarButtonPlayPause_Click(object sender, EventArgs e)
	{
		PlayOrPauseLog();
	}

	private void toolBarButtonStop_Click(object sender, EventArgs e)
	{
		LogStop();
	}

	private void toolBarButtonSeekStart_Click(object sender, EventArgs e)
	{
		LogSeekStart();
	}

	private void toolBarButtonRewind_Click(object sender, EventArgs e)
	{
		LogRewind();
	}

	private void toolBarButtonFastForward_Click(object sender, EventArgs e)
	{
		LogFastForward();
	}

	private void toolBarButtonSeekEnd_Click(object sender, EventArgs e)
	{
		LogSeekEnd();
	}

	private string MakeLabel()
	{
		labelNumber++;
		return string.Format(CultureInfo.CurrentCulture, Resources.FormatUserEvent, labelNumber);
	}

	private void AddUserEvent(bool showDialog)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		string text = MakeLabel();
		if (showDialog)
		{
			LabelDialog val = new LabelDialog(text);
			try
			{
				if (((Form)(object)val).ShowDialog() != DialogResult.OK)
				{
					return;
				}
				text = val.Label;
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		else
		{
			using SoundPlayer soundPlayer = new SoundPlayer(Resources.button_click);
			soundPlayer.Play();
		}
		sapiManager.Sapi.LogFiles.LabelLog(text);
		StatusLog.Add(new StatusMessage(string.Format(CultureInfo.InvariantCulture, "The user entered a User Event named \"{0}\" into the log file.", text), (StatusMessageType)2, (object)this));
	}

	private void toolBarButtonUserEventClick(object sender, EventArgs e)
	{
		AddUserEvent((Control.ModifierKeys & Keys.Shift) != 0);
	}

	private void UserEventLabelClick(object sender, EventArgs e)
	{
		AddUserEvent(showDialog: false);
	}

	private void MarkLogForUploadClick(object sender, EventArgs e)
	{
		ServerDataManager.GlobalInstance.AddUploadRequest(SapiManager.GlobalInstance.CurrentEngineSerialNumber, SapiManager.GlobalInstance.CurrentVehicleIdentification, SapiManager.GlobalInstance.CurrentLogFileInformation.LogFilePath);
		ControlHelpers.ShowMessageBox((Control)this, Resources.Message_MarkedLogForUpload, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
	}

	private void ShowLogFileInfo(object sender, EventArgs e)
	{
		using LogFileInfoDialog logFileInfoDialog = new LogFileInfoDialog(SapiManager.GlobalInstance.LogFileChannels.LogFile);
		logFileInfoDialog.ShowDialog();
	}

	private void EditableUserEventLabelClick(object sender, EventArgs e)
	{
		AddUserEvent(showDialog: true);
	}

	private void toolStripButtonPrint_Click(object sender, EventArgs e)
	{
		Print();
	}

	private void toolBarButtonCut_Click(object sender, EventArgs e)
	{
		Cut();
	}

	private void toolBarButtonCopy_Click(object sender, EventArgs e)
	{
		Copy();
	}

	private void toolBarButtonPaste_Click(object sender, EventArgs e)
	{
		Paste();
	}

	private void toolBarButtonUndo_Click(object sender, EventArgs e)
	{
		Undo();
	}

	private void OnConnectionsButtonClick(object sender, EventArgs e)
	{
		connectionsButton.Checked = !connectionsButton.Checked;
		UpdateConnectionPane();
	}

	private void UpdateConnectionPane()
	{
		if (!menuFullscreen.Checked)
		{
			return;
		}
		if (connectionsButton.Checked)
		{
			BeginInvoke((Action)delegate
			{
				mainTabView.StatusView.Location = new Point(0, statusStrip.Top - mainTabView.StatusView.Height);
			});
			mainTabView.StatusView.Visible = true;
			mainTabView.StatusView.BringToFront();
		}
		else
		{
			mainTabView.StatusView.Visible = false;
		}
	}

	private void OnSeekTrackBarValueChanged(object sender, ValueChangedEventArgs e)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (logFile != null && (int)e.Action == 0)
		{
			logFile.Playing = false;
			DateTime dateTime = logFile.StartTime + TimeSpan.FromSeconds(toolBarTrackBarSeek.Value);
			if (logFile.CurrentTime != dateTime)
			{
				logFile.CurrentTime = dateTime;
			}
		}
	}

	public void RefreshView()
	{
		mainTabView.RefreshView();
	}

	private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (CanRefreshView)
		{
			RefreshView();
		}
	}

	private void toolBarButtonRefresh_Click(object sender, EventArgs e)
	{
		if (CanRefreshView)
		{
			RefreshView();
		}
	}

	private void OnMenuPartCatalogClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.PartsCatalog);
	}

	private void OnMenuItemDDECVIAIClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.DDECVIAIManual);
	}

	private void OnMenuDDEC10AIClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.DDEC10AIManual);
	}

	private void OnMenuItemFcccEngineSupportClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.FcccEngineSupport);
	}

	private void OnMenuItemFcccOasisClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.FcccOasis);
	}

	private void OnMenuItemFcccRVChassisClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.FcccRVChassis);
	}

	private void OnMenuItemFcccS2B2ChassisClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.FcccS2B2Chassis);
	}

	private void OnMenuItemFcccEconicClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.FcccEconic);
	}

	private void OnMenuDDEC13AIClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.DDEC13AIManual);
	}

	private void OnMenuEuro4AIClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.Euro4AIManual);
	}

	private void menuItemDetroitTransmissionsAIClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.DetroitTransmissionsAIManual);
	}

	private void OnMenuDetroitTransmissionsRGClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.DetroitTransmissionsRGManual);
	}

	private void OnMenuItemDDEC10ReferenceGuideClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.DDEC10ReferenceGuide);
	}

	private void OnMenuItemDDEC13ReferenceGuideClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.DDEC13And16ReferenceGuide);
	}

	private void OnMenuDataMiningReportsClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.DataMiningReports);
	}

	private void OnHistoryBackClick(object sender, EventArgs e)
	{
		ViewHistory.GlobalInstance.GoBack((IContainerApplication)(object)this);
	}

	private void OnHistoryForwardClick(object sender, EventArgs e)
	{
		ViewHistory.GlobalInstance.GoForward((IContainerApplication)(object)this);
	}

	private void OnHistoryChanged(object sender, EventArgs e)
	{
		ToolStripSplitButton toolStripSplitButton = toolStripButtonBack;
		bool enabled = (backToolStripMenuItem.Enabled = ViewHistory.GlobalInstance.BackStack.Any());
		toolStripSplitButton.Enabled = enabled;
		toolStripButtonBack.ToolTipText = (ViewHistory.GlobalInstance.BackStack.Any() ? string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_BackTo0, ViewHistory.GlobalInstance.BackStack.First().DisplayName) : Resources.HistoryBack);
		ToolStripSplitButton toolStripSplitButton2 = toolStripButtonForward;
		enabled = (forwardToolStripMenuItem.Enabled = ViewHistory.GlobalInstance.ForwardStack.Any());
		toolStripSplitButton2.Enabled = enabled;
		toolStripButtonForward.ToolTipText = (ViewHistory.GlobalInstance.ForwardStack.Any() ? string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_FowardTo0, ViewHistory.GlobalInstance.ForwardStack.First().DisplayName) : Resources.HistoryForward);
	}

	private static void AddHistoryItems(IEnumerable<ViewHistoryItem> items, ToolStripItemCollection menu)
	{
		foreach (ViewHistoryItem item in items)
		{
			ToolStripItem toolStripItem = new ToolStripMenuItem(item.DisplayName);
			toolStripItem.Tag = item;
			menu.Add(toolStripItem);
		}
	}

	private void toolStripButtonBack_DropDownOpening(object sender, EventArgs e)
	{
		toolStripButtonBack.DropDownItems.Clear();
		AddHistoryItems(ViewHistory.GlobalInstance.BackStack, toolStripButtonBack.DropDownItems);
	}

	private void toolStripButtonForward_DropDownOpening(object sender, EventArgs e)
	{
		toolStripButtonForward.DropDownItems.Clear();
		AddHistoryItems(ViewHistory.GlobalInstance.ForwardStack, toolStripButtonForward.DropDownItems);
	}

	private void gotoToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
	{
		int num = 0;
		while (num < gotoToolStripMenuItem.DropDownItems.Count)
		{
			ToolStripItem toolStripItem = gotoToolStripMenuItem.DropDownItems[num];
			if (toolStripItem.Tag is ViewHistoryItem)
			{
				gotoToolStripMenuItem.DropDownItems.RemoveAt(num);
				toolStripItem.Dispose();
			}
			else
			{
				num++;
			}
		}
		AddHistoryItems(ViewHistory.GlobalInstance.ForwardStack.Reverse(), gotoToolStripMenuItem.DropDownItems);
		ToolStripItem toolStripItem2 = new ToolStripMenuItem(ViewHistory.GlobalInstance.Current.DisplayName);
		toolStripItem2.Tag = ViewHistory.GlobalInstance.Current;
		toolStripItem2.Font = new Font(toolStripItem2.Font, FontStyle.Bold);
		gotoToolStripMenuItem.DropDownItems.Add(toolStripItem2);
		AddHistoryItems(ViewHistory.GlobalInstance.BackStack, gotoToolStripMenuItem.DropDownItems);
	}

	private void gotoToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
	{
		object tag = e.ClickedItem.Tag;
		ViewHistoryItem val = (ViewHistoryItem)((tag is ViewHistoryItem) ? tag : null);
		if (val != null)
		{
			ViewHistory.GlobalInstance.Navigate((IContainerApplication)(object)this, val);
		}
	}

	private void toolStripButtonForward_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
	{
		_003F val = ViewHistory.GlobalInstance;
		object tag = e.ClickedItem.Tag;
		((ViewHistory)val).Navigate((IContainerApplication)(object)this, (ViewHistoryItem)((tag is ViewHistoryItem) ? tag : null));
	}

	private void toolStripButtonBack_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
	{
		_003F val = ViewHistory.GlobalInstance;
		object tag = e.ClickedItem.Tag;
		((ViewHistory)val).Navigate((IContainerApplication)(object)this, (ViewHistoryItem)((tag is ViewHistoryItem) ? tag : null));
	}

	private void menuFeedBack_Click(object sender, EventArgs e)
	{
		using FeedbackDialog feedbackDialog = new FeedbackDialog();
		feedbackDialog.ShowDialog();
	}

	private void switchDataMiningProcessTagClick(object sender, EventArgs e)
	{
		SapiManager.SwitchDataMiningProcessTag();
	}

	private void OnMenuVehicleInformationClick(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(LinkSupport.VehicleInformationHome);
	}

	private void toolStripButtonFilter_Click(object sender, EventArgs e)
	{
		FilterDialog filterDialog = new FilterDialog();
		filterDialog.FilterList = activeFilterList;
		if (filterDialog.ShowDialog() == DialogResult.OK)
		{
			UpdateFilterList(filterDialog.FilterList);
		}
	}

	private void LoadSettingsFilterList()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		List<FilterTypes> list = mainTabView.Filters.ToList();
		activeFilterList.Clear();
		if (list.Count == 0)
		{
			if (PanelBase.GetFiltered((FilterTypes)1))
			{
				activeFilterList.Add((FilterTypes)1, value: true);
			}
			else
			{
				activeFilterList.Add((FilterTypes)1, value: false);
			}
		}
		else
		{
			foreach (FilterTypes item in list)
			{
				if (PanelBase.GetFiltered(item))
				{
					activeFilterList.Add(item, value: true);
				}
				else
				{
					activeFilterList.Add(item, value: false);
				}
			}
		}
		UpdateFilterButtonHighlight();
	}

	private void UpdateFilterList(Dictionary<FilterTypes, bool> newFilterValues)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<FilterTypes, bool> newFilterValue in newFilterValues)
		{
			if (!activeFilterList.TryGetValue(newFilterValue.Key, out var value))
			{
				activeFilterList.Add(newFilterValue.Key, newFilterValue.Value);
				PanelBase.SetFiltered(newFilterValue.Key, newFilterValue.Value);
			}
			else if (value != newFilterValue.Value)
			{
				activeFilterList[newFilterValue.Key] = newFilterValue.Value;
				PanelBase.SetFiltered(newFilterValue.Key, newFilterValue.Value);
			}
		}
		UpdateFilterButtonHighlight();
	}

	private void UpdateFilterButtonHighlight()
	{
		if (activeFilterList.Values.Contains(value: false))
		{
			toolStripButtonFilter.Image = Resources.funnel_enabled;
		}
		else
		{
			toolStripButtonFilter.Image = Resources.funnel;
		}
		UpdateFilterButtonText();
	}

	private void mainTabView_FilteredContentChanged(object sender, EventArgs e)
	{
		UpdateFilterButtonText();
	}

	private void UpdateFilterButtonText()
	{
		string value = string.Empty;
		IFilterable filterableChild = mainTabView.FilterableChild;
		List<FilterTypes> list = (from x in activeFilterList
			where x.Value
			select x.Key).ToList();
		string toolTipText;
		if (list.Count == activeFilterList.Count)
		{
			toolTipText = Resources.Tooltip_FilterShowingAllContent;
		}
		else
		{
			toolTipText = ((list.Count == 0) ? Resources.Tooltip_FilterShowingOnlyEssentialContent : string.Format(CultureInfo.CurrentCulture, Resources.Tooltip_Format_ShowingSelectedContent, string.Join(", ", list.Select((FilterTypes f) => FilterTypeExtensions.ToDisplayString(f)))));
			if (filterableChild != null)
			{
				value = string.Format(CultureInfo.CurrentCulture, Resources.Format_FilterButtonCount, filterableChild.TotalNumberOfFilterableItems - filterableChild.NumberOfItemsFiltered, filterableChild.TotalNumberOfFilterableItems);
				toolTipText = toolTipText + Environment.NewLine + string.Format(CultureInfo.InvariantCulture, Resources.Tooltip_Format_FilterCountDescription, filterableChild.TotalNumberOfFilterableItems - filterableChild.NumberOfItemsFiltered, filterableChild.TotalNumberOfFilterableItems);
			}
			else
			{
				toolTipText = toolTipText + Environment.NewLine + Resources.Tooltip_FilterCurrentViewDoesNotSupportFiltering;
			}
		}
		if (toolStripButtonFilter != null && !toolStripButtonFilter.IsDisposed)
		{
			toolStripButtonFilter.DisplayStyle = ((!string.IsNullOrEmpty(value)) ? ToolStripItemDisplayStyle.ImageAndText : ToolStripItemDisplayStyle.Image);
			toolStripButtonFilter.Text = value;
			toolStripButtonFilter.ToolTipText = toolTipText;
		}
	}

	public void ExpandAllItems()
	{
		mainTabView.ExpandAllItems();
	}

	public void CollapseAllItems()
	{
		mainTabView.CollapseAllItems();
	}

	private void toolStripButtonExpandAll_Click(object sender, EventArgs e)
	{
		if (CanExpandAllItems)
		{
			ExpandAllItems();
		}
	}

	private void toolStripButtonCollapseAll_Click(object sender, EventArgs e)
	{
		if (CanCollapseAllItems)
		{
			CollapseAllItems();
		}
	}

	private void UpdateToolStripButtonUnitsSystemTooltip()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Invalid comparison between Unknown and I4
		if (toolStripButtonUnitsSystem != null && !toolStripButtonUnitsSystem.IsDisposed)
		{
			toolStripButtonUnitsSystem.ToolTipText = (((int)sapiManager.UserUnits == 1) ? Resources.Tooltip_UnitsSystem_English : Resources.Tootip_UnitsSystem_Metric);
		}
	}

	private void toolStripButtonUnitsSystem_Click(object sender, EventArgs e)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		sapiManager.SetUserUnits((UnitsSystemSelection)(((int)sapiManager.UserUnits != 1) ? 1 : 2));
		UpdateToolStripButtonUnitsSystemTooltip();
		Invalidate(invalidateChildren: true);
	}

	private void toolStripButtonBusMonitor_Click(object sender, EventArgs e)
	{
		BusMonitorForm.Show();
	}

	private void busMonitoringToolStripMenuItem_Click(object sender, EventArgs e)
	{
		BusMonitorForm.Show();
	}

	private void toolStripButtonMute_Click(object sender, EventArgs e)
	{
		ToolStripMenuItem toolStripMenuItem = toolStripMenuItemMute;
		bool flag = (toolStripButtonMute.Checked = !toolStripButtonMute.Checked);
		toolStripMenuItem.Checked = flag;
		ToggleMute(!toolStripButtonMute.Checked);
	}

	private void toolStripMenuItemMute_Click(object sender, EventArgs e)
	{
		ToolStripButton toolStripButton = toolStripButtonMute;
		bool flag = (toolStripMenuItemMute.Checked = !toolStripMenuItemMute.Checked);
		toolStripButton.Checked = flag;
		ToggleMute(!toolStripMenuItemMute.Checked);
	}

	private static void ToggleMute(bool autoRead)
	{
		foreach (Channel item in (ChannelBaseCollection)SapiManager.GlobalInstance.Sapi.Channels)
		{
			InstrumentCollection instruments = item.Instruments;
			FaultCodeCollection faultCodes = item.FaultCodes;
			bool flag = (item.EcuInfos.AutoRead = autoRead);
			bool autoRead2 = (faultCodes.AutoRead = flag);
			instruments.AutoRead = autoRead2;
		}
	}

	private void OnMenuPrintFaultsClick(object sender, EventArgs e)
	{
		if (PrintHelper.Busy)
		{
			return;
		}
		using PrintFaultsDialog printFaultsDialog = new PrintFaultsDialog();
		if (printFaultsDialog.ShowDialog() == DialogResult.OK)
		{
			PrintHelper.ShowPrintDialog("Faults", (IProvideHtml)(object)printFaultsDialog, (IncludeInfo)3);
		}
	}
}
