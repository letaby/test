// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.MainForm
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

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
using System.Windows.Forms.Layout;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal sealed class MainForm : 
  Form,
  IContainerApplication,
  ISearchable,
  IRefreshable,
  IProgressBar,
  ISupportExpandCollapseAll
{
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
      return this.statusStripProgressBarServerConnection == null ? 0 : this.statusStripProgressBarServerConnection.Maximum;
    }
    set
    {
      if (this.statusStripProgressBarServerConnection == null)
        return;
      if (value > 0)
        this.statusStripProgressBarServerConnection.Maximum = value;
      this.statusStripProgressBarServerConnection.Visible = value > 0;
    }
  }

  int IProgressBar.Value
  {
    get
    {
      return this.statusStripProgressBarServerConnection == null ? 0 : this.statusStripProgressBarServerConnection.Value;
    }
    set
    {
      if (this.statusStripProgressBarServerConnection == null || value > this.statusStripProgressBarServerConnection.Maximum)
        return;
      this.statusStripProgressBarServerConnection.Value = value;
      this.statusStrip.Update();
    }
  }

  public string OverallStatusText
  {
    get => this.overallStatusText;
    set
    {
      this.overallStatusText = value;
      this.UpdateProgressFromStatus();
    }
  }

  public string ComponentStatusText
  {
    get => this.componentStatusText;
    set
    {
      this.componentStatusText = value;
      this.UpdateProgressFromStatus();
    }
  }

  public Control MarshalControl
  {
    get => (Control) this.statusStripPanelServerConnection.GetCurrentParent();
  }

  public void AllDone(bool success, bool canceled, string message)
  {
    this.componentStatusText = string.Empty;
    this.overallStatusText = message;
    if (this.statusStripProgressBarServerConnection != null)
      this.statusStripProgressBarServerConnection.Visible = false;
    this.UpdateProgressFromStatus();
    this.serverProgressTimer.Tick += new EventHandler(this.serverProgressTimer_Tick);
    this.serverProgressTimer.Interval = 5000;
    this.serverProgressTimer.Enabled = true;
  }

  private void serverProgressTimer_Tick(object sender, EventArgs e)
  {
    this.serverProgressTimer.Tick -= new EventHandler(this.serverProgressTimer_Tick);
    this.serverProgressTimer.Enabled = false;
    this.statusStripPanelServerConnection.Text = string.Empty;
  }

  private void UpdateProgressFromStatus()
  {
    if (!string.IsNullOrEmpty(this.componentStatusText))
      this.statusStripPanelServerConnection.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatConnectProgressWithCompStatus, (object) NetworkSettings.GlobalInstance.Server, (object) this.overallStatusText, (object) this.componentStatusText);
    else
      this.statusStripPanelServerConnection.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatConnectProgressWithoutCompStatus, (object) NetworkSettings.GlobalInstance.Server, (object) this.overallStatusText);
    this.statusStrip.Update();
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing)
    {
      while (this.toolStrip.Items.Count > 0)
        this.toolStrip.Items.RemoveAt(0);
      while (this.fullScreenStrip.Items.Count > 0)
        this.fullScreenStrip.Items.RemoveAt(0);
      while (this.navigationToolStrip.Items.Count > 0)
        this.navigationToolStrip.Items.RemoveAt(0);
      PrintHelper.DisposeInstance();
      if (this.components != null)
        this.components.Dispose();
    }
    base.Dispose(disposing);
    if (!disposing)
      return;
    this.sapiManager.SaveSettings();
    this.sapiManager.Dispose();
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new System.ComponentModel.Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (MainForm));
    this.menuUndo = new ToolStripMenuItem();
    this.menuCut = new ToolStripMenuItem();
    this.menuCopy = new ToolStripMenuItem();
    this.menuPaste = new ToolStripMenuItem();
    this.menuDelete = new ToolStripMenuItem();
    this.menuSelectAll = new ToolStripMenuItem();
    this.menuUpdate = new ToolStripMenuItem();
    this.menuToolsSeperator1 = new ToolStripSeparator();
    this.menuLogPlayPause = new ToolStripMenuItem();
    this.menuLogStop = new ToolStripMenuItem();
    this.menuLogSpeedX1 = new ToolStripMenuItem();
    this.menuLogSpeedX2 = new ToolStripMenuItem();
    this.menuLogSpeedX4 = new ToolStripMenuItem();
    this.menuLogSpeedX8 = new ToolStripMenuItem();
    this.menuLogSpeedX1B = new ToolStripMenuItem();
    this.menuLogSpeedX2B = new ToolStripMenuItem();
    this.menuLogSpeedX4B = new ToolStripMenuItem();
    this.menuLogSpeedX8B = new ToolStripMenuItem();
    this.menuLogSeekStart = new ToolStripMenuItem();
    this.menuLogSeekEnd = new ToolStripMenuItem();
    this.menuLogSeekLabel = new ToolStripMenuItem();
    this.menuLogLabel = new ToolStripMenuItem();
    this.menuLogEditableLabel = new ToolStripMenuItem();
    this.menuLogUpload = new ToolStripMenuItem();
    this.switchDataMiningProcessTagToolStripMenuItem = new ToolStripMenuItem();
    this.infoLogSplitter = new ToolStripSeparator();
    this.logFileInfo = new ToolStripMenuItem();
    this.menuTroubleshootingGuides = new ToolStripMenuItem();
    this.menuItemHelp = new ToolStripMenuItem();
    this.contextHelpToolStripMenuItem = new ToolStripMenuItem();
    this.menuItemReferences = new ToolStripMenuItem();
    this.menuItemDtnaConnect = new ToolStripMenuItem();
    this.menuItemCatalog = new ToolStripMenuItem();
    this.menuItemVehicleInformation = new ToolStripMenuItem();
    this.menuItemDataMiningReports = new ToolStripMenuItem();
    this.menuItemAandIManual = new ToolStripMenuItem();
    this.menuItemDDECVIAI = new ToolStripMenuItem();
    this.menuItemDDEC10AI = new ToolStripMenuItem();
    this.menuItemDDEC13AI = new ToolStripMenuItem();
    this.menuItemEuro4AI = new ToolStripMenuItem();
    this.menuItemDetroitTransmissionsAI = new ToolStripMenuItem();
    this.menuItemFcccEngines = new ToolStripMenuItem();
    this.menuItemFcccEngineSupport = new ToolStripMenuItem();
    this.menuItemFcccOasis = new ToolStripMenuItem();
    this.menuItemFcccRVChassis = new ToolStripMenuItem();
    this.menuItemFcccS2B2Chassis = new ToolStripMenuItem();
    this.menuItemFcccEconic = new ToolStripMenuItem();
    this.menuItemReferenceGuide = new ToolStripMenuItem();
    this.menuItemDDEC10RefrenceGuide = new ToolStripMenuItem();
    this.menuItemDDEC13RefrenceGuide = new ToolStripMenuItem();
    this.menuItemDetroitTransmissionsRG = new ToolStripMenuItem();
    this.toolStripSeparator8 = new ToolStripSeparator();
    this.menuFeedBack = new ToolStripMenuItem();
    this.toolStripSeparator12 = new ToolStripSeparator();
    this.menuConnect = new ToolStripMenuItem();
    this.menuClose = new ToolStripMenuItem();
    this.menuPrint = new ToolStripMenuItem();
    this.menuPrintPreview = new ToolStripMenuItem();
    this.menuRecentFiles = new ToolStripMenuItem();
    this.toolStrip = new ToolStrip();
    this.imageListToolStrip = new ImageList(this.components);
    this.toolStripButtonRetryAutoConnect = new ToolStripButton();
    this.toolStripButtonBusMonitor = new ToolStripButton();
    this.toolStripButtonMute = new ToolStripButton();
    this.toolStripSeparator1 = new ToolStripSeparator();
    this.toolBarButtonOpen = new ToolStripButton();
    this.toolBarButtonPlayPause = new ToolStripButton();
    this.toolBarButtonStop = new ToolStripButton();
    this.toolBarButtonSeekStart = new ToolStripButton();
    this.toolBarButtonRewind = new ToolStripButton();
    this.toolBarTrackBarSeek = new ToolStripTrackBar();
    this.toolBarButtonFastForward = new ToolStripButton();
    this.toolBarButtonSeekEnd = new ToolStripButton();
    this.toolStripButtonSeekTime = new ToolStripButton();
    this.toolBarButtonUserEvent = new ToolStripSplitButton();
    this.toolBarMenuItemLabel = new ToolStripMenuItem();
    this.toolBarMenuItemEditableLabel = new ToolStripMenuItem();
    this.toolBarButtonLogUpload = new ToolStripButton();
    this.toolStripButtonDataMiningTag = new ToolStripButton();
    this.toolStripSeparator2 = new ToolStripSeparator();
    this.toolStripButtonPrint = new ToolStripButton();
    this.toolStripSeparator3 = new ToolStripSeparator();
    this.toolBarButtonCut = new ToolStripButton();
    this.toolBarButtonCopy = new ToolStripButton();
    this.toolBarButtonPaste = new ToolStripButton();
    this.toolStripSeparator16 = new ToolStripSeparator();
    this.toolBarButtonUndo = new ToolStripButton();
    this.toolStripSeparator4 = new ToolStripSeparator();
    this.toolStripButtonFilter = new ToolStripButton();
    this.toolStripSeparator7 = new ToolStripSeparator();
    this.toolBarButtonRefresh = new ToolStripButton();
    this.toolStripSeparator6 = new ToolStripSeparator();
    this.toolStripButtonUnitsSystem = new ToolStripButton();
    this.toolStripSeparator17 = new ToolStripSeparator();
    this.toolStripButtonExpandAll = new ToolStripButton();
    this.toolStripButtonCollapseAll = new ToolStripButton();
    this.menuStrip1 = new MenuStrip();
    this.menuFile = new ToolStripMenuItem();
    this.menuPrintFaults = new ToolStripMenuItem();
    this.menuView = new ToolStripMenuItem();
    this.menuViewSplit2 = new ToolStripSeparator();
    this.menuNextView = new ToolStripMenuItem();
    this.menuPreviousView = new ToolStripMenuItem();
    this.toolStripSeparator9 = new ToolStripSeparator();
    this.gotoToolStripMenuItem = new ToolStripMenuItem();
    this.backToolStripMenuItem = new ToolStripMenuItem();
    this.forwardToolStripMenuItem = new ToolStripMenuItem();
    this.toolStripSeparator10 = new ToolStripSeparator();
    this.toolStripSeparator11 = new ToolStripSeparator();
    this.toolStripSeparator14 = new ToolStripSeparator();
    this.filterToolStripMenuItem = new ToolStripMenuItem();
    this.menuViewSplit1 = new ToolStripSeparator();
    this.busMonitoringToolStripMenuItem = new ToolStripMenuItem();
    this.toolStripMenuItemMute = new ToolStripMenuItem();
    this.toolStripSeparator15 = new ToolStripSeparator();
    this.refreshToolStripMenuItem = new ToolStripMenuItem();
    this.fullScreenStrip = new ToolStrip();
    this.menuFullscreen = new ToolStripButton();
    this.findBar = new FindBar();
    this.statusStrip = new StatusStrip();
    this.connectionsButton = new ConnectionStatusButton();
    this.statusSeparator1 = new ToolStripSeparator();
    this.statusStripPanelLogTime = new ToolStripStatusLabel();
    this.statusStripPanelEcuConnection = new ToolStripStatusLabel();
    this.statusStripPanelServerConnection = new ToolStripStatusLabel();
    this.statusStripProgressBarServerConnection = new ToolStripProgressBar();
    this.toolStripContainer = new Panel();
    this.toolStripContainer2 = new Panel();
    this.navigationToolStrip = new ToolStrip();
    this.toolStripButtonBack = new ToolStripSplitButton();
    this.toolStripButtonForward = new ToolStripSplitButton();
    this.mainTabView = new TabbedView();
    this.menuItemZenZefiT = new ToolStripMenuItem();
    ToolStripMenuItem toolStripMenuItem1 = new ToolStripMenuItem();
    ToolStripSeparator toolStripSeparator1 = new ToolStripSeparator();
    ToolStripSeparator toolStripSeparator2 = new ToolStripSeparator();
    ToolStripSeparator toolStripSeparator3 = new ToolStripSeparator();
    ToolStripSeparator toolStripSeparator4 = new ToolStripSeparator();
    ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem();
    ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem();
    ToolStripSeparator toolStripSeparator5 = new ToolStripSeparator();
    ToolStripSeparator toolStripSeparator6 = new ToolStripSeparator();
    ToolStripMenuItem toolStripMenuItem4 = new ToolStripMenuItem();
    ToolStripMenuItem toolStripMenuItem5 = new ToolStripMenuItem();
    ToolStripMenuItem toolStripMenuItem6 = new ToolStripMenuItem();
    ToolStripMenuItem toolStripMenuItem7 = new ToolStripMenuItem();
    ToolStripSeparator toolStripSeparator7 = new ToolStripSeparator();
    ToolStripMenuItem toolStripMenuItem8 = new ToolStripMenuItem();
    ToolStripMenuItem toolStripMenuItem9 = new ToolStripMenuItem();
    ToolStripMenuItem toolStripMenuItem10 = new ToolStripMenuItem();
    ToolStripMenuItem toolStripMenuItem11 = new ToolStripMenuItem();
    ToolStripMenuItem toolStripMenuItem12 = new ToolStripMenuItem();
    ToolStripMenuItem toolStripMenuItem13 = new ToolStripMenuItem();
    ToolStripMenuItem toolStripMenuItem14 = new ToolStripMenuItem();
    ToolStripMenuItem toolStripMenuItem15 = new ToolStripMenuItem();
    ToolStripSeparator toolStripSeparator8 = new ToolStripSeparator();
    ToolStripMenuItem toolStripMenuItem16 = new ToolStripMenuItem();
    ToolStripSeparator toolStripSeparator9 = new ToolStripSeparator();
    ToolStripSeparator toolStripSeparator10 = new ToolStripSeparator();
    ToolStripMenuItem toolStripMenuItem17 = new ToolStripMenuItem();
    this.toolStrip.SuspendLayout();
    this.menuStrip1.SuspendLayout();
    this.fullScreenStrip.SuspendLayout();
    this.statusStrip.SuspendLayout();
    this.toolStripContainer.SuspendLayout();
    this.toolStripContainer2.SuspendLayout();
    this.navigationToolStrip.SuspendLayout();
    this.SuspendLayout();
    toolStripMenuItem1.Image = (Image) Resources.open_log;
    toolStripMenuItem1.Name = "menuLogOpen";
    componentResourceManager.ApplyResources((object) toolStripMenuItem1, "menuLogOpen");
    toolStripMenuItem1.Click += new EventHandler(this.menuLogOpen_Click);
    toolStripSeparator1.Name = "menuFileSplit1";
    componentResourceManager.ApplyResources((object) toolStripSeparator1, "menuFileSplit1");
    toolStripSeparator2.Name = "menuFileSplit2";
    componentResourceManager.ApplyResources((object) toolStripSeparator2, "menuFileSplit2");
    toolStripSeparator3.Name = "toolStripSeparator13";
    componentResourceManager.ApplyResources((object) toolStripSeparator3, "toolStripSeparator13");
    toolStripSeparator4.Name = "menuFileSplit3";
    componentResourceManager.ApplyResources((object) toolStripSeparator4, "menuFileSplit3");
    toolStripMenuItem2.Name = "menuExit";
    componentResourceManager.ApplyResources((object) toolStripMenuItem2, "menuExit");
    toolStripMenuItem2.Click += new EventHandler(this.OnMenuExitClick);
    toolStripMenuItem3.DropDownItems.AddRange(new ToolStripItem[11]
    {
      (ToolStripItem) this.menuUndo,
      (ToolStripItem) toolStripSeparator5,
      (ToolStripItem) this.menuCut,
      (ToolStripItem) this.menuCopy,
      (ToolStripItem) this.menuPaste,
      (ToolStripItem) this.menuDelete,
      (ToolStripItem) toolStripSeparator6,
      (ToolStripItem) this.menuSelectAll,
      (ToolStripItem) toolStripMenuItem4,
      (ToolStripItem) toolStripMenuItem5,
      (ToolStripItem) toolStripMenuItem6
    });
    toolStripMenuItem3.Name = "menuEdit";
    toolStripMenuItem3.Overflow = ToolStripItemOverflow.AsNeeded;
    componentResourceManager.ApplyResources((object) toolStripMenuItem3, "menuEdit");
    componentResourceManager.ApplyResources((object) this.menuUndo, "menuUndo");
    this.menuUndo.Image = (Image) Resources.undo;
    this.menuUndo.Name = "menuUndo";
    this.menuUndo.Click += new EventHandler(this.menuUndo_Click);
    toolStripSeparator5.Name = "editMenuSplit1";
    componentResourceManager.ApplyResources((object) toolStripSeparator5, "editMenuSplit1");
    componentResourceManager.ApplyResources((object) this.menuCut, "menuCut");
    this.menuCut.Image = (Image) Resources.cut;
    this.menuCut.Name = "menuCut";
    this.menuCut.Click += new EventHandler(this.menuCut_Click);
    componentResourceManager.ApplyResources((object) this.menuCopy, "menuCopy");
    this.menuCopy.Image = (Image) Resources.copy;
    this.menuCopy.Name = "menuCopy";
    this.menuCopy.Click += new EventHandler(this.menuCopy_Click);
    componentResourceManager.ApplyResources((object) this.menuPaste, "menuPaste");
    this.menuPaste.Image = (Image) Resources.paste;
    this.menuPaste.Name = "menuPaste";
    this.menuPaste.Click += new EventHandler(this.menuPaste_Click);
    componentResourceManager.ApplyResources((object) this.menuDelete, "menuDelete");
    this.menuDelete.Name = "menuDelete";
    this.menuDelete.Click += new EventHandler(this.menuDelete_Click);
    toolStripSeparator6.Name = "editMenuSplit2";
    componentResourceManager.ApplyResources((object) toolStripSeparator6, "editMenuSplit2");
    componentResourceManager.ApplyResources((object) this.menuSelectAll, "menuSelectAll");
    this.menuSelectAll.Name = "menuSelectAll";
    this.menuSelectAll.Click += new EventHandler(this.menuSelectAll_Click);
    toolStripMenuItem4.Image = (Image) Resources.find_next;
    toolStripMenuItem4.Name = "findNextMenu";
    componentResourceManager.ApplyResources((object) toolStripMenuItem4, "findNextMenu");
    toolStripMenuItem4.Click += new EventHandler(this.findNextMenu_Click);
    toolStripMenuItem5.Image = (Image) Resources.find_previous;
    toolStripMenuItem5.Name = "findPreviousMenu";
    componentResourceManager.ApplyResources((object) toolStripMenuItem5, "findPreviousMenu");
    toolStripMenuItem5.Click += new EventHandler(this.findPreviousMenu_Click);
    toolStripMenuItem6.Name = "findMenu";
    componentResourceManager.ApplyResources((object) toolStripMenuItem6, "findMenu");
    toolStripMenuItem6.Click += new EventHandler(this.findMenu_Click);
    componentResourceManager.ApplyResources((object) toolStripMenuItem7, "menuNoViews");
    toolStripMenuItem7.Name = "menuNoViews";
    toolStripSeparator7.Name = "menuLogSplitter";
    componentResourceManager.ApplyResources((object) toolStripSeparator7, "menuLogSplitter");
    toolStripMenuItem8.DropDownItems.AddRange(new ToolStripItem[3]
    {
      (ToolStripItem) this.menuUpdate,
      (ToolStripItem) this.menuToolsSeperator1,
      (ToolStripItem) toolStripMenuItem9
    });
    toolStripMenuItem8.Name = "menuTools";
    toolStripMenuItem8.Overflow = ToolStripItemOverflow.AsNeeded;
    componentResourceManager.ApplyResources((object) toolStripMenuItem8, "menuTools");
    this.menuUpdate.Name = "menuUpdate";
    componentResourceManager.ApplyResources((object) this.menuUpdate, "menuUpdate");
    this.menuUpdate.Click += new EventHandler(this.OnMenuUpdateClick);
    this.menuToolsSeperator1.Name = "menuToolsSeperator1";
    componentResourceManager.ApplyResources((object) this.menuToolsSeperator1, "menuToolsSeperator1");
    toolStripMenuItem9.Name = "menuOptions";
    componentResourceManager.ApplyResources((object) toolStripMenuItem9, "menuOptions");
    toolStripMenuItem9.Click += new EventHandler(this.OnMenuOptionsClick);
    toolStripMenuItem10.DropDownItems.AddRange(new ToolStripItem[11]
    {
      (ToolStripItem) this.menuLogPlayPause,
      (ToolStripItem) this.menuLogStop,
      (ToolStripItem) toolStripMenuItem11,
      (ToolStripItem) toolStripMenuItem14,
      (ToolStripItem) toolStripSeparator7,
      (ToolStripItem) this.menuLogLabel,
      (ToolStripItem) this.menuLogEditableLabel,
      (ToolStripItem) this.menuLogUpload,
      (ToolStripItem) this.switchDataMiningProcessTagToolStripMenuItem,
      (ToolStripItem) this.infoLogSplitter,
      (ToolStripItem) this.logFileInfo
    });
    toolStripMenuItem10.Name = "menuLog";
    toolStripMenuItem10.Overflow = ToolStripItemOverflow.AsNeeded;
    componentResourceManager.ApplyResources((object) toolStripMenuItem10, "menuLog");
    this.menuLogPlayPause.Image = (Image) Resources.log_play;
    this.menuLogPlayPause.Name = "menuLogPlayPause";
    componentResourceManager.ApplyResources((object) this.menuLogPlayPause, "menuLogPlayPause");
    this.menuLogPlayPause.Click += new EventHandler(this.menuLogPlay_Click);
    this.menuLogStop.Image = (Image) Resources.log_stop;
    this.menuLogStop.Name = "menuLogStop";
    componentResourceManager.ApplyResources((object) this.menuLogStop, "menuLogStop");
    this.menuLogStop.Click += new EventHandler(this.menuLogStop_Click);
    toolStripMenuItem11.DropDownItems.AddRange(new ToolStripItem[2]
    {
      (ToolStripItem) toolStripMenuItem12,
      (ToolStripItem) toolStripMenuItem13
    });
    toolStripMenuItem11.Name = "menuLogSpeed";
    componentResourceManager.ApplyResources((object) toolStripMenuItem11, "menuLogSpeed");
    toolStripMenuItem12.DropDownItems.AddRange(new ToolStripItem[4]
    {
      (ToolStripItem) this.menuLogSpeedX1,
      (ToolStripItem) this.menuLogSpeedX2,
      (ToolStripItem) this.menuLogSpeedX4,
      (ToolStripItem) this.menuLogSpeedX8
    });
    toolStripMenuItem12.Name = "menuFastForward";
    componentResourceManager.ApplyResources((object) toolStripMenuItem12, "menuFastForward");
    this.menuLogSpeedX1.Name = "menuLogSpeedX1";
    componentResourceManager.ApplyResources((object) this.menuLogSpeedX1, "menuLogSpeedX1");
    this.menuLogSpeedX1.Click += new EventHandler(this.menuLogSpeedX1_Click);
    this.menuLogSpeedX2.Name = "menuLogSpeedX2";
    componentResourceManager.ApplyResources((object) this.menuLogSpeedX2, "menuLogSpeedX2");
    this.menuLogSpeedX2.Click += new EventHandler(this.menuLogSpeedX2_Click);
    this.menuLogSpeedX4.Name = "menuLogSpeedX4";
    componentResourceManager.ApplyResources((object) this.menuLogSpeedX4, "menuLogSpeedX4");
    this.menuLogSpeedX4.Click += new EventHandler(this.menuLogSpeedX4_Click);
    this.menuLogSpeedX8.Name = "menuLogSpeedX8";
    componentResourceManager.ApplyResources((object) this.menuLogSpeedX8, "menuLogSpeedX8");
    this.menuLogSpeedX8.Click += new EventHandler(this.menuLogSpeedX8_Click);
    toolStripMenuItem13.DropDownItems.AddRange(new ToolStripItem[4]
    {
      (ToolStripItem) this.menuLogSpeedX1B,
      (ToolStripItem) this.menuLogSpeedX2B,
      (ToolStripItem) this.menuLogSpeedX4B,
      (ToolStripItem) this.menuLogSpeedX8B
    });
    toolStripMenuItem13.Name = "menuRewind";
    componentResourceManager.ApplyResources((object) toolStripMenuItem13, "menuRewind");
    this.menuLogSpeedX1B.Name = "menuLogSpeedX1B";
    componentResourceManager.ApplyResources((object) this.menuLogSpeedX1B, "menuLogSpeedX1B");
    this.menuLogSpeedX1B.Click += new EventHandler(this.menuLogSpeedX1B_Click);
    this.menuLogSpeedX2B.Name = "menuLogSpeedX2B";
    componentResourceManager.ApplyResources((object) this.menuLogSpeedX2B, "menuLogSpeedX2B");
    this.menuLogSpeedX2B.Click += new EventHandler(this.menuLogSpeedX2B_Click);
    this.menuLogSpeedX4B.Name = "menuLogSpeedX4B";
    componentResourceManager.ApplyResources((object) this.menuLogSpeedX4B, "menuLogSpeedX4B");
    this.menuLogSpeedX4B.Click += new EventHandler(this.menuLogSpeedX4B_Click);
    this.menuLogSpeedX8B.Name = "menuLogSpeedX8B";
    componentResourceManager.ApplyResources((object) this.menuLogSpeedX8B, "menuLogSpeedX8B");
    this.menuLogSpeedX8B.Click += new EventHandler(this.menuLogSpeedX8B_Click);
    toolStripMenuItem14.DropDownItems.AddRange(new ToolStripItem[3]
    {
      (ToolStripItem) this.menuLogSeekStart,
      (ToolStripItem) this.menuLogSeekEnd,
      (ToolStripItem) this.menuLogSeekLabel
    });
    toolStripMenuItem14.Name = "menuLogSeek";
    componentResourceManager.ApplyResources((object) toolStripMenuItem14, "menuLogSeek");
    this.menuLogSeekStart.Image = (Image) Resources.log_seek_start;
    this.menuLogSeekStart.Name = "menuLogSeekStart";
    componentResourceManager.ApplyResources((object) this.menuLogSeekStart, "menuLogSeekStart");
    this.menuLogSeekStart.Click += new EventHandler(this.menuLogSeekStart_Click);
    this.menuLogSeekEnd.Image = (Image) Resources.log_seek_end;
    this.menuLogSeekEnd.Name = "menuLogSeekEnd";
    componentResourceManager.ApplyResources((object) this.menuLogSeekEnd, "menuLogSeekEnd");
    this.menuLogSeekEnd.Click += new EventHandler(this.menuLogSeekEnd_Click);
    this.menuLogSeekLabel.Image = (Image) Resources.seek_time;
    this.menuLogSeekLabel.Name = "menuLogSeekLabel";
    componentResourceManager.ApplyResources((object) this.menuLogSeekLabel, "menuLogSeekLabel");
    this.menuLogSeekLabel.Click += new EventHandler(this.OnSeekTime);
    this.menuLogLabel.Image = (Image) Resources.add_user_event;
    this.menuLogLabel.Name = "menuLogLabel";
    componentResourceManager.ApplyResources((object) this.menuLogLabel, "menuLogLabel");
    this.menuLogLabel.Click += new EventHandler(this.UserEventLabelClick);
    this.menuLogEditableLabel.Name = "menuLogEditableLabel";
    componentResourceManager.ApplyResources((object) this.menuLogEditableLabel, "menuLogEditableLabel");
    this.menuLogEditableLabel.Click += new EventHandler(this.EditableUserEventLabelClick);
    this.menuLogUpload.Image = (Image) Resources.upload_log;
    this.menuLogUpload.Name = "menuLogUpload";
    componentResourceManager.ApplyResources((object) this.menuLogUpload, "menuLogUpload");
    this.menuLogUpload.Click += new EventHandler(this.MarkLogForUploadClick);
    componentResourceManager.ApplyResources((object) this.switchDataMiningProcessTagToolStripMenuItem, "switchDataMiningProcessTagToolStripMenuItem");
    this.switchDataMiningProcessTagToolStripMenuItem.Image = (Image) Resources.summaryfiles_noupload;
    this.switchDataMiningProcessTagToolStripMenuItem.Name = "switchDataMiningProcessTagToolStripMenuItem";
    this.switchDataMiningProcessTagToolStripMenuItem.Click += new EventHandler(this.switchDataMiningProcessTagClick);
    this.infoLogSplitter.Name = "infoLogSplitter";
    componentResourceManager.ApplyResources((object) this.infoLogSplitter, "infoLogSplitter");
    componentResourceManager.ApplyResources((object) this.logFileInfo, "logFileInfo");
    this.logFileInfo.Name = "logFileInfo";
    this.logFileInfo.Click += new EventHandler(this.ShowLogFileInfo);
    toolStripMenuItem15.Name = "menuItemFullScreen";
    componentResourceManager.ApplyResources((object) toolStripMenuItem15, "menuItemFullScreen");
    toolStripMenuItem15.Click += new EventHandler(this.menuItemFullScreen_Click);
    toolStripSeparator8.Name = "toolStripSeparator5";
    componentResourceManager.ApplyResources((object) toolStripSeparator8, "toolStripSeparator5");
    toolStripMenuItem16.DropDownItems.AddRange(new ToolStripItem[10]
    {
      (ToolStripItem) this.menuTroubleshootingGuides,
      (ToolStripItem) toolStripSeparator9,
      (ToolStripItem) this.menuItemHelp,
      (ToolStripItem) this.contextHelpToolStripMenuItem,
      (ToolStripItem) toolStripSeparator10,
      (ToolStripItem) this.menuItemReferences,
      (ToolStripItem) this.toolStripSeparator8,
      (ToolStripItem) this.menuFeedBack,
      (ToolStripItem) this.toolStripSeparator12,
      (ToolStripItem) toolStripMenuItem17
    });
    toolStripMenuItem16.Name = "menuHelp";
    toolStripMenuItem16.Overflow = ToolStripItemOverflow.AsNeeded;
    componentResourceManager.ApplyResources((object) toolStripMenuItem16, "menuHelp");
    componentResourceManager.ApplyResources((object) this.menuTroubleshootingGuides, "menuTroubleshootingGuides");
    this.menuTroubleshootingGuides.Name = "menuTroubleshootingGuides";
    toolStripSeparator9.Name = "menuHelpSeparator1";
    componentResourceManager.ApplyResources((object) toolStripSeparator9, "menuHelpSeparator1");
    this.menuItemHelp.Name = "menuItemHelp";
    componentResourceManager.ApplyResources((object) this.menuItemHelp, "menuItemHelp");
    this.menuItemHelp.Click += new EventHandler(this.OnHelpClick);
    this.contextHelpToolStripMenuItem.Name = "contextHelpToolStripMenuItem";
    componentResourceManager.ApplyResources((object) this.contextHelpToolStripMenuItem, "contextHelpToolStripMenuItem");
    this.contextHelpToolStripMenuItem.Click += new EventHandler(this.OnContextHelpClick);
    toolStripSeparator10.Name = "menuHelpSeperator2";
    componentResourceManager.ApplyResources((object) toolStripSeparator10, "menuHelpSeperator2");
    this.menuItemReferences.DropDownItems.AddRange(new ToolStripItem[8]
    {
      (ToolStripItem) this.menuItemDtnaConnect,
      (ToolStripItem) this.menuItemCatalog,
      (ToolStripItem) this.menuItemVehicleInformation,
      (ToolStripItem) this.menuItemDataMiningReports,
      (ToolStripItem) this.menuItemZenZefiT,
      (ToolStripItem) this.menuItemAandIManual,
      (ToolStripItem) this.menuItemFcccEngines,
      (ToolStripItem) this.menuItemReferenceGuide
    });
    this.menuItemReferences.Name = "menuItemReferences";
    componentResourceManager.ApplyResources((object) this.menuItemReferences, "menuItemReferences");
    this.menuItemDtnaConnect.Name = "menuItemDtnaConnect";
    componentResourceManager.ApplyResources((object) this.menuItemDtnaConnect, "menuItemDtnaConnect");
    this.menuItemDtnaConnect.Click += new EventHandler(this.menuItemDtnaConnect_Click);
    this.menuItemCatalog.Name = "menuItemCatalog";
    componentResourceManager.ApplyResources((object) this.menuItemCatalog, "menuItemCatalog");
    this.menuItemCatalog.Click += new EventHandler(this.OnMenuPartCatalogClick);
    this.menuItemVehicleInformation.Name = "menuItemVehicleInformation";
    componentResourceManager.ApplyResources((object) this.menuItemVehicleInformation, "menuItemVehicleInformation");
    this.menuItemVehicleInformation.Click += new EventHandler(this.OnMenuVehicleInformationClick);
    this.menuItemDataMiningReports.Name = "menuItemDataMiningReports";
    componentResourceManager.ApplyResources((object) this.menuItemDataMiningReports, "menuItemDataMiningReports");
    this.menuItemDataMiningReports.Click += new EventHandler(this.OnMenuDataMiningReportsClick);
    this.menuItemAandIManual.DropDownItems.AddRange(new ToolStripItem[5]
    {
      (ToolStripItem) this.menuItemDDECVIAI,
      (ToolStripItem) this.menuItemDDEC10AI,
      (ToolStripItem) this.menuItemDDEC13AI,
      (ToolStripItem) this.menuItemEuro4AI,
      (ToolStripItem) this.menuItemDetroitTransmissionsAI
    });
    this.menuItemAandIManual.Name = "menuItemAandIManual";
    componentResourceManager.ApplyResources((object) this.menuItemAandIManual, "menuItemAandIManual");
    this.menuItemDDECVIAI.Name = "menuItemDDECVIAI";
    componentResourceManager.ApplyResources((object) this.menuItemDDECVIAI, "menuItemDDECVIAI");
    this.menuItemDDECVIAI.Click += new EventHandler(this.OnMenuItemDDECVIAIClick);
    this.menuItemDDEC10AI.Name = "menuItemDDEC10AI";
    componentResourceManager.ApplyResources((object) this.menuItemDDEC10AI, "menuItemDDEC10AI");
    this.menuItemDDEC10AI.Click += new EventHandler(this.OnMenuDDEC10AIClick);
    this.menuItemDDEC13AI.Name = "menuItemDDEC13AI";
    componentResourceManager.ApplyResources((object) this.menuItemDDEC13AI, "menuItemDDEC13AI");
    this.menuItemDDEC13AI.Click += new EventHandler(this.OnMenuDDEC13AIClick);
    this.menuItemEuro4AI.Name = "menuItemEuro4AI";
    componentResourceManager.ApplyResources((object) this.menuItemEuro4AI, "menuItemEuro4AI");
    this.menuItemEuro4AI.Click += new EventHandler(this.OnMenuEuro4AIClick);
    this.menuItemDetroitTransmissionsAI.Name = "menuItemDetroitTransmissionsAI";
    componentResourceManager.ApplyResources((object) this.menuItemDetroitTransmissionsAI, "menuItemDetroitTransmissionsAI");
    this.menuItemDetroitTransmissionsAI.Click += new EventHandler(this.menuItemDetroitTransmissionsAIClick);
    this.menuItemFcccEngines.DropDownItems.AddRange(new ToolStripItem[5]
    {
      (ToolStripItem) this.menuItemFcccEngineSupport,
      (ToolStripItem) this.menuItemFcccOasis,
      (ToolStripItem) this.menuItemFcccRVChassis,
      (ToolStripItem) this.menuItemFcccS2B2Chassis,
      (ToolStripItem) this.menuItemFcccEconic
    });
    this.menuItemFcccEngines.Name = "menuItemFcccEngines";
    componentResourceManager.ApplyResources((object) this.menuItemFcccEngines, "menuItemFcccEngines");
    this.menuItemFcccEngineSupport.Name = "menuItemFcccEngineSupport";
    componentResourceManager.ApplyResources((object) this.menuItemFcccEngineSupport, "menuItemFcccEngineSupport");
    this.menuItemFcccEngineSupport.Click += new EventHandler(this.OnMenuItemFcccEngineSupportClick);
    this.menuItemFcccOasis.Name = "menuItemFcccOasis";
    componentResourceManager.ApplyResources((object) this.menuItemFcccOasis, "menuItemFcccOasis");
    this.menuItemFcccOasis.Click += new EventHandler(this.OnMenuItemFcccOasisClick);
    this.menuItemFcccRVChassis.Name = "menuItemFcccRVChassis";
    componentResourceManager.ApplyResources((object) this.menuItemFcccRVChassis, "menuItemFcccRVChassis");
    this.menuItemFcccRVChassis.Click += new EventHandler(this.OnMenuItemFcccRVChassisClick);
    this.menuItemFcccS2B2Chassis.Name = "menuItemFcccS2B2Chassis";
    componentResourceManager.ApplyResources((object) this.menuItemFcccS2B2Chassis, "menuItemFcccS2B2Chassis");
    this.menuItemFcccS2B2Chassis.Click += new EventHandler(this.OnMenuItemFcccS2B2ChassisClick);
    this.menuItemFcccEconic.Name = "menuItemFcccEconic";
    componentResourceManager.ApplyResources((object) this.menuItemFcccEconic, "menuItemFcccEconic");
    this.menuItemFcccEconic.Click += new EventHandler(this.OnMenuItemFcccEconicClick);
    this.menuItemReferenceGuide.DropDownItems.AddRange(new ToolStripItem[3]
    {
      (ToolStripItem) this.menuItemDDEC10RefrenceGuide,
      (ToolStripItem) this.menuItemDDEC13RefrenceGuide,
      (ToolStripItem) this.menuItemDetroitTransmissionsRG
    });
    this.menuItemReferenceGuide.Name = "menuItemReferenceGuide";
    componentResourceManager.ApplyResources((object) this.menuItemReferenceGuide, "menuItemReferenceGuide");
    this.menuItemDDEC10RefrenceGuide.Name = "menuItemDDEC10RefrenceGuide";
    componentResourceManager.ApplyResources((object) this.menuItemDDEC10RefrenceGuide, "menuItemDDEC10RefrenceGuide");
    this.menuItemDDEC10RefrenceGuide.Click += new EventHandler(this.OnMenuItemDDEC10ReferenceGuideClick);
    this.menuItemDDEC13RefrenceGuide.Name = "menuItemDDEC13RefrenceGuide";
    componentResourceManager.ApplyResources((object) this.menuItemDDEC13RefrenceGuide, "menuItemDDEC13RefrenceGuide");
    this.menuItemDDEC13RefrenceGuide.Click += new EventHandler(this.OnMenuItemDDEC13ReferenceGuideClick);
    this.menuItemDetroitTransmissionsRG.Name = "menuItemDetroitTransmissionsRG";
    componentResourceManager.ApplyResources((object) this.menuItemDetroitTransmissionsRG, "menuItemDetroitTransmissionsRG");
    this.menuItemDetroitTransmissionsRG.Click += new EventHandler(this.OnMenuDetroitTransmissionsRGClick);
    this.toolStripSeparator8.Name = "toolStripSeparator8";
    componentResourceManager.ApplyResources((object) this.toolStripSeparator8, "toolStripSeparator8");
    this.menuFeedBack.Name = "menuFeedBack";
    componentResourceManager.ApplyResources((object) this.menuFeedBack, "menuFeedBack");
    this.menuFeedBack.Click += new EventHandler(this.menuFeedBack_Click);
    this.toolStripSeparator12.Name = "toolStripSeparator12";
    componentResourceManager.ApplyResources((object) this.toolStripSeparator12, "toolStripSeparator12");
    toolStripMenuItem17.Name = "menuAbout";
    componentResourceManager.ApplyResources((object) toolStripMenuItem17, "menuAbout");
    toolStripMenuItem17.Click += new EventHandler(this.menuAbout_Click);
    this.menuConnect.Name = "menuConnect";
    componentResourceManager.ApplyResources((object) this.menuConnect, "menuConnect");
    this.menuConnect.Click += new EventHandler(this.OnMenuConnectClick);
    this.menuClose.Name = "menuClose";
    componentResourceManager.ApplyResources((object) this.menuClose, "menuClose");
    this.menuClose.Click += new EventHandler(this.OnCloseClick);
    this.menuPrint.Image = (Image) Resources.print;
    this.menuPrint.Name = "menuPrint";
    componentResourceManager.ApplyResources((object) this.menuPrint, "menuPrint");
    this.menuPrint.Click += new EventHandler(this.OnMenuPrintClick);
    this.menuPrintPreview.Name = "menuPrintPreview";
    componentResourceManager.ApplyResources((object) this.menuPrintPreview, "menuPrintPreview");
    this.menuPrintPreview.Click += new EventHandler(this.OnMenuPrintPreviewClick);
    componentResourceManager.ApplyResources((object) this.menuRecentFiles, "menuRecentFiles");
    this.menuRecentFiles.Name = "menuRecentFiles";
    componentResourceManager.ApplyResources((object) this.toolStrip, "toolStrip");
    this.toolStrip.GripStyle = ToolStripGripStyle.Hidden;
    this.toolStrip.ImageList = this.imageListToolStrip;
    this.toolStrip.Items.AddRange(new ToolStripItem[34]
    {
      (ToolStripItem) this.toolStripButtonRetryAutoConnect,
      (ToolStripItem) this.toolStripButtonBusMonitor,
      (ToolStripItem) this.toolStripButtonMute,
      (ToolStripItem) this.toolStripSeparator1,
      (ToolStripItem) this.toolBarButtonOpen,
      (ToolStripItem) this.toolBarButtonPlayPause,
      (ToolStripItem) this.toolBarButtonStop,
      (ToolStripItem) this.toolBarButtonSeekStart,
      (ToolStripItem) this.toolBarButtonRewind,
      (ToolStripItem) this.toolBarTrackBarSeek,
      (ToolStripItem) this.toolBarButtonFastForward,
      (ToolStripItem) this.toolBarButtonSeekEnd,
      (ToolStripItem) this.toolStripButtonSeekTime,
      (ToolStripItem) toolStripSeparator8,
      (ToolStripItem) this.toolBarButtonUserEvent,
      (ToolStripItem) this.toolBarButtonLogUpload,
      (ToolStripItem) this.toolStripButtonDataMiningTag,
      (ToolStripItem) this.toolStripSeparator2,
      (ToolStripItem) this.toolStripButtonPrint,
      (ToolStripItem) this.toolStripSeparator3,
      (ToolStripItem) this.toolBarButtonCut,
      (ToolStripItem) this.toolBarButtonCopy,
      (ToolStripItem) this.toolBarButtonPaste,
      (ToolStripItem) this.toolStripSeparator16,
      (ToolStripItem) this.toolBarButtonUndo,
      (ToolStripItem) this.toolStripSeparator4,
      (ToolStripItem) this.toolStripButtonFilter,
      (ToolStripItem) this.toolStripSeparator7,
      (ToolStripItem) this.toolBarButtonRefresh,
      (ToolStripItem) this.toolStripSeparator6,
      (ToolStripItem) this.toolStripButtonUnitsSystem,
      (ToolStripItem) this.toolStripSeparator17,
      (ToolStripItem) this.toolStripButtonExpandAll,
      (ToolStripItem) this.toolStripButtonCollapseAll
    });
    this.toolStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
    this.toolStrip.Name = "toolStrip";
    this.toolStrip.TabStop = true;
    this.imageListToolStrip.ColorDepth = ColorDepth.Depth32Bit;
    componentResourceManager.ApplyResources((object) this.imageListToolStrip, "imageListToolStrip");
    this.imageListToolStrip.TransparentColor = System.Drawing.Color.Transparent;
    componentResourceManager.ApplyResources((object) this.toolStripButtonRetryAutoConnect, "toolStripButtonRetryAutoConnect");
    this.toolStripButtonRetryAutoConnect.Image = (Image) Resources.retry_autoconnect;
    this.toolStripButtonRetryAutoConnect.Name = "toolStripButtonRetryAutoConnect";
    this.toolStripButtonRetryAutoConnect.Click += new EventHandler(this.toolStripButtonRetryAutoConnect_Click);
    this.toolStripButtonBusMonitor.DisplayStyle = ToolStripItemDisplayStyle.Image;
    this.toolStripButtonBusMonitor.Image = (Image) Resources.environment_view;
    componentResourceManager.ApplyResources((object) this.toolStripButtonBusMonitor, "toolStripButtonBusMonitor");
    this.toolStripButtonBusMonitor.Name = "toolStripButtonBusMonitor";
    this.toolStripButtonBusMonitor.Click += new EventHandler(this.toolStripButtonBusMonitor_Click);
    this.toolStripButtonMute.DisplayStyle = ToolStripItemDisplayStyle.Image;
    this.toolStripButtonMute.Image = (Image) Resources.cd_pause;
    componentResourceManager.ApplyResources((object) this.toolStripButtonMute, "toolStripButtonMute");
    this.toolStripButtonMute.Name = "toolStripButtonMute";
    this.toolStripButtonMute.Click += new EventHandler(this.toolStripButtonMute_Click);
    this.toolStripSeparator1.Name = "toolStripSeparator1";
    componentResourceManager.ApplyResources((object) this.toolStripSeparator1, "toolStripSeparator1");
    componentResourceManager.ApplyResources((object) this.toolBarButtonOpen, "toolBarButtonOpen");
    this.toolBarButtonOpen.Image = (Image) Resources.open_log;
    this.toolBarButtonOpen.Name = "toolBarButtonOpen";
    this.toolBarButtonOpen.Click += new EventHandler(this.toolBarButtonOpen_Click);
    componentResourceManager.ApplyResources((object) this.toolBarButtonPlayPause, "toolBarButtonPlayPause");
    this.toolBarButtonPlayPause.Image = (Image) Resources.log_play;
    this.toolBarButtonPlayPause.Name = "toolBarButtonPlayPause";
    this.toolBarButtonPlayPause.Click += new EventHandler(this.toolBarButtonPlayPause_Click);
    componentResourceManager.ApplyResources((object) this.toolBarButtonStop, "toolBarButtonStop");
    this.toolBarButtonStop.Image = (Image) Resources.log_stop;
    this.toolBarButtonStop.Name = "toolBarButtonStop";
    this.toolBarButtonStop.Click += new EventHandler(this.toolBarButtonStop_Click);
    componentResourceManager.ApplyResources((object) this.toolBarButtonSeekStart, "toolBarButtonSeekStart");
    this.toolBarButtonSeekStart.Image = (Image) Resources.log_seek_start;
    this.toolBarButtonSeekStart.Name = "toolBarButtonSeekStart";
    this.toolBarButtonSeekStart.Click += new EventHandler(this.toolBarButtonSeekStart_Click);
    componentResourceManager.ApplyResources((object) this.toolBarButtonRewind, "toolBarButtonRewind");
    this.toolBarButtonRewind.Image = (Image) Resources.log_rewind;
    this.toolBarButtonRewind.Name = "toolBarButtonRewind";
    this.toolBarButtonRewind.Click += new EventHandler(this.toolBarButtonRewind_Click);
    ((ToolStripItem) this.toolBarTrackBarSeek).BackColor = System.Drawing.Color.Transparent;
    this.toolBarTrackBarSeek.Maximum = 10.0;
    ((ToolStripItem) this.toolBarTrackBarSeek).Name = "toolBarTrackBarSeek";
    componentResourceManager.ApplyResources((object) this.toolBarTrackBarSeek, "toolBarTrackBarSeek");
    this.toolBarTrackBarSeek.ValueChanged += new EventHandler<ValueChangedEventArgs>(this.OnSeekTrackBarValueChanged);
    componentResourceManager.ApplyResources((object) this.toolBarButtonFastForward, "toolBarButtonFastForward");
    this.toolBarButtonFastForward.Image = (Image) Resources.log_fast_forward;
    this.toolBarButtonFastForward.Name = "toolBarButtonFastForward";
    this.toolBarButtonFastForward.Click += new EventHandler(this.toolBarButtonFastForward_Click);
    componentResourceManager.ApplyResources((object) this.toolBarButtonSeekEnd, "toolBarButtonSeekEnd");
    this.toolBarButtonSeekEnd.Image = (Image) Resources.log_seek_end;
    this.toolBarButtonSeekEnd.Name = "toolBarButtonSeekEnd";
    this.toolBarButtonSeekEnd.Click += new EventHandler(this.toolBarButtonSeekEnd_Click);
    this.toolStripButtonSeekTime.DisplayStyle = ToolStripItemDisplayStyle.Image;
    componentResourceManager.ApplyResources((object) this.toolStripButtonSeekTime, "toolStripButtonSeekTime");
    this.toolStripButtonSeekTime.Image = (Image) Resources.seek_time;
    this.toolStripButtonSeekTime.Name = "toolStripButtonSeekTime";
    this.toolStripButtonSeekTime.Click += new EventHandler(this.OnSeekTime);
    this.toolBarButtonUserEvent.DisplayStyle = ToolStripItemDisplayStyle.Image;
    this.toolBarButtonUserEvent.DropDownItems.AddRange(new ToolStripItem[2]
    {
      (ToolStripItem) this.toolBarMenuItemLabel,
      (ToolStripItem) this.toolBarMenuItemEditableLabel
    });
    componentResourceManager.ApplyResources((object) this.toolBarButtonUserEvent, "toolBarButtonUserEvent");
    this.toolBarButtonUserEvent.Image = (Image) Resources.add_user_event;
    this.toolBarButtonUserEvent.Name = "toolBarButtonUserEvent";
    this.toolBarButtonUserEvent.ButtonClick += new EventHandler(this.toolBarButtonUserEventClick);
    this.toolBarMenuItemLabel.Image = (Image) Resources.add_user_event;
    this.toolBarMenuItemLabel.Name = "toolBarMenuItemLabel";
    componentResourceManager.ApplyResources((object) this.toolBarMenuItemLabel, "toolBarMenuItemLabel");
    this.toolBarMenuItemLabel.Click += new EventHandler(this.UserEventLabelClick);
    this.toolBarMenuItemEditableLabel.Name = "toolBarMenuItemEditableLabel";
    componentResourceManager.ApplyResources((object) this.toolBarMenuItemEditableLabel, "toolBarMenuItemEditableLabel");
    this.toolBarMenuItemEditableLabel.Click += new EventHandler(this.EditableUserEventLabelClick);
    componentResourceManager.ApplyResources((object) this.toolBarButtonLogUpload, "toolBarButtonLogUpload");
    this.toolBarButtonLogUpload.Image = (Image) Resources.upload_log;
    this.toolBarButtonLogUpload.Name = "toolBarButtonLogUpload";
    this.toolBarButtonLogUpload.Click += new EventHandler(this.MarkLogForUploadClick);
    componentResourceManager.ApplyResources((object) this.toolStripButtonDataMiningTag, "toolStripButtonDataMiningTag");
    this.toolStripButtonDataMiningTag.Image = (Image) Resources.summaryfiles_noupload;
    this.toolStripButtonDataMiningTag.Name = "toolStripButtonDataMiningTag";
    this.toolStripButtonDataMiningTag.Click += new EventHandler(this.switchDataMiningProcessTagClick);
    this.toolStripSeparator2.Name = "toolStripSeparator2";
    componentResourceManager.ApplyResources((object) this.toolStripSeparator2, "toolStripSeparator2");
    componentResourceManager.ApplyResources((object) this.toolStripButtonPrint, "toolStripButtonPrint");
    this.toolStripButtonPrint.Image = (Image) Resources.print;
    this.toolStripButtonPrint.Name = "toolStripButtonPrint";
    this.toolStripButtonPrint.Click += new EventHandler(this.toolStripButtonPrint_Click);
    this.toolStripSeparator3.Name = "toolStripSeparator3";
    componentResourceManager.ApplyResources((object) this.toolStripSeparator3, "toolStripSeparator3");
    componentResourceManager.ApplyResources((object) this.toolBarButtonCut, "toolBarButtonCut");
    this.toolBarButtonCut.Image = (Image) Resources.cut;
    this.toolBarButtonCut.Name = "toolBarButtonCut";
    this.toolBarButtonCut.Click += new EventHandler(this.toolBarButtonCut_Click);
    componentResourceManager.ApplyResources((object) this.toolBarButtonCopy, "toolBarButtonCopy");
    this.toolBarButtonCopy.Image = (Image) Resources.copy;
    this.toolBarButtonCopy.Name = "toolBarButtonCopy";
    this.toolBarButtonCopy.Click += new EventHandler(this.toolBarButtonCopy_Click);
    componentResourceManager.ApplyResources((object) this.toolBarButtonPaste, "toolBarButtonPaste");
    this.toolBarButtonPaste.Image = (Image) Resources.paste;
    this.toolBarButtonPaste.Name = "toolBarButtonPaste";
    this.toolBarButtonPaste.Click += new EventHandler(this.toolBarButtonPaste_Click);
    this.toolStripSeparator16.Name = "toolStripSeparator16";
    componentResourceManager.ApplyResources((object) this.toolStripSeparator16, "toolStripSeparator16");
    componentResourceManager.ApplyResources((object) this.toolBarButtonUndo, "toolBarButtonUndo");
    this.toolBarButtonUndo.Image = (Image) Resources.undo;
    this.toolBarButtonUndo.Name = "toolBarButtonUndo";
    this.toolBarButtonUndo.Click += new EventHandler(this.toolBarButtonUndo_Click);
    this.toolStripSeparator4.Name = "toolStripSeparator4";
    componentResourceManager.ApplyResources((object) this.toolStripSeparator4, "toolStripSeparator4");
    this.toolStripButtonFilter.DisplayStyle = ToolStripItemDisplayStyle.Image;
    this.toolStripButtonFilter.Image = (Image) Resources.funnel;
    componentResourceManager.ApplyResources((object) this.toolStripButtonFilter, "toolStripButtonFilter");
    this.toolStripButtonFilter.Name = "toolStripButtonFilter";
    this.toolStripButtonFilter.Click += new EventHandler(this.toolStripButtonFilter_Click);
    this.toolStripSeparator7.Name = "toolStripSeparator7";
    componentResourceManager.ApplyResources((object) this.toolStripSeparator7, "toolStripSeparator7");
    this.toolBarButtonRefresh.DisplayStyle = ToolStripItemDisplayStyle.Image;
    this.toolBarButtonRefresh.Image = (Image) Resources.refresh;
    componentResourceManager.ApplyResources((object) this.toolBarButtonRefresh, "toolBarButtonRefresh");
    this.toolBarButtonRefresh.Name = "toolBarButtonRefresh";
    this.toolBarButtonRefresh.Click += new EventHandler(this.toolBarButtonRefresh_Click);
    this.toolStripSeparator6.Name = "toolStripSeparator6";
    componentResourceManager.ApplyResources((object) this.toolStripSeparator6, "toolStripSeparator6");
    this.toolStripButtonUnitsSystem.DisplayStyle = ToolStripItemDisplayStyle.Image;
    this.toolStripButtonUnitsSystem.Image = (Image) Resources.units_system;
    this.toolStripButtonUnitsSystem.Name = "toolStripButtonUnitsSystem";
    componentResourceManager.ApplyResources((object) this.toolStripButtonUnitsSystem, "toolStripButtonUnitsSystem");
    this.toolStripButtonUnitsSystem.Click += new EventHandler(this.toolStripButtonUnitsSystem_Click);
    this.toolStripSeparator17.Name = "toolStripSeparator17";
    componentResourceManager.ApplyResources((object) this.toolStripSeparator17, "toolStripSeparator17");
    this.toolStripButtonExpandAll.DisplayStyle = ToolStripItemDisplayStyle.Image;
    this.toolStripButtonExpandAll.Image = (Image) Resources.expandall;
    componentResourceManager.ApplyResources((object) this.toolStripButtonExpandAll, "toolStripButtonExpandAll");
    this.toolStripButtonExpandAll.Name = "toolStripButtonExpandAll";
    this.toolStripButtonExpandAll.Click += new EventHandler(this.toolStripButtonExpandAll_Click);
    this.toolStripButtonCollapseAll.DisplayStyle = ToolStripItemDisplayStyle.Image;
    this.toolStripButtonCollapseAll.Image = (Image) Resources.collapseall;
    componentResourceManager.ApplyResources((object) this.toolStripButtonCollapseAll, "toolStripButtonCollapseAll");
    this.toolStripButtonCollapseAll.Name = "toolStripButtonCollapseAll";
    this.toolStripButtonCollapseAll.Click += new EventHandler(this.toolStripButtonCollapseAll_Click);
    componentResourceManager.ApplyResources((object) this.menuStrip1, "menuStrip1");
    this.menuStrip1.Items.AddRange(new ToolStripItem[6]
    {
      (ToolStripItem) this.menuFile,
      (ToolStripItem) toolStripMenuItem3,
      (ToolStripItem) this.menuView,
      (ToolStripItem) toolStripMenuItem10,
      (ToolStripItem) toolStripMenuItem8,
      (ToolStripItem) toolStripMenuItem16
    });
    this.menuStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
    this.menuStrip1.Name = "menuStrip1";
    this.menuFile.DropDownItems.AddRange(new ToolStripItem[12]
    {
      (ToolStripItem) toolStripMenuItem1,
      (ToolStripItem) this.menuConnect,
      (ToolStripItem) toolStripSeparator1,
      (ToolStripItem) this.menuClose,
      (ToolStripItem) toolStripSeparator2,
      (ToolStripItem) this.menuPrint,
      (ToolStripItem) this.menuPrintPreview,
      (ToolStripItem) this.menuPrintFaults,
      (ToolStripItem) toolStripSeparator3,
      (ToolStripItem) this.menuRecentFiles,
      (ToolStripItem) toolStripSeparator4,
      (ToolStripItem) toolStripMenuItem2
    });
    this.menuFile.Name = "menuFile";
    this.menuFile.Overflow = ToolStripItemOverflow.AsNeeded;
    componentResourceManager.ApplyResources((object) this.menuFile, "menuFile");
    this.menuPrintFaults.Name = "menuPrintFaults";
    componentResourceManager.ApplyResources((object) this.menuPrintFaults, "menuPrintFaults");
    this.menuPrintFaults.Click += new EventHandler(this.OnMenuPrintFaultsClick);
    this.menuView.DropDownItems.AddRange(new ToolStripItem[15]
    {
      (ToolStripItem) toolStripMenuItem7,
      (ToolStripItem) this.menuViewSplit2,
      (ToolStripItem) this.menuNextView,
      (ToolStripItem) this.menuPreviousView,
      (ToolStripItem) this.toolStripSeparator9,
      (ToolStripItem) this.gotoToolStripMenuItem,
      (ToolStripItem) this.toolStripSeparator11,
      (ToolStripItem) toolStripMenuItem15,
      (ToolStripItem) this.toolStripSeparator14,
      (ToolStripItem) this.filterToolStripMenuItem,
      (ToolStripItem) this.menuViewSplit1,
      (ToolStripItem) this.busMonitoringToolStripMenuItem,
      (ToolStripItem) this.toolStripMenuItemMute,
      (ToolStripItem) this.toolStripSeparator15,
      (ToolStripItem) this.refreshToolStripMenuItem
    });
    this.menuView.Name = "menuView";
    this.menuView.Overflow = ToolStripItemOverflow.AsNeeded;
    componentResourceManager.ApplyResources((object) this.menuView, "menuView");
    this.menuView.DropDownOpened += new EventHandler(this.menuView_Popup);
    this.menuViewSplit2.Name = "menuViewSplit2";
    componentResourceManager.ApplyResources((object) this.menuViewSplit2, "menuViewSplit2");
    componentResourceManager.ApplyResources((object) this.menuNextView, "menuNextView");
    this.menuNextView.Name = "menuNextView";
    this.menuNextView.Click += new EventHandler(this.nextMenu_Click);
    componentResourceManager.ApplyResources((object) this.menuPreviousView, "menuPreviousView");
    this.menuPreviousView.Name = "menuPreviousView";
    this.menuPreviousView.Click += new EventHandler(this.prevMenu_Click);
    this.toolStripSeparator9.Name = "toolStripSeparator9";
    componentResourceManager.ApplyResources((object) this.toolStripSeparator9, "toolStripSeparator9");
    this.gotoToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[3]
    {
      (ToolStripItem) this.backToolStripMenuItem,
      (ToolStripItem) this.forwardToolStripMenuItem,
      (ToolStripItem) this.toolStripSeparator10
    });
    this.gotoToolStripMenuItem.Name = "gotoToolStripMenuItem";
    componentResourceManager.ApplyResources((object) this.gotoToolStripMenuItem, "gotoToolStripMenuItem");
    this.gotoToolStripMenuItem.DropDownOpening += new EventHandler(this.gotoToolStripMenuItem_DropDownOpening);
    this.gotoToolStripMenuItem.DropDownItemClicked += new ToolStripItemClickedEventHandler(this.gotoToolStripMenuItem_DropDownItemClicked);
    this.backToolStripMenuItem.Image = (Image) Resources.back;
    componentResourceManager.ApplyResources((object) this.backToolStripMenuItem, "backToolStripMenuItem");
    this.backToolStripMenuItem.Name = "backToolStripMenuItem";
    this.backToolStripMenuItem.Click += new EventHandler(this.OnHistoryBackClick);
    this.forwardToolStripMenuItem.Image = (Image) Resources.forward;
    componentResourceManager.ApplyResources((object) this.forwardToolStripMenuItem, "forwardToolStripMenuItem");
    this.forwardToolStripMenuItem.Name = "forwardToolStripMenuItem";
    this.forwardToolStripMenuItem.Click += new EventHandler(this.OnHistoryForwardClick);
    this.toolStripSeparator10.Name = "toolStripSeparator10";
    componentResourceManager.ApplyResources((object) this.toolStripSeparator10, "toolStripSeparator10");
    this.toolStripSeparator11.Name = "toolStripSeparator11";
    componentResourceManager.ApplyResources((object) this.toolStripSeparator11, "toolStripSeparator11");
    this.toolStripSeparator14.Name = "toolStripSeparator14";
    componentResourceManager.ApplyResources((object) this.toolStripSeparator14, "toolStripSeparator14");
    this.filterToolStripMenuItem.Image = (Image) Resources.funnel;
    this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
    componentResourceManager.ApplyResources((object) this.filterToolStripMenuItem, "filterToolStripMenuItem");
    this.filterToolStripMenuItem.Click += new EventHandler(this.toolStripButtonFilter_Click);
    this.menuViewSplit1.Name = "menuViewSplit1";
    componentResourceManager.ApplyResources((object) this.menuViewSplit1, "menuViewSplit1");
    this.busMonitoringToolStripMenuItem.Image = (Image) Resources.environment_view;
    this.busMonitoringToolStripMenuItem.Name = "busMonitoringToolStripMenuItem";
    componentResourceManager.ApplyResources((object) this.busMonitoringToolStripMenuItem, "busMonitoringToolStripMenuItem");
    this.busMonitoringToolStripMenuItem.Click += new EventHandler(this.busMonitoringToolStripMenuItem_Click);
    this.toolStripMenuItemMute.BackColor = SystemColors.Control;
    this.toolStripMenuItemMute.Image = (Image) Resources.cd_pause;
    this.toolStripMenuItemMute.Name = "toolStripMenuItemMute";
    componentResourceManager.ApplyResources((object) this.toolStripMenuItemMute, "toolStripMenuItemMute");
    this.toolStripMenuItemMute.Click += new EventHandler(this.toolStripMenuItemMute_Click);
    this.toolStripSeparator15.Name = "toolStripSeparator15";
    componentResourceManager.ApplyResources((object) this.toolStripSeparator15, "toolStripSeparator15");
    this.refreshToolStripMenuItem.Image = (Image) Resources.refresh;
    this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
    componentResourceManager.ApplyResources((object) this.refreshToolStripMenuItem, "refreshToolStripMenuItem");
    this.refreshToolStripMenuItem.Click += new EventHandler(this.refreshToolStripMenuItem_Click);
    componentResourceManager.ApplyResources((object) this.fullScreenStrip, "fullScreenStrip");
    this.fullScreenStrip.GripStyle = ToolStripGripStyle.Hidden;
    this.fullScreenStrip.Items.AddRange(new ToolStripItem[1]
    {
      (ToolStripItem) this.menuFullscreen
    });
    this.fullScreenStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
    this.fullScreenStrip.Name = "fullScreenStrip";
    this.menuFullscreen.DisplayStyle = ToolStripItemDisplayStyle.Text;
    this.menuFullscreen.Name = "menuFullscreen";
    componentResourceManager.ApplyResources((object) this.menuFullscreen, "menuFullscreen");
    this.menuFullscreen.Click += new EventHandler(this.menuFullscreen_Click);
    componentResourceManager.ApplyResources((object) this.findBar, "findBar");
    ((ToolStrip) this.findBar).GripStyle = ToolStripGripStyle.Hidden;
    ((ToolStrip) this.findBar).LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
    ((Control) this.findBar).Name = "findBar";
    ((ToolStrip) this.findBar).TabStop = true;
    componentResourceManager.ApplyResources((object) this.statusStrip, "statusStrip");
    this.statusStrip.Items.AddRange(new ToolStripItem[6]
    {
      (ToolStripItem) this.connectionsButton,
      (ToolStripItem) this.statusSeparator1,
      (ToolStripItem) this.statusStripPanelLogTime,
      (ToolStripItem) this.statusStripPanelEcuConnection,
      (ToolStripItem) this.statusStripPanelServerConnection,
      (ToolStripItem) this.statusStripProgressBarServerConnection
    });
    this.statusStrip.Name = "statusStrip";
    this.statusStrip.TabStop = true;
    this.connectionsButton.BackColor = SystemColors.Control;
    this.connectionsButton.Checked = true;
    this.connectionsButton.CheckState = CheckState.Checked;
    this.connectionsButton.ForeColor = SystemColors.ControlText;
    this.connectionsButton.Name = "connectionsButton";
    componentResourceManager.ApplyResources((object) this.connectionsButton, "connectionsButton");
    this.connectionsButton.Click += new EventHandler(this.OnConnectionsButtonClick);
    this.statusSeparator1.Name = "statusSeparator1";
    componentResourceManager.ApplyResources((object) this.statusSeparator1, "statusSeparator1");
    this.statusStripPanelLogTime.Margin = new Padding(0, 3, 0, 3);
    this.statusStripPanelLogTime.Name = "statusStripPanelLogTime";
    componentResourceManager.ApplyResources((object) this.statusStripPanelLogTime, "statusStripPanelLogTime");
    this.statusStripPanelEcuConnection.Margin = new Padding(0, 3, 0, 3);
    this.statusStripPanelEcuConnection.Name = "statusStripPanelEcuConnection";
    componentResourceManager.ApplyResources((object) this.statusStripPanelEcuConnection, "statusStripPanelEcuConnection");
    this.statusStripPanelServerConnection.Margin = new Padding(0, 3, 0, 3);
    this.statusStripPanelServerConnection.Name = "statusStripPanelServerConnection";
    componentResourceManager.ApplyResources((object) this.statusStripPanelServerConnection, "statusStripPanelServerConnection");
    componentResourceManager.ApplyResources((object) this.statusStripProgressBarServerConnection, "statusStripProgressBarServerConnection");
    this.statusStripProgressBarServerConnection.Margin = new Padding(0, 4, 0, 4);
    this.statusStripProgressBarServerConnection.Name = "statusStripProgressBarServerConnection";
    componentResourceManager.ApplyResources((object) this.toolStripContainer, "toolStripContainer");
    this.toolStripContainer.Controls.Add((Control) this.menuStrip1);
    this.toolStripContainer.Controls.Add((Control) this.fullScreenStrip);
    this.toolStripContainer.Name = "toolStripContainer";
    componentResourceManager.ApplyResources((object) this.toolStripContainer2, "toolStripContainer2");
    this.toolStripContainer2.Controls.Add((Control) this.toolStrip);
    this.toolStripContainer2.Controls.Add((Control) this.navigationToolStrip);
    this.toolStripContainer2.Controls.Add((Control) this.findBar);
    this.toolStripContainer2.Name = "toolStripContainer2";
    componentResourceManager.ApplyResources((object) this.navigationToolStrip, "navigationToolStrip");
    this.navigationToolStrip.GripStyle = ToolStripGripStyle.Hidden;
    this.navigationToolStrip.Items.AddRange(new ToolStripItem[2]
    {
      (ToolStripItem) this.toolStripButtonBack,
      (ToolStripItem) this.toolStripButtonForward
    });
    this.navigationToolStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
    this.navigationToolStrip.Name = "navigationToolStrip";
    this.navigationToolStrip.TabStop = true;
    this.toolStripButtonBack.DisplayStyle = ToolStripItemDisplayStyle.Image;
    componentResourceManager.ApplyResources((object) this.toolStripButtonBack, "toolStripButtonBack");
    this.toolStripButtonBack.Image = (Image) Resources.back;
    this.toolStripButtonBack.Name = "toolStripButtonBack";
    this.toolStripButtonBack.ButtonClick += new EventHandler(this.OnHistoryBackClick);
    this.toolStripButtonBack.DropDownOpening += new EventHandler(this.toolStripButtonBack_DropDownOpening);
    this.toolStripButtonBack.DropDownItemClicked += new ToolStripItemClickedEventHandler(this.toolStripButtonBack_DropDownItemClicked);
    this.toolStripButtonForward.DisplayStyle = ToolStripItemDisplayStyle.Image;
    componentResourceManager.ApplyResources((object) this.toolStripButtonForward, "toolStripButtonForward");
    this.toolStripButtonForward.Image = (Image) Resources.forward;
    this.toolStripButtonForward.Name = "toolStripButtonForward";
    this.toolStripButtonForward.ButtonClick += new EventHandler(this.OnHistoryForwardClick);
    this.toolStripButtonForward.DropDownOpening += new EventHandler(this.toolStripButtonForward_DropDownOpening);
    this.toolStripButtonForward.DropDownItemClicked += new ToolStripItemClickedEventHandler(this.toolStripButtonForward_DropDownItemClicked);
    this.mainTabView.ContainerApplication = (IContainerApplication) null;
    componentResourceManager.ApplyResources((object) this.mainTabView, "mainTabView");
    ((Control) this.mainTabView).Name = "mainTabView";
    this.mainTabView.ShowSidebar = true;
    this.mainTabView.SplashProgressBar = (IProgressBar) null;
    ((UserControl) this.mainTabView).Load += new EventHandler(this.mainTabView_Load);
    this.menuItemZenZefiT.Name = "menuItemZenZefiT";
    componentResourceManager.ApplyResources((object) this.menuItemZenZefiT, "menuItemZenZefiT");
    this.menuItemZenZefiT.Click += new EventHandler(this.menuItemZenZefiT_Click);
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.Controls.Add((Control) this.mainTabView);
    this.Controls.Add((Control) this.toolStripContainer2);
    this.Controls.Add((Control) this.toolStripContainer);
    this.Controls.Add((Control) this.statusStrip);
    this.Name = nameof (MainForm);
    this.SizeGripStyle = SizeGripStyle.Show;
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
    this.ResumeLayout(false);
    this.PerformLayout();
  }

  ITroubleshooting IContainerApplication.Troubleshooting
  {
    get => TroubleshootingMenuProxy.GlobalInstance.Troubleshooting;
  }

  ToolStrip IContainerApplication.Menu => (ToolStrip) this.menuStrip1;

  void IContainerApplication.SelectPlace(IPlace place) => this.mainTabView.SelectPlace(place);

  void IContainerApplication.SelectPlace(PanelIdentifier identifier, string location)
  {
    this.mainTabView.SelectPlace(identifier, location);
  }

  void IContainerApplication.PreloadPlace(IPlace place)
  {
    this.mainTabView.EnsurePanelCreated(place);
  }

  void IContainerApplication.DisableForProgramming(bool disabled)
  {
    this.mainTabView.SidebarEnabled = !disabled;
    ((Control) WarningsPanel.GlobalInstance).Enabled = !disabled;
  }

  public MainForm(SplashScreen splashScreen, string logFileToLoad)
  {
    WarningItem.LinkClicked += (EventHandler<LinkLabelLinkClickedEventArgs>) ((sender, e) => Link.ShowTarget((string) e.Link.LinkData));
    this.splashScreen = splashScreen;
    ((Component) this.splashScreen).Disposed += new EventHandler(this.OnSplashScreenDisposed);
    this.sapiManager = SapiManager.GlobalInstance;
    this.sapiManager.DesignMode = this.DesignMode;
    this.AddCaesarComponentInformation();
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    this.toolStripButtonMute.Visible = false;
    this.toolStripMenuItemMute.Visible = false;
    using (Graphics graphics = this.CreateGraphics())
    {
      this.MaximumSize = new Size((int) (20f * graphics.DpiX), (int) (11.25f * graphics.DpiY));
      ToolStrip[] toolStripArray = new ToolStrip[5]
      {
        (ToolStrip) this.statusStrip,
        this.fullScreenStrip,
        this.toolStrip,
        (ToolStrip) this.menuStrip1,
        this.navigationToolStrip
      };
      foreach (ToolStrip toolStrip in toolStripArray)
      {
        toolStrip.AutoSize = false;
        toolStrip.ImageScalingSize = new Size((int) (20.0 * (double) graphics.DpiX / 96.0), (int) (20.0 * (double) graphics.DpiY / 96.0));
        toolStrip.AutoSize = true;
      }
      this.statusStrip.AutoSize = false;
      this.statusStrip.Height = this.menuStrip1.Height;
    }
    this.mainTabView.SplashProgressBar = (IProgressBar) this.splashScreen;
    this.mainTabView.ContainerApplication = (IContainerApplication) this;
    this.mainTabView.SetBranding(ApplicationInformation.Branding);
    this.Icon = ApplicationInformation.Branding.ProductIcon;
    this.sapiManager.ActiveChannelsChanged += new EventHandler(this.sapiManager_ActiveChannelsChanged);
    this.sapiManager.LogFileChannelsChanged += new EventHandler(this.sapiManager_LogFileChannelsChanged);
    this.sapiManager.EquipmentTypeChanged += new EventHandler<EquipmentTypeChangedEventArgs>(this.sapiManager_EquipmentTypeChanged);
    LicenseManager.GlobalInstance.KeysChanged += new EventHandler(this.LicenseKeysChanged);
    AdapterInformation.GlobalInstance.AdapterUpdated += new EventHandler(this.AdaptersUpdated);
    PrintHelper.SetParent((Control) this);
    this.logFileToLoad = logFileToLoad;
    this.recentFiles = new RecentFiles(this.menuRecentFiles, "LogFiles", "RecentLogFiles", 10);
    this.recentFiles.RecentFileClicked += new EventHandler(this.menuRecentFile_Click);
    this.mainTabView.FilteredContentChanged += new EventHandler(this.mainTabView_FilteredContentChanged);
  }

  private void AddCaesarComponentInformation()
  {
    ComponentInformation componentInformation = new ComponentInformation();
    ComponentInformationGroups.GlobalInstance.Add(Components.GroupSupportedDevices, Resources.ComponentGroup_SupportedDevices, (IComponentInformation) componentInformation);
    foreach (IGrouping<string, Ecu> source in ((IEnumerable<Ecu>) this.sapiManager.Sapi.Ecus).GroupBy<Ecu, string>((Func<Ecu, string>) (e => e.Name)))
    {
      foreach (Ecu ecu in (IEnumerable<Ecu>) source)
        componentInformation.Add($"{ecu.Name}_{(object) ecu.DiagnosisSource}", ecu.Name + (source.Count<Ecu>() > 1 ? $" ({Path.GetExtension(ecu.DescriptionFileName)})" : string.Empty), ecu.ConfigurationFileVersion.HasValue ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1})", (object) ecu.DescriptionDataVersion, (object) ecu.ConfigurationFileVersion) : ecu.DescriptionDataVersion, true);
    }
    if (((ReadOnlyCollection<Ecu>) this.sapiManager.Sapi.Ecus).Count == 0)
      componentInformation.Add(Components.InfoNotFound, Resources.ComponentInfo_NotFound, string.Empty, true);
    MainForm.UpdateAdapterInformation();
    IComponentInformation componentsVersionInformation = ComponentInformationGroups.GlobalInstance.GetGroupInformation("Application Components");
    componentsVersionInformation.Add(Components.InfoApplicationType, Resources.ComponentInfo_ApplicationType, Environment.Is64BitProcess ? "64-Bit" : "32-Bit", true);
    componentsVersionInformation.Add(Components.InfoSapi, Resources.ComponentInfo_SAPI, ApplicationInformation.GetAssemblyVersion(typeof (Sapi).Assembly), true);
    componentsVersionInformation.Add(Components.InfoCaesar, Resources.ComponentInfo_Caesar, $"{this.sapiManager.Sapi.CaesarLibraryVersion} ({this.sapiManager.Sapi.CaesarLibraryCompilationDate})", true);
    componentsVersionInformation.Add(Components.InfoRP1210, Resources.ComponentInfo_RP1210, string.Empty, false);
    componentsVersionInformation.Add(Components.InfoMvci, Resources.ComponentInfo_MVCI, string.Empty, false);
    componentsVersionInformation.Add(Components.InfoEthernetPduApi, Resources.ComponentInfo_DoIP_PDU_API, string.Empty, false);
    componentsVersionInformation.Add(Components.InfoGlobals, Resources.ComponentInfo_Globals, string.Empty, false);
    this.UpdateComponentVersionInformation(componentsVersionInformation);
    this.sapiManager.SapiResetCompleted += (EventHandler) ((sender, e) => this.UpdateComponentVersionInformation(componentsVersionInformation));
    this.sapiManager.ChannelInitializingEvent += new EventHandler<ChannelInitializingEventArgs>(this.GlobalInstance_ChannelInitializingEvent);
    string sidDll = Program.GetSidDll();
    if (string.IsNullOrEmpty(sidDll) || !File.Exists(sidDll))
      return;
    componentsVersionInformation.Add(Components.InfoSid, Resources.ComponentInfo_Sid, FileVersionInfo.GetVersionInfo(sidDll).FileVersion, true);
  }

  private void GlobalInstance_ChannelInitializingEvent(
    object sender,
    ChannelInitializingEventArgs e)
  {
    if (!this.toolStripButtonMute.Checked && !this.toolStripMenuItemMute.Checked)
      return;
    e.AutoRead = false;
    e.Channel.EcuInfos.AutoRead = false;
    e.Channel.Parameters.AutoReadSummaryParameters = false;
  }

  private void UpdateComponentVersionInformation(IComponentInformation componentsVersionInformation)
  {
    string str1 = (string) null;
    string str2 = (string) null;
    string str3 = (string) null;
    bool useMcd = this.sapiManager.Sapi.UseMcd;
    if (useMcd && this.sapiManager.McdAvailable)
    {
      str1 = Sapi.McdSystemDescription;
      int startIndex = str1.IndexOf("DtsBaseSystem", StringComparison.OrdinalIgnoreCase);
      if (startIndex != -1)
        str1 = str1.Substring(startIndex);
      str2 = ApplicationInformation.GetLoadedModuleVersion("PDUAPI_Bosch.dll");
      str3 = this.sapiManager.Sapi.DiagnosisProtocols["UDS_CAN_D"]?.DescriptionDataVersion;
    }
    componentsVersionInformation.UpdateValue(Components.InfoMvci, str1 ?? (useMcd ? Resources.ComponentInfo_NotAvailable : Resources.ComponentInfo_NotEnabled), useMcd);
    componentsVersionInformation.UpdateValue(Components.InfoEthernetPduApi, str2 ?? (useMcd ? Resources.ComponentInfo_NotAvailable : Resources.ComponentInfo_NotEnabled), useMcd);
    componentsVersionInformation.UpdateValue(Components.InfoGlobals, str3 ?? (useMcd ? Resources.ComponentInfo_NotAvailable : Resources.ComponentInfo_NotEnabled), useMcd);
    IRollCall manager = ChannelCollection.GetManager((Protocol) 71993);
    IComponentInformation icomponentInformation = componentsVersionInformation;
    string infoRp1210 = Components.InfoRP1210;
    string str4;
    if (manager == null)
      str4 = "n/a";
    else
      str4 = string.Join(" - ", ((IEnumerable<string>) new string[2]
      {
        manager?.DeviceName,
        manager.DeviceLibraryVersion
      }).Where<string>((Func<string, bool>) (s => s != null)));
    icomponentInformation.UpdateValue(infoRp1210, str4, true);
  }

  private void OnSplashScreenDisposed(object sender, EventArgs e)
  {
    ((Component) this.splashScreen).Disposed -= new EventHandler(this.OnSplashScreenDisposed);
    this.splashScreen = (SplashScreen) null;
    this.mainTabView.SplashProgressBar = (IProgressBar) this.splashScreen;
  }

  private void LicenseKeysChanged(object sender, EventArgs e) => this.UpdateViewMenu();

  private void AdaptersUpdated(object sender, EventArgs e)
  {
    this.CheckForProhibitedAdapters();
    this.CheckForBluetoothAdapters();
    MainForm.UpdateAdapterInformation();
    this.CheckConnectionHardware();
  }

  private static void UpdateAdapterInformation()
  {
    ComponentInformation componentInformation;
    if (ComponentInformationGroups.GlobalInstance.GroupIdentifiers.Contains<string>(Components.GroupConfiguredTranslators))
    {
      componentInformation = (ComponentInformation) ComponentInformationGroups.GlobalInstance.GetGroupInformation(Components.GroupConfiguredTranslators);
      componentInformation.Clear();
    }
    else
    {
      componentInformation = new ComponentInformation();
      ComponentInformationGroups.GlobalInstance.Add(Components.GroupConfiguredTranslators, Resources.ComponentGroup_ConfiguredTranslators, (IComponentInformation) componentInformation);
    }
    if (AdapterInformation.GlobalInstance == null)
      return;
    foreach (string connectedAdapterName in AdapterInformation.GlobalInstance.ConnectedAdapterNames)
      componentInformation.Add(connectedAdapterName, connectedAdapterName, string.Empty, true);
  }

  private void SaveSettings(ISettings settings)
  {
    if (this.WindowState != FormWindowState.Minimized)
      settings.SetValue<FormWindowState>("WindowState", "MainWindow", this.WindowState, false);
    settings.SetValue<Point>("Location", "MainWindow", this.lastKnownGoodLocation, false);
    settings.SetValue<Size>("Size", "MainWindow", this.lastKnownGoodSize, false);
  }

  private void LoadSettings(ISettings settings)
  {
    this.WindowState = FormWindowState.Normal;
    Point savedLocation = settings.GetValue<Point>("Location", "MainWindow", this.Location);
    if (!((IEnumerable<Screen>) Screen.AllScreens).Any<Screen>((Func<Screen, bool>) (s => s.Bounds.Contains(savedLocation))))
      savedLocation = Screen.PrimaryScreen.WorkingArea.Location;
    Rectangle rect = new Rectangle(savedLocation, settings.GetValue<Size>("Size", "MainWindow", this.Size));
    rect.Intersect(Screen.GetWorkingArea(rect));
    this.Bounds = rect;
    this.lastKnownGoodLocation = this.Location;
    this.lastKnownGoodSize = this.Size;
    this.WindowState = settings.GetValue<FormWindowState>("WindowState", "MainWindow", this.WindowState);
    this.regularFormBorderStyle = this.FormBorderStyle;
    this.regularFormWindowState = this.WindowState;
  }

  private void OnMenuExitClick(object sender, EventArgs e) => this.Close();

  protected override void OnFormClosing(FormClosingEventArgs e)
  {
    base.OnFormClosing(e);
    if (!e.Cancel)
      e.Cancel = this.CancelDueToFlashing();
    if (e.Cancel)
      return;
    foreach (Control control in (ArrangedElementCollection) this.Controls)
      control.Enabled = false;
    Application.DoEvents();
    this.CloseAllConnections();
    Application.DoEvents();
    foreach (ToolStrip toolStrip in this.toolStripContainer.Controls.OfType<ToolStrip>().Union<ToolStrip>(this.toolStripContainer2.Controls.OfType<ToolStrip>()))
      toolStrip.Items.Clear();
  }

  private bool CancelDueToFlashing()
  {
    bool flashing = this.CurrentlyFlashing();
    if (flashing)
    {
      int num = (int) MessageBox.Show(Resources.MessageDeviceBeingFlashedCannotBeTerminatedPleaseWait, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, ControlHelpers.IsRightToLeft((Control) this) ? MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading : (MessageBoxOptions) 0);
      flashing = true;
    }
    return flashing;
  }

  private bool CurrentlyFlashing()
  {
    bool flag = false;
    foreach (Channel channel in (ChannelBaseCollection) this.sapiManager.Sapi.Channels)
    {
      if (channel.CommunicationsState == 7)
      {
        flag = true;
        break;
      }
    }
    return flag;
  }

  private void OnMenuUpdateClick(object sender, EventArgs e)
  {
    ServerClient.GlobalInstance.Go((Collection<UnitInformation>) null, (Collection<UnitInformation>) null);
  }

  private void OnMenuOptionsClick(object sender, EventArgs e)
  {
    OptionsDialog optionsDialog = new OptionsDialog();
    int num = (int) optionsDialog.ShowDialog();
    optionsDialog.Dispose();
  }

  private void menuView_ChildClick(object sender, EventArgs e)
  {
    if (!(sender is ToolStripMenuItem toolStripMenuItem))
      return;
    this.mainTabView.SelectPlace(toolStripMenuItem.Tag as IPlace);
  }

  private void OnMenuConnectClick(object sender, EventArgs e)
  {
    using (ConnectionDialog connectionDialog = new ConnectionDialog())
    {
      int num = (int) connectionDialog.ShowDialog();
    }
  }

  private void menuView_Popup(object sender, EventArgs e) => this.UpdateViewRadio();

  private void UpdateViewRadio()
  {
    foreach (object dropDownItem in (ArrangedElementCollection) this.menuView.DropDownItems)
    {
      if (dropDownItem is ToolStripMenuItem toolStripMenuItem && toolStripMenuItem.Checked)
      {
        toolStripMenuItem.Checked = false;
        break;
      }
    }
    if (this.mainTabView.SelectedIndex < 0 || !(this.menuView.DropDownItems[this.mainTabView.SelectedIndex] is ToolStripMenuItem dropDownItem1))
      return;
    dropDownItem1.Checked = true;
  }

  private void mainTabView_Load(object sender, EventArgs e) => this.UpdateViewMenu();

  private void UpdateViewMenu()
  {
    if (this.mainTabView.Tabs.Count > 0)
    {
      foreach (ToolStripItem toolStripItem in (IEnumerable<ToolStripItem>) this.menuView.DropDownItems.Cast<ToolStripItem>().TakeWhile<ToolStripItem>((Func<ToolStripItem, bool>) (item => !(item is ToolStripSeparator))).ToList<ToolStripItem>())
      {
        this.menuView.DropDownItems.Remove(toolStripItem);
        toolStripItem.Dispose();
      }
      for (int index = 0; index < this.mainTabView.Tabs.Count; ++index)
      {
        IPlace tab = this.mainTabView.Tabs[index];
        if (ApplicationInformation.IsViewVisible(tab.Identifier))
        {
          ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(tab.Title, (Image) null, new EventHandler(this.menuView_ChildClick));
          toolStripMenuItem.ShortcutKeys = tab.Shortcut;
          toolStripMenuItem.Enabled = LicenseManager.GlobalInstance.AccessLevel >= tab.MinAccessLevel;
          toolStripMenuItem.Tag = (object) tab;
          this.menuView.DropDownItems.Insert(index, (ToolStripItem) toolStripMenuItem);
        }
      }
      if (this.mainTabView.Tabs.Count > 1)
      {
        this.menuNextView.Enabled = true;
        this.menuPreviousView.Enabled = true;
      }
    }
    this.UpdateViewRadio();
  }

  private void menuLogOpen_Click(object sender, EventArgs e) => this.LogOpen();

  private void menuLogPlay_Click(object sender, EventArgs e) => this.PlayOrPauseLog();

  private void menuRecentFile_Click(object sender, EventArgs e)
  {
    if (!this.sapiManager.RequestOpenLog())
      return;
    string text = ((ToolStripItem) sender).Text;
    if (((IEnumerable<string>) new string[2]
    {
      SapiManager.GlobalInstance.CurrentLogFileInformation.SummaryFilePath,
      SapiManager.GlobalInstance.CurrentLogFileInformation.LogFilePath
    }).Contains<string>(text))
    {
      int num = (int) MessageBox.Show(Resources.MessageCanNotOpenActiveLog, Resources.CaptionFileAccessError, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, ControlHelpers.IsRightToLeft((Control) this) ? MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading : (MessageBoxOptions) 0);
    }
    else
    {
      if (!File.Exists(text))
        return;
      this.LogOpen(text);
      if (this.logFile == null || !this.logFile.Summary)
        return;
      this.ShowLogFileInfoDialog();
    }
  }

  private void PlayOrPauseLog()
  {
    if (this.logFile == null)
      return;
    if (this.logFile.Playing)
      this.LogPause();
    else
      this.LogPlay();
  }

  private void UpdateSapiManagerStatus(string text)
  {
    this.statusStripPanelEcuConnection.Text = text;
    this.statusStrip.Update();
  }

  private void CloseAllConnections() => this.sapiManager.CloseAllConnections();

  private void OnCloseClick(object sender, EventArgs e) => this.CloseAllConnections();

  private void menuLogStop_Click(object sender, EventArgs e) => this.LogStop();

  private void menuLogSeekStart_Click(object sender, EventArgs e) => this.LogSeekStart();

  private void menuLogSeekEnd_Click(object sender, EventArgs e) => this.LogSeekEnd();

  private void OnSeekTime(object sender, EventArgs e) => this.LogSeek();

  private void SetPlaybackSpeed(double speed)
  {
    if (this.logFile == null)
      return;
    this.logFile.Playing = true;
    this.logFile.PlaybackSpeed = speed;
  }

  private void menuLogSpeedX1_Click(object sender, EventArgs e) => this.SetPlaybackSpeed(1.0);

  private void menuLogSpeedX2_Click(object sender, EventArgs e) => this.SetPlaybackSpeed(2.0);

  private void menuLogSpeedX4_Click(object sender, EventArgs e) => this.SetPlaybackSpeed(4.0);

  private void menuLogSpeedX8_Click(object sender, EventArgs e) => this.SetPlaybackSpeed(8.0);

  private void menuLogSpeedX1B_Click(object sender, EventArgs e) => this.SetPlaybackSpeed(-1.0);

  private void menuLogSpeedX2B_Click(object sender, EventArgs e) => this.SetPlaybackSpeed(-2.0);

  private void menuLogSpeedX4B_Click(object sender, EventArgs e) => this.SetPlaybackSpeed(-4.0);

  private void menuLogSpeedX8B_Click(object sender, EventArgs e) => this.SetPlaybackSpeed(-8.0);

  private void Cut()
  {
    this.editSupport.SetTarget((object) this.ActiveControl);
    this.editSupport.Cut();
  }

  private void Copy()
  {
    this.editSupport.SetTarget((object) this.ActiveControl);
    this.editSupport.Copy();
  }

  private void Paste()
  {
    this.editSupport.SetTarget((object) this.ActiveControl);
    this.editSupport.Paste();
  }

  private void Undo()
  {
    this.editSupport.SetTarget((object) this.ActiveControl);
    this.editSupport.Undo();
  }

  private void Delete()
  {
    this.editSupport.SetTarget((object) this.ActiveControl);
    this.editSupport.Delete();
  }

  private void SelectAll()
  {
    this.editSupport.SetTarget((object) this.ActiveControl);
    this.editSupport.SelectAll();
  }

  private void Print()
  {
    if (!this.CanPrint)
      return;
    PrintHelper.ShowPrintDialog(this.mainTabView.Tabs[this.mainTabView.SelectedIndex].Title, (IProvideHtml) this.mainTabView, (IncludeInfo) 3);
  }

  private void PrintPreview()
  {
    if (!this.CanPrint)
      return;
    PrintHelper.ShowPrintPreviewDialog(this.mainTabView.Tabs[this.mainTabView.SelectedIndex].Title, (IProvideHtml) this.mainTabView, (IncludeInfo) 3);
  }

  private bool CanPrint
  {
    get => !PrintHelper.Busy && this.mainTabView != null && this.mainTabView.CanProvideHtml;
  }

  private void LogOpen()
  {
    if (!this.sapiManager.RequestOpenLog())
      return;
    using (OpenLogFileForm openLogFileForm = new OpenLogFileForm())
    {
      if (openLogFileForm.ShowDialog() != DialogResult.OK)
        return;
      this.LogOpen(openLogFileForm.FileName);
      if (this.logFile == null || !this.logFile.Summary)
        return;
      this.ShowLogFileInfoDialog();
    }
  }

  private void LogOpen(string logFileToLoad)
  {
    if (string.IsNullOrEmpty(logFileToLoad))
      return;
    LogFile logFile = SapiManager.GlobalInstance.TryLoadLogFile(logFileToLoad, (Control) this);
    if (logFile == null)
      return;
    this.recentFiles.AddRecentFile(logFileToLoad);
    this.CloseAllConnections();
    this.sapiManager.LogFileChannels = logFile.Channels;
    this.sapiManager.Online = false;
    this.UpdateLogSeekBar();
  }

  private void LogPlay()
  {
    if (this.logFile == null)
      return;
    this.logFile.PlaybackSpeed = 1.0;
    this.logFile.Playing = true;
  }

  private void LogStop() => this.LogSeekStart();

  private void LogSeekStart()
  {
    if (this.logFile == null)
      return;
    this.LogSeek(this.logFile.StartTime);
  }

  private void LogSeekEnd()
  {
    if (this.logFile == null)
      return;
    this.LogSeek(this.logFile.EndTime);
  }

  private void LogSeek()
  {
    if (this.logFile == null)
      return;
    using (SeekTimeDialog seekTimeDialog = new SeekTimeDialog(this.logFile))
    {
      if (seekTimeDialog.ShowDialog((IWin32Window) this) != DialogResult.OK)
        return;
      this.LogSeek(seekTimeDialog.SelectedTime);
    }
  }

  private void LogSeek(DateTime time)
  {
    if (this.logFile == null || !(time >= this.logFile.StartTime) || !(time <= this.logFile.EndTime))
      return;
    this.logFile.Playing = false;
    this.logFile.CurrentTime = time;
  }

  private void LogPause() => this.logFile.Playing = false;

  private void LogFastForward()
  {
    if (this.logFile == null)
      return;
    if (this.logFile.Playing)
    {
      double playbackSpeed = this.logFile.PlaybackSpeed;
      this.logFile.PlaybackSpeed = playbackSpeed > 0.0 ? playbackSpeed * 2.0 : 1.0;
    }
    else
      this.SetPlaybackSpeed(2.0);
  }

  private void LogRewind()
  {
    if (this.logFile == null)
      return;
    if (this.logFile.Playing)
    {
      double playbackSpeed = this.logFile.PlaybackSpeed;
      this.logFile.PlaybackSpeed = playbackSpeed < 0.0 ? playbackSpeed * 2.0 : -1.0;
    }
    else
      this.SetPlaybackSpeed(-1.0);
  }

  public ToolStrip GetLogFileControls()
  {
    Panel parent = this.toolStrip.Parent as Panel;
    parent.MinimumSize = parent.Size;
    return new ToolStrip()
    {
      Items = {
        (ToolStripItem) this.toolBarButtonPlayPause,
        (ToolStripItem) this.toolBarButtonStop,
        (ToolStripItem) this.toolBarButtonSeekStart,
        (ToolStripItem) this.toolBarButtonRewind,
        (ToolStripItem) this.toolBarTrackBarSeek,
        (ToolStripItem) this.toolBarButtonFastForward,
        (ToolStripItem) this.toolBarButtonSeekEnd,
        (ToolStripItem) this.toolStripButtonSeekTime
      }
    };
  }

  public void ReplaceLogFileControls()
  {
    if (this.toolStrip == null || this.toolStrip.IsDisposed)
      return;
    int index = this.toolStrip.Items.IndexOf((ToolStripItem) this.toolBarButtonOpen) + 1;
    this.toolStrip.Items.Insert(index, (ToolStripItem) this.toolStripButtonSeekTime);
    this.toolStrip.Items.Insert(index, (ToolStripItem) this.toolBarButtonSeekEnd);
    this.toolStrip.Items.Insert(index, (ToolStripItem) this.toolBarButtonFastForward);
    this.toolStrip.Items.Insert(index, (ToolStripItem) this.toolBarTrackBarSeek);
    this.toolStrip.Items.Insert(index, (ToolStripItem) this.toolBarButtonRewind);
    this.toolStrip.Items.Insert(index, (ToolStripItem) this.toolBarButtonSeekStart);
    this.toolStrip.Items.Insert(index, (ToolStripItem) this.toolBarButtonStop);
    this.toolStrip.Items.Insert(index, (ToolStripItem) this.toolBarButtonPlayPause);
    (this.toolStrip.Parent as Panel).MinimumSize = new Size(0, 0);
  }

  private void UpdateUIStatus()
  {
    this.UpdateCommands();
    this.UpdateTitleStatus();
  }

  private void UpdateCommands()
  {
    bool online = this.sapiManager.Online;
    bool flag1 = this.logFile == null;
    bool flag2 = !online && !flag1 && this.logFile.Playing;
    double num1 = flag1 ? 0.0 : this.logFile.PlaybackSpeed;
    bool flag3 = !flag1 && this.logFile.CurrentTime == this.logFile.EndTime;
    bool flag4 = !flag1 && this.logFile.CurrentTime == this.logFile.StartTime;
    this.logFileInfo.Enabled = !flag1;
    if (flag1)
    {
      if (((ReadOnlyCollection<Channel>) this.sapiManager.ActiveChannels).Count == 0)
      {
        this.menuClose.Enabled = false;
      }
      else
      {
        this.menuClose.Text = Resources.CommandCloseConnections;
        this.menuClose.Enabled = true;
      }
    }
    else
    {
      this.menuClose.Text = Resources.CommandCloseLog;
      this.menuClose.Enabled = true;
    }
    this.menuConnect.Enabled = SapiManager.GlobalInstance.Sapi.InitState == 1 && !AdapterInformation.AdapterProhibited;
    this.toolBarButtonOpen.Enabled = true;
    this.toolBarButtonUserEvent.Enabled = this.menuLogEditableLabel.Enabled = this.menuLogLabel.Enabled = this.CanLabel;
    if (this.CanLabel)
    {
      this.menuLogUpload.Checked = this.toolBarButtonLogUpload.Checked = ServerDataManager.GlobalInstance.IsMarkedForUpload(SapiManager.GlobalInstance.CurrentLogFileInformation.LogFilePath);
      this.menuLogUpload.Enabled = this.toolBarButtonLogUpload.Enabled = !this.menuLogUpload.Checked;
    }
    else
      this.menuLogUpload.Checked = this.menuLogUpload.Enabled = this.toolBarButtonLogUpload.Enabled = this.toolBarButtonLogUpload.Checked = false;
    if (this.CanLabel)
    {
      this.switchDataMiningProcessTagToolStripMenuItem.Enabled = this.toolStripButtonDataMiningTag.Enabled = true;
      this.switchDataMiningProcessTagToolStripMenuItem.Checked = this.toolStripButtonDataMiningTag.Checked = !SapiManager.DataMiningProcessTag;
      SapiManager.UpdateDataMiningWarning(SapiManager.DataMiningProcessTag);
    }
    else
    {
      this.switchDataMiningProcessTagToolStripMenuItem.Enabled = this.toolStripButtonDataMiningTag.Enabled = false;
      this.switchDataMiningProcessTagToolStripMenuItem.Checked = this.toolStripButtonDataMiningTag.Checked = false;
      SapiManager.UpdateDataMiningWarning(true);
    }
    this.menuLogPlayPause.Text = !flag2 ? Resources.CommandPlay : Resources.CommandPause;
    this.menuLogPlayPause.Enabled = !online && !flag1;
    this.toolBarButtonPlayPause.Enabled = !online && !flag1;
    string b = !flag2 ? Resources.TooltipPlay : Resources.TooltipPause;
    if (!string.Equals(this.toolBarButtonPlayPause.ToolTipText, b))
    {
      this.toolBarButtonPlayPause.ToolTipText = b;
      if (flag2)
      {
        this.toolBarButtonPlayPause.Image = (Image) Resources.log_pause;
        this.menuLogPlayPause.Image = (Image) Resources.log_pause;
      }
      else
      {
        this.toolBarButtonPlayPause.Image = (Image) Resources.log_play;
        this.menuLogPlayPause.Image = (Image) Resources.log_play;
      }
    }
    this.menuFile.Enabled = !this.CurrentlyFlashing();
    this.menuLogStop.Enabled = !online && (flag2 || !flag4 && !flag3);
    this.toolBarButtonStop.Enabled = !online && (flag2 || !flag4 && !flag3);
    this.menuLogSpeedX1.Checked = flag2 && num1 == 1.0;
    this.menuLogSpeedX2.Checked = flag2 && num1 == 2.0;
    this.menuLogSpeedX4.Checked = flag2 && num1 == 4.0;
    this.menuLogSpeedX8.Checked = flag2 && num1 == 8.0;
    this.menuLogSpeedX1B.Checked = flag2 && num1 == -1.0;
    this.menuLogSpeedX2B.Checked = flag2 && num1 == -2.0;
    this.menuLogSpeedX4B.Checked = flag2 && num1 == -4.0;
    this.menuLogSpeedX8B.Checked = flag2 && num1 == -8.0;
    this.menuLogSpeedX1.Enabled = !online && !flag1 && !flag3;
    this.menuLogSpeedX2.Enabled = !online && !flag1 && !flag3;
    this.menuLogSpeedX4.Enabled = !online && !flag1 && !flag3;
    this.menuLogSpeedX8.Enabled = !online && !flag1 && !flag3;
    this.menuLogSpeedX1B.Enabled = !online && !flag1 && !flag4;
    this.menuLogSpeedX2B.Enabled = !online && !flag1 && !flag4;
    this.menuLogSpeedX4B.Enabled = !online && !flag1 && !flag4;
    this.menuLogSpeedX8B.Enabled = !online && !flag1 && !flag4;
    this.toolBarButtonRewind.Enabled = !online && !flag1 && !flag4;
    this.toolBarButtonRewind.Checked = flag2 && num1 < 0.0;
    this.toolBarButtonSeekStart.Enabled = !online && !flag1 && !flag4;
    this.menuLogSeekStart.Enabled = !online && !flag1 && !flag4;
    this.menuLogSeekLabel.Enabled = !online && !flag1;
    this.toolStripButtonSeekTime.Enabled = !online && !flag1;
    this.toolBarButtonFastForward.Enabled = !online && !flag1 && !flag3;
    this.toolBarButtonFastForward.Checked = flag2 && num1 > 1.0;
    this.toolBarButtonSeekEnd.Enabled = !online && !flag1 && !flag3;
    this.menuLogSeekEnd.Enabled = !online && !flag1 && !flag3;
    ((ToolStripItem) this.toolBarTrackBarSeek).Enabled = !online && !flag1;
    if (!flag1 && !this.toolBarTrackBarSeek.HasCapture)
      this.toolBarTrackBarSeek.Value = Math.Round((this.logFile.CurrentTime - this.logFile.StartTime).TotalSeconds, MidpointRounding.AwayFromZero);
    this.toolStripButtonRetryAutoConnect.Enabled = SapiManager.GlobalInstance.Sapi.InitState == 1 && SapiManager.GlobalInstance.AutoConnectNeedsRefresh;
    this.editSupport.SetTarget((object) this.ActiveControl);
    this.toolBarButtonCut.Enabled = this.menuCut.Enabled = this.editSupport.CanCut;
    this.toolBarButtonCopy.Enabled = this.menuCopy.Enabled = this.editSupport.CanCopy;
    this.toolBarButtonPaste.Enabled = this.menuPaste.Enabled = this.editSupport.CanPaste;
    this.toolBarButtonUndo.Enabled = this.menuUndo.Enabled = this.editSupport.CanUndo;
    this.menuSelectAll.Enabled = this.editSupport.CanSelectAll;
    this.menuDelete.Enabled = this.editSupport.CanDelete;
    this.toolStripButtonPrint.Enabled = this.menuPrint.Enabled = this.menuPrintPreview.Enabled = this.CanPrint;
    this.menuPrintFaults.Enabled = ((IEnumerable<Channel>) this.sapiManager.ActiveChannels).Count<Channel>() > 0;
    this.refreshToolStripMenuItem.Enabled = this.toolBarButtonRefresh.Enabled = this.CanRefreshView;
    this.toolStripButtonExpandAll.Enabled = this.CanExpandAllItems;
    this.toolStripButtonCollapseAll.Enabled = this.CanCollapseAllItems;
    if (ApplicationInformation.Branding.CanViewUnitsSystemToolBarButton)
    {
      this.toolStripButtonUnitsSystem.Visible = true;
      this.toolStripSeparator17.Visible = true;
      this.toolStripButtonUnitsSystem.Enabled = true;
      this.UpdateToolStripButtonUnitsSystemTooltip();
    }
    else
    {
      this.toolStripButtonUnitsSystem.Visible = false;
      this.toolStripSeparator17.Visible = false;
      this.toolStripButtonUnitsSystem.Enabled = false;
    }
    this.menuItemHelp.Enabled = this.CanShowHelp(MainForm.HelpAction.Main);
    this.contextHelpToolStripMenuItem.Enabled = this.CanShowHelp(MainForm.HelpAction.Context);
    ToolStripMenuItem vehicleInformation = this.menuItemVehicleInformation;
    Link link = LinkSupport.VehicleInformationHome;
    int num2 = !((Link) ref link).IsBroken ? 1 : 0;
    vehicleInformation.Enabled = num2 != 0;
    ToolStripMenuItem menuItemCatalog = this.menuItemCatalog;
    link = LinkSupport.PartsCatalog;
    int num3 = !((Link) ref link).IsBroken ? 1 : 0;
    menuItemCatalog.Enabled = num3 != 0;
    ToolStripMenuItem menuItemDdecviai = this.menuItemDDECVIAI;
    link = LinkSupport.DDECVIAIManual;
    int num4 = !((Link) ref link).IsBroken ? 1 : 0;
    menuItemDdecviai.Enabled = num4 != 0;
    ToolStripMenuItem menuItemDdeC10Ai = this.menuItemDDEC10AI;
    link = LinkSupport.DDEC10AIManual;
    int num5 = !((Link) ref link).IsBroken ? 1 : 0;
    menuItemDdeC10Ai.Enabled = num5 != 0;
    ToolStripMenuItem fcccEngineSupport = this.menuItemFcccEngineSupport;
    link = LinkSupport.FcccEngineSupport;
    int num6 = !((Link) ref link).IsBroken ? 1 : 0;
    fcccEngineSupport.Enabled = num6 != 0;
    ToolStripMenuItem menuItemFcccOasis = this.menuItemFcccOasis;
    link = LinkSupport.FcccOasis;
    int num7 = !((Link) ref link).IsBroken ? 1 : 0;
    menuItemFcccOasis.Enabled = num7 != 0;
    ToolStripMenuItem itemFcccRvChassis = this.menuItemFcccRVChassis;
    link = LinkSupport.FcccRVChassis;
    int num8 = !((Link) ref link).IsBroken ? 1 : 0;
    itemFcccRvChassis.Enabled = num8 != 0;
    ToolStripMenuItem itemFcccS2B2Chassis = this.menuItemFcccS2B2Chassis;
    link = LinkSupport.FcccS2B2Chassis;
    int num9 = !((Link) ref link).IsBroken ? 1 : 0;
    itemFcccS2B2Chassis.Enabled = num9 != 0;
    ToolStripMenuItem menuItemFcccEconic = this.menuItemFcccEconic;
    link = LinkSupport.FcccEconic;
    int num10 = !((Link) ref link).IsBroken ? 1 : 0;
    menuItemFcccEconic.Enabled = num10 != 0;
    ToolStripMenuItem dataMiningReports = this.menuItemDataMiningReports;
    link = LinkSupport.DataMiningReports;
    int num11 = !((Link) ref link).IsBroken ? 1 : 0;
    dataMiningReports.Enabled = num11 != 0;
    this.menuItemZenZefiT.Visible = ApplicationInformation.ShowZenZefiTHelpLink;
    this.menuUpdate.Enabled = !ServerClient.GlobalInstance.InUse;
  }

  private bool CanLabel
  {
    get
    {
      bool canLabel = false;
      if (this.sapiManager != null && this.sapiManager.Sapi != null)
      {
        foreach (Channel channel in (ChannelBaseCollection) this.sapiManager.Sapi.Channels)
        {
          if (channel.Online && channel.CommunicationsState != 4)
          {
            canLabel = true;
            break;
          }
        }
      }
      return canLabel;
    }
  }

  private void UpdateTitleStatus()
  {
    string a = !this.sapiManager.Online ? (this.logFile == null ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatTitleOfflineNoLog, (object) ApplicationInformation.ProductName) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatTitleOfflineWithLogFile, (object) ApplicationInformation.ProductName, (object) Path.GetFileNameWithoutExtension(this.logFile.Name))) : ApplicationInformation.ProductName;
    if (string.Equals(a, this.title))
      return;
    this.Text = this.title = a;
  }

  private void UpdateLogTime()
  {
    if (this.logFile != null)
      this.statusStripPanelLogTime.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatLogTimeStatus, (object) this.logFile.CurrentTime.ToString((IFormatProvider) CultureInfo.CurrentCulture));
    else
      this.statusStripPanelLogTime.Text = string.Empty;
  }

  private void logFile_LogFilePlaybackUpdateEvent(object sender, EventArgs e)
  {
    this.UpdateLogTime();
  }

  private void sapiManager_LogFileChannelsChanged(object sender, EventArgs e)
  {
    this.UpdateLogFile();
  }

  private void sapiManager_ActiveChannelsChanged(object sender, EventArgs e)
  {
    this.UpdateLogFile();
  }

  private void sapiManager_EquipmentTypeChanged(object sender, EquipmentTypeChangedEventArgs e)
  {
    MainForm.ShowEquipmentTypeSpecificWarnings(e.Category);
  }

  private void UpdateLogFile()
  {
    if (this.logFile != null)
      this.logFile.LogFilePlaybackUpdateEvent -= new LogFilePlaybackUpdateEventHandler(this.logFile_LogFilePlaybackUpdateEvent);
    if (this.sapiManager.Online)
      this.logFile = (LogFile) null;
    else if (this.logFile != this.sapiManager.LogFileChannels.LogFile)
    {
      this.logFile = this.sapiManager.LogFileChannels.LogFile;
      this.UpdateLogSeekBar();
    }
    if (this.logFile != null)
      this.logFile.LogFilePlaybackUpdateEvent += new LogFilePlaybackUpdateEventHandler(this.logFile_LogFilePlaybackUpdateEvent);
    this.UpdateLogTime();
  }

  private void ShowLogFileInfoDialog()
  {
    using (LogFileInfoDialog logFileInfoDialog = new LogFileInfoDialog(this.logFile))
    {
      int num = (int) logFileInfoDialog.ShowDialog((IWin32Window) this);
    }
  }

  private void UpdateLogSeekBar()
  {
    if (this.logFile == null)
      return;
    this.toolBarTrackBarSeek.Minimum = 0.0;
    this.toolBarTrackBarSeek.Maximum = Math.Round((this.logFile.EndTime - this.logFile.StartTime).TotalSeconds, MidpointRounding.AwayFromZero);
    this.toolBarTrackBarSeek.SmallChange = this.toolBarTrackBarSeek.Maximum / 10.0;
    this.toolBarTrackBarSeek.LargeChange = this.toolBarTrackBarSeek.Maximum / 5.0;
  }

  protected override void OnSizeChanged(EventArgs e)
  {
    if (this.WindowState == FormWindowState.Normal && !this.menuFullscreen.Checked)
      this.lastKnownGoodSize = this.Size;
    base.OnSizeChanged(e);
    this.ArrangeToolbars();
  }

  protected override void OnLocationChanged(EventArgs e)
  {
    if (this.WindowState == FormWindowState.Normal && !this.menuFullscreen.Checked)
      this.lastKnownGoodLocation = this.Location;
    base.OnLocationChanged(e);
  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    Application.Idle += new EventHandler(this.Application_Idle);
    Converter.GlobalInstance.SelectedUnitSystem = SapiManager.GlobalInstance.UserUnits;
    this.LoadSettings((ISettings) SettingsManager.GlobalInstance);
    if (ApplicationInformation.ConnectToServerAutomaticallyAtStartup)
      ServerClient.GlobalInstance.Go((Collection<UnitInformation>) null, (Collection<UnitInformation>) null, (IProgressBar) this);
    else
      ServerClient.GlobalInstance.SetProgressBar((IProgressBar) this);
    using (Form form = (Form) new CautionDialog())
    {
      int num = (int) form.ShowDialog((IWin32Window) this);
    }
    if (this.IsDisposed)
      return;
    this.CheckRequiredMinimumPerformance();
    if (this.IsDisposed)
      return;
    this.CheckForProhibitedAdapters();
    this.CheckForBluetoothAdapters();
    if (!this.sapiManager.LogFileIsOpen)
      this.CheckRollCallStatus();
    SapiManager.GlobalInstance.RollCallEnabledChanged += new EventHandler(this.SapiManager_RollCallEnabledChanged);
    this.CheckAutoBaudRate();
    this.CheckConnectionHardware();
    MainForm.CheckSidVersion();
    if (this.IsDisposed)
      return;
    this.CheckServerRegistration();
    ServerClient.GlobalInstance.Complete += new EventHandler<ClientConnectionCompleteEventArgs>(this.OnServerClientComplete);
    LargeFileDownloadManager.GlobalInstance.DownloadFilesComplete += new EventHandler<OperationCompleteEventArgs>(this.GlobalInstance_DownloadFilesComplete);
    MainForm.DisplayProgrammingEventsAvailabiltyWarning();
    MainForm.UpdateToolLicenseInformation();
    if (this.IsDisposed)
      return;
    ExtractionManager.GlobalInstance.Init();
    if (ApplicationInformation.CanAccessVehicleInformationLink)
      this.menuItemVehicleInformation.Visible = true;
    this.menuItemDataMiningReports.Visible = ApplicationInformation.CanSetDataMiningTag;
    this.switchDataMiningProcessTagToolStripMenuItem.Visible = ApplicationInformation.CanSetDataMiningTag;
    this.toolStripButtonDataMiningTag.Visible = ApplicationInformation.CanSetDataMiningTag;
    this.menuTroubleshootingGuides.Text = Resources.TroubleshootingGuides;
    ITroubleshooting troubleshooting = ((IContainerApplication) this).Troubleshooting;
    this.menuTroubleshootingGuides.Enabled = troubleshooting != null && troubleshooting.CanViewCollections();
    if (troubleshooting != null)
      troubleshooting.ContentUpdated += new EventHandler(this.troubleshooting_ContentUpdated);
    this.menuTroubleshootingGuides.Click += new EventHandler(this.OnTroubleshootingGuides);
    this.sapiManager.StatusCallback = new Action<string>(this.UpdateSapiManagerStatus);
    if (string.IsNullOrEmpty(this.logFileToLoad))
      this.sapiManager.RefreshAutoConnect();
    this.LoadSettingsFilterList();
    if (ApplicationForceUpgrade.ProductUpgradeRequired)
      WarningsPanel.GlobalInstance.Add("ApplicationKill", MessageBoxIcon.Hand, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_ProductUpdateRequired, (object) ApplicationInformation.ProductName), string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_ProductUpdateRequiredContent, (object) ApplicationForceUpgrade.RequiredUpgradeVersion, (object) ApplicationForceUpgrade.RequiredUpgradeOriginalReleaseDate.ToString("Y", (IFormatProvider) CultureInfo.CurrentCulture), (object) ApplicationForceUpgrade.ForceUpgradeDate.ToLongDateString()), (EventHandler) null);
    if (this.logFile != null && this.logFile.Summary)
    {
      this.ShowLogFileInfoDialog();
      if (this.IsDisposed)
        return;
    }
    ViewHistory.GlobalInstance.HistoryChanged += new EventHandler(this.OnHistoryChanged);
    SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(this.SystemEvents_PowerModeChanged);
    if (string.IsNullOrEmpty(this.logFileToLoad))
      return;
    this.BeginInvoke((Delegate) (() => this.LogOpen(this.logFileToLoad)));
  }

  private void GlobalInstance_DownloadFilesComplete(object sender, OperationCompleteEventArgs e)
  {
    if ((sender as LargeFileDownload).LargeFileDownloadType != 1)
      return;
    Link.ResetIsBroken();
    this.menuItemHelp.Enabled = this.CanShowHelp(MainForm.HelpAction.Main);
    this.contextHelpToolStripMenuItem.Enabled = this.CanShowHelp(MainForm.HelpAction.Context);
    this.mainTabView.ContextLinkButton.UpdateState();
  }

  private void troubleshooting_ContentUpdated(object sender, EventArgs e)
  {
    this.menuTroubleshootingGuides.Enabled = ((IContainerApplication) this).Troubleshooting.CanViewCollections();
  }

  private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
  {
    if (!SapiManager.GlobalInstance.Online || e.Mode != PowerModes.Suspend && e.Mode != PowerModes.Resume)
      return;
    this.CloseAllConnections();
  }

  private void CheckRequiredMinimumPerformance()
  {
    ulong num = 128 /*0x80*/;
    ComputerInfo computerInfo = new ComputerInfo();
    if (computerInfo != null && computerInfo.TotalPhysicalMemory / 1048576UL /*0x100000*/ < ApplicationInformation.ProductMinimumRequiredRam - num)
      WarningMessageBox.WarnMinimumRequiredRam((IWin32Window) this, ApplicationInformation.ProductName);
    IComponentInformation groupInformation = ComponentInformationGroups.GlobalInstance.GetGroupInformation(Components.GroupSystemInformation);
    if (groupInformation.Identifiers.Contains<string>(Components.InfoComputerSystemModel) && groupInformation.Identifiers.Contains<string>(Components.InfoComputerSystemManufacturer))
    {
      string makeModel = $"{groupInformation.GetValue(Components.InfoComputerSystemManufacturer)} {groupInformation.GetValue(Components.InfoComputerSystemModel)}";
      if (ApplicationInformation.ProhibitedSystemMakeModels.Any<string>((Func<string, bool>) (psm => Regex.Match(makeModel, psm).Value == makeModel)))
        WarningMessageBox.Show((IWin32Window) this, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_WarningSystemModel, (object) makeModel, (object) ApplicationInformation.ProductName), ApplicationInformation.ProductName, "WarningSystemModel");
    }
    ulong result1;
    ulong result2;
    if (!groupInformation.Identifiers.Contains<string>(Components.InfoProcessorName) || !groupInformation.Identifiers.Contains<string>(Components.InfoProcessorMaxClockSpeed) || !groupInformation.Identifiers.Contains<string>(Components.InfoProcessorNumberOfCores) || !ulong.TryParse(groupInformation.GetValue(Components.InfoProcessorMaxClockSpeed), out result1) || !ulong.TryParse(groupInformation.GetValue(Components.InfoProcessorNumberOfCores), out result2) || MainForm.IsProcessorPerformanceAdequate(groupInformation.GetValue(Components.InfoProcessorName), result2, result1))
      return;
    WarningMessageBox.Show((IWin32Window) this, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_WarningProcessor, (object) groupInformation.GetValue(Components.InfoProcessorName), (object) ApplicationInformation.ProductName), ApplicationInformation.ProductName, "WarningProcessor");
  }

  private static bool IsProcessorPerformanceAdequate(
    string processorName,
    ulong numberCores,
    ulong maxClockSpeed)
  {
    if (processorName != null)
    {
      if (((IEnumerable<string>) new string[3]
      {
        "Core(TM) i5",
        "Core(TM) i7",
        "Core(TM) i9"
      }).Any<string>((Func<string, bool>) (p => processorName.IndexOf(p, StringComparison.Ordinal) != -1)))
        return true;
    }
    return numberCores > 2UL || maxClockSpeed >= 2000UL;
  }

  private void CheckForProhibitedAdapters()
  {
    if (AdapterInformation.AdapterProhibited)
    {
      if (AdapterInformation.AdapterProhibitedByVersion)
        WarningsPanel.GlobalInstance.Add("ProhibitedConnectionHardware", MessageBoxIcon.Hand, string.Empty, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Format_ProhibitedAdapterDriverVersionWarning, (object) string.Join(", ", AdapterInformation.GlobalInstance.ConnectedAdapterNames), (object) AdapterInformation.GlobalInstance.SelectedLibraryVersion, (object) ApplicationInformation.ProductName), (EventHandler) null);
      else
        WarningsPanel.GlobalInstance.Add("ProhibitedConnectionHardware", MessageBoxIcon.Hand, string.Empty, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Format_ProhibitedAdapterWarning, (object) string.Join(", ", AdapterInformation.GlobalInstance.ConnectedAdapterNames), (object) ApplicationInformation.ProductName), new EventHandler(this.connectionHardwareWarning_Click));
    }
    else
      WarningsPanel.GlobalInstance.Remove("ProhibitedConnectionHardware");
  }

  private void CheckForBluetoothAdapters()
  {
    string performanceWarning = AdapterInformation.AdapterBluetoothPerformanceWarning;
    if (!string.IsNullOrEmpty(performanceWarning))
      WarningsPanel.GlobalInstance.Add("BluetoothPerformanceWarning", (MessageBoxIcon) Enum.Parse(typeof (MessageBoxIcon), performanceWarning, true), string.Empty, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Format_BluetoothPerformanceWarning, (object) string.Join(", ", AdapterInformation.GlobalInstance.ConnectedAdapterNames), (object) ApplicationInformation.ProductName), new EventHandler(this.connectionHardwareWarning_Click));
    else
      WarningsPanel.GlobalInstance.Remove("BluetoothPerformanceWarning");
  }

  private void CheckRollCallStatus()
  {
    if (!ApplicationInformation.Branding.OmitWarnings && !this.sapiManager.RollCallEnabled && AdapterInformation.SupportsConcurrentProtocols != 1)
      WarningsPanel.GlobalInstance.Add("RollCallDisabled", MessageBoxIcon.Exclamation, string.Empty, Resources.Message_RollCallDisabled, new EventHandler(this.connectionHardwareWarning_Click));
    else
      WarningsPanel.GlobalInstance.Remove("RollCallDisabled");
  }

  private void SapiManager_RollCallEnabledChanged(object sender, EventArgs e)
  {
    this.CheckRollCallStatus();
  }

  private void CheckAutoBaudRate()
  {
    if (AdapterInformation.SupportsAutomaticBaudRate != 1)
      return;
    WarningMessageBox.WarnAutoBaudRateUnavailable((IWin32Window) this, ApplicationInformation.ProductName, string.Join(", ", AdapterInformation.GlobalInstance.ConnectedAdapterNames));
  }

  private static void CheckSidVersion()
  {
    string sidDll = Program.GetSidDll();
    Version result;
    if (File.Exists(sidDll) && Version.TryParse(FileVersionInfo.GetVersionInfo(sidDll).FileVersion, out result))
    {
      Version version = Version.Parse(Assembly.GetExecutingAssembly().GetCustomAttribute<SidVersionAttribute>().Version);
      if (!(result < version))
        return;
      WarningsPanel.GlobalInstance.Add("SidVersion", MessageBoxIcon.Exclamation, string.Empty, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_SidRollback, (object) version, (object) result), (EventHandler) null);
    }
    else
      WarningsPanel.GlobalInstance.Add("SidVersion", MessageBoxIcon.Hand, string.Empty, Resources.MessageFormat_SidNotFound, (EventHandler) null);
  }

  private void CheckConnectionHardware()
  {
    if (!AdapterInformation.GlobalInstance.ConnectedAdapterNames.Any<string>())
    {
      WarningsPanel.GlobalInstance.Add("NoConnectionHardware", MessageBoxIcon.Exclamation, string.Empty, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_NoConnectionHardwareWarning, (object) ApplicationInformation.ProductName), new EventHandler(this.connectionHardwareWarning_Click));
    }
    else
    {
      WarningsPanel.GlobalInstance.Remove("NoConnectionHardware");
      if (AdapterInformation.SupportsConcurrentProtocols != 1)
        return;
      WarningMessageBox.WarnRollCallDisabled((IWin32Window) this, ApplicationInformation.ProductName);
    }
  }

  private void connectionHardwareWarning_Click(object sender, EventArgs e)
  {
    int num = (int) new OptionsDialog((OptionsPanel) new ConnectionOptionsPanel()).ShowDialog();
  }

  private static void DisplayProgrammingEventsAvailabiltyWarning()
  {
    int? nullable1 = ServerDataManager.GlobalInstance.ToolLicenseInformation != null ? ServerDataManager.GlobalInstance.ToolLicenseInformation.ToolLicenseTerms.ProgrammingEventsLeft : new int?();
    if (nullable1.HasValue)
    {
      int? nullable2 = nullable1;
      int num1 = 0;
      if ((nullable2.GetValueOrDefault() == num1 ? (nullable2.HasValue ? 1 : 0) : 0) != 0)
      {
        WarningsPanel.GlobalInstance.Add("ProgrammingEventsAllowed", MessageBoxIcon.Exclamation, string.Empty, Resources.NoProgrammingEventsLeft, (EventHandler) null);
      }
      else
      {
        nullable2 = nullable1;
        int num2 = 3;
        if ((nullable2.GetValueOrDefault() <= num2 ? (nullable2.HasValue ? 1 : 0) : 0) != 0)
          WarningsPanel.GlobalInstance.Add("ProgrammingEventsAllowed", MessageBoxIcon.Exclamation, string.Empty, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_ProgrammingEventsLeft, (object) nullable1), (EventHandler) null);
        else
          WarningsPanel.GlobalInstance.Remove("ProgrammingEventsAllowed");
      }
    }
    else
      WarningsPanel.GlobalInstance.Remove("ProgrammingEventsAllowed");
  }

  private static string FormatDaysRemaining(TimeSpan period)
  {
    int int32 = Convert.ToInt32(period.TotalDays);
    return int32 > 1 ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatConnectionRequiredWithnXDays, (object) int32) : (int32 > 0 ? Resources.ConnectionRequiredWithin1Day : Resources.ConnectionRequiredNow);
  }

  private static void UpdateLastUpdateInformation()
  {
    TimeZone currentTimeZone = TimeZone.CurrentTimeZone;
    IComponentInformation icomponentInformation1 = (IComponentInformation) null;
    if (!ComponentInformationGroups.GlobalInstance.GroupIdentifiers.Contains<string>(Components.GroupLastUpdateInfo))
    {
      icomponentInformation1 = (IComponentInformation) new ComponentInformation();
      icomponentInformation1.Add(Components.InfoLastServerConnection, Resources.LastUpdateCheckInformation_KeyName, string.Empty, true);
      icomponentInformation1.Add(Components.InfoServerConnectionRequired, Resources.InfoDaysTilConnectionExpiration, string.Empty, true);
      ComponentInformationGroups.GlobalInstance.Add(Components.GroupLastUpdateInfo, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.LastUpdateInformation_Groupname), icomponentInformation1);
    }
    string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatLastServerUpdateTimeandTimeZone, (object) ServerClient.GlobalInstance.LastUpdateCheck.ToString((IFormatProvider) CultureInfo.CurrentCulture), (object) currentTimeZone.DaylightName.ToString((IFormatProvider) CultureInfo.CurrentCulture));
    IComponentInformation icomponentInformation2 = icomponentInformation1 ?? ComponentInformationGroups.GlobalInstance.GetGroupInformation(Components.GroupLastUpdateInfo);
    icomponentInformation2.UpdateValue(Components.InfoLastServerConnection, str, true);
    TimeSpan period = ServerRegistration.GlobalInstance.ExpirationDate - DateTime.Today;
    icomponentInformation2.UpdateValue(Components.InfoServerConnectionRequired, MainForm.FormatDaysRemaining(period), true);
  }

  private static void UpdateToolLicenseInformation()
  {
    if (ServerDataManager.GlobalInstance.ToolLicenseInformation == null)
      return;
    IComponentInformation icomponentInformation1 = (IComponentInformation) null;
    if (!ComponentInformationGroups.GlobalInstance.GroupIdentifiers.Contains<string>(Components.GroupToolLicenseExpiration))
    {
      icomponentInformation1 = (IComponentInformation) new ComponentInformation((IList<ICommand>) ((IEnumerable<ICommand>) new ICommand[1]
      {
        (ICommand) new ToolLicenseInformationExportToolLicenseCommand()
      }).ToList<ICommand>());
      icomponentInformation1.Add(Components.InfoToolLicenseExpirationDate, Resources.ToolLicenseExpirationDate, string.Empty, true);
      icomponentInformation1.Add(Components.InfoProgrammingEventsLeft, Resources.RemainingProgrammingEvents, string.Empty, true);
      ComponentInformationGroups.GlobalInstance.Add(Components.GroupToolLicenseExpiration, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ToolLicenseInfo_Groupname), icomponentInformation1);
    }
    IComponentInformation icomponentInformation2 = icomponentInformation1 ?? ComponentInformationGroups.GlobalInstance.GetGroupInformation(Components.GroupToolLicenseExpiration);
    icomponentInformation2.UpdateValue(Components.InfoToolLicenseExpirationDate, ServerDataManager.GlobalInstance.ToolLicenseInformation.ToolLicenseTerms.ExpirationDate.ToString((IFormatProvider) CultureInfo.CurrentCulture), true);
    int? programmingEventsLeft = ServerDataManager.GlobalInstance.ToolLicenseInformation.ToolLicenseTerms.ProgrammingEventsLeft;
    if (programmingEventsLeft.HasValue)
    {
      int? nullable = programmingEventsLeft;
      int num = 9999;
      if ((nullable.GetValueOrDefault() == num ? (nullable.HasValue ? 1 : 0) : 0) != 0)
        icomponentInformation2.UpdateValue(Components.InfoProgrammingEventsLeft, Resources.Unlimited, true);
      else
        icomponentInformation2.UpdateValue(Components.InfoProgrammingEventsLeft, programmingEventsLeft.ToString(), true);
    }
    else
      icomponentInformation2.UpdateValue(Components.InfoProgrammingEventsLeft, "0", false);
  }

  private void OnServerClientComplete(object sender, ClientConnectionCompleteEventArgs e)
  {
    if (e.Status == null)
      return;
    this.CheckServerRegistration();
    ServerDataManager.GlobalInstance.AddCrashHandlerInfo();
    ComponentInformationGroups.GlobalInstance.GetGroupInformation(Components.GroupSystemInformation)?.UpdateValue(Components.InfoComputerDescription, ServerRegistration.GlobalInstance.ComputerDescription, true);
    MainForm.DisplayProgrammingEventsAvailabiltyWarning();
    MainForm.UpdateToolLicenseInformation();
  }

  private void CheckServerRegistration()
  {
    this.ShowServerRegistrationMessage();
    if (!ServerRegistration.GlobalInstance.Valid && !this.dialoginuse)
    {
      this.dialoginuse = true;
      if (!ServerRegistrationDialog.ShowRegistrationDialog() && !this.IsDisposed && this.IsHandleCreated)
        this.BeginInvoke((Delegate) (() => this.Close()));
      this.dialoginuse = false;
    }
    MainForm.UpdateToolLicenseInformation();
    MainForm.UpdateLastUpdateInformation();
  }

  private static void ShowEquipmentTypeSpecificWarnings(string category)
  {
    string specificWarnings = ApplicationInformation.GetEquipmentTypeSpecificWarnings(category);
    if (!string.IsNullOrEmpty(specificWarnings))
      WarningsPanel.GlobalInstance.Add("EquipmentTypeSpecificWarning" + category, MessageBoxIcon.None, string.Empty, specificWarnings, (EventHandler) null);
    else
      WarningsPanel.GlobalInstance.Remove("EquipmentTypeSpecificWarning" + category);
  }

  private void ShowServerRegistrationMessage()
  {
    if (ServerRegistration.GlobalInstance.Valid)
    {
      TimeSpan timeSpan1 = ServerRegistration.GlobalInstance.ToolLicenseExpirationDate - DateTime.Today;
      if (timeSpan1.TotalDays < (double) ApplicationInformation.SoftwareLicenseExpirationWarningPeriodDays)
        WarningsPanel.GlobalInstance.Add("ServerRegistrationFixedExpiry", timeSpan1.TotalDays <= (double) ApplicationInformation.SoftwareLicenseExpirationErrorPeriodDays ? MessageBoxIcon.Hand : MessageBoxIcon.Exclamation, string.Empty, string.Format((IFormatProvider) CultureInfo.CurrentCulture, ApplicationInformation.RegistrationIsManagedByOrderingSite ? Resources.FormatRegistrationSetToExpireFixedDate : Resources.FormatRegistrationSetToExpireFixedDateInternal, (object) ServerRegistration.GlobalInstance.ToolLicenseExpirationDate.ToLongDateString()), new EventHandler(this.OnMenuUpdateClick));
      else
        WarningsPanel.GlobalInstance.Remove("ServerRegistrationFixedExpiry");
      TimeSpan timeSpan2 = DateTime.Now - ServerClient.GlobalInstance.LastUpdateCheck;
      if (ServerRegistration.GlobalInstance.InLimitedFunctionalityPeriod)
      {
        TimeSpan period = ServerRegistration.GlobalInstance.CompleteLossOfFunctionalityDate - DateTime.Today;
        WarningsPanel.GlobalInstance.Add("ServerRegistration", MessageBoxIcon.Hand, string.Empty, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatRegistrationInLossOfFunctionalityMode, (object) Convert.ToInt32(timeSpan2.TotalDays), (object) MainForm.FormatDaysRemaining(period)), new EventHandler(this.OnMenuUpdateClick));
      }
      else
      {
        int num = 5;
        if (ServerDataManager.GlobalInstance.ToolLicenseInformation != null && ServerDataManager.GlobalInstance.ToolLicenseInformation.ToolLicenseTerms.ShowWarningOnXDaysBeforeExpiration.HasValue)
          num = ServerDataManager.GlobalInstance.ToolLicenseInformation.ToolLicenseTerms.ShowWarningOnXDaysBeforeExpiration.Value;
        TimeSpan period = ServerRegistration.GlobalInstance.ExpirationDate - DateTime.Today;
        if (period.TotalDays >= 0.0 && period.TotalDays <= (double) num)
          WarningsPanel.GlobalInstance.Add("ServerRegistration", MessageBoxIcon.Exclamation, string.Empty, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatRegistrationSetToExpire, (object) MainForm.FormatDaysRemaining(period)), new EventHandler(this.OnMenuUpdateClick));
        else if (timeSpan2.TotalDays > 30.0)
          WarningsPanel.GlobalInstance.Add("ServerRegistration", MessageBoxIcon.Exclamation, string.Empty, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatLastUpdateCheckWasDaysAgoCheckForUpdates, (object) ApplicationInformation.ProductName, (object) Convert.ToInt32(timeSpan2.TotalDays)), new EventHandler(this.OnMenuUpdateClick));
        else
          WarningsPanel.GlobalInstance.Remove("ServerRegistration");
      }
      if (!string.IsNullOrEmpty(ServerRegistration.GlobalInstance.UserMessage))
        WarningsPanel.GlobalInstance.Add("UserMessage", MessageBoxIcon.Exclamation, string.Empty, string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerRegistration.GlobalInstance.UserMessage), (EventHandler) null);
      else
        WarningsPanel.GlobalInstance.Remove("UserMessage");
      if (!string.IsNullOrEmpty(ServerRegistration.GlobalInstance.NewVersion) && new Version(ServerRegistration.GlobalInstance.NewVersion) > Assembly.GetEntryAssembly().GetName().Version)
        WarningsPanel.GlobalInstance.Add("NewVersion", MessageBoxIcon.Exclamation, (string) null, string.Format((IFormatProvider) CultureInfo.CurrentCulture, !ApplicationInformation.NewVersionDownloadIsManagedByOrderingSite ? Resources.Format_NewVersionAvailable : Resources.Format_NewVersionAvailableService, (object) ApplicationInformation.ProductName, (object) ServerRegistration.GlobalInstance.NewVersion), !ApplicationInformation.NewVersionDownloadIsManagedByOrderingSite ? (EventHandler) null : new EventHandler(this.warningPanel_NewVersionClick));
      else
        WarningsPanel.GlobalInstance.Remove("NewVersion");
    }
    else
      WarningsPanel.GlobalInstance.Remove("ServerRegistration");
  }

  private void warningPanel_NewVersionClick(object sender, EventArgs e)
  {
    Process.Start(ServerRegistration.GlobalInstance.NewVersionLink);
  }

  protected override void OnFormClosed(FormClosedEventArgs e)
  {
    Application.Idle -= new EventHandler(this.Application_Idle);
    this.SaveSettings((ISettings) SettingsManager.GlobalInstance);
    base.OnFormClosed(e);
  }

  private void menuAbout_Click(object sender, EventArgs e)
  {
    AboutDialog aboutDialog = new AboutDialog((IComponentInformationGroups) ComponentInformationGroups.GlobalInstance);
    int num = (int) ((Form) aboutDialog).ShowDialog();
    ((Component) aboutDialog).Dispose();
  }

  private void prevMenu_Click(object sender, EventArgs e) => this.mainTabView.SelectPrevious();

  private void nextMenu_Click(object sender, EventArgs e) => this.mainTabView.SelectNext();

  private void findNextMenu_Click(object sender, EventArgs e) => this.findBar.Find((FindMode) 0);

  private void findPreviousMenu_Click(object sender, EventArgs e)
  {
    this.findBar.Find((FindMode) 1);
  }

  private void findMenu_Click(object sender, EventArgs e) => ((Control) this.findBar).Focus();

  private void Application_Idle(object sender, EventArgs e)
  {
    int num = Environment.TickCount & int.MaxValue;
    if (num <= this.lastUpdate + 100 || this.IsDisposed || CrashHandler.Reporting)
      return;
    this.UpdateUIStatus();
    this.findBar.UpdateUIStatus();
    this.lastUpdate = num;
  }

  private void OnTroubleshootingGuides(object sender, EventArgs e)
  {
    ((IContainerApplication) this).Troubleshooting?.StartViewingCollections();
  }

  private void OnMenuPowerTrainGuidesClick(object sender, EventArgs e)
  {
    ((IContainerApplication) this).Troubleshooting?.StartViewingCollections();
  }

  private void OnMenuFaultCodesClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.VehicleFaultCodesTroubleshootingManual);
  }

  private void OnMenuElectricalSystemClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.VehicleElectricalSystemTroubleshootingManual);
  }

  private void menuItemDtnaConnect_Click(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.DtnaConnect);
  }

  private void menuItemZenZefiT_Click(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.ZenZefiT);
  }

  private void OnMenuDataLinkClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.VehicleDataLinkTroubleshootingManual);
  }

  private void OnMenuInstrumentClusterClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.VehicleInstrumentTroubleshootingManual);
  }

  private void OnMenuHvacClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.VehicleHvacTroubleshootingManual);
  }

  private void OnMenuLightingClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.VehicleLightingSystemTroubleshootingManual);
  }

  private void OnMenuDomeLightingClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.VehicleDomeLightingTroubleshootingManual);
  }

  private void OnMenuWindshieldClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.VehicleWindshieldTroubleshootingManual);
  }

  private void OnMenuBackupLightsClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.VehicleBackupLightsTroubleshootingManual);
  }

  private void OnMenuAbsClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.VehicleAbsTroubleshootingManual);
  }

  private void OnMenuPowerSteeringClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.VehiclePowerSteeringTroubleshootingManual);
  }

  private void OnMenuStartingClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.VehicleStartingTroubleshootingManual);
  }

  private void OnMenuClutchClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.VehicleClutchTroubleshootingManual);
  }

  private void OnMenuCruiseControlClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.VehicleCruiseControlTroubleshootingManual);
  }

  private void OnMenuWiringDiagramIClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.VehicleWiringDiagramITroubleshootingManual);
  }

  private void OnMenuWiringDiagramIIClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.VehicleWiringDiagramIITroubleshootingManual);
  }

  public bool Search(string searchText, bool caseSensitive, FindMode direction)
  {
    return this.mainTabView.Search(searchText, caseSensitive, direction);
  }

  private void menuUndo_Click(object sender, EventArgs e) => this.Undo();

  private void menuCut_Click(object sender, EventArgs e) => this.Cut();

  private void menuCopy_Click(object sender, EventArgs e) => this.Copy();

  private void menuPaste_Click(object sender, EventArgs e) => this.Paste();

  private void menuDelete_Click(object sender, EventArgs e) => this.Delete();

  private void menuSelectAll_Click(object sender, EventArgs e) => this.SelectAll();

  public bool CanSearch => this.mainTabView.CanSearch;

  private void OnMenuPrintClick(object sender, EventArgs e) => this.Print();

  private void OnMenuPrintPreviewClick(object sender, EventArgs e) => this.PrintPreview();

  private void OnHelpClick(object sender, EventArgs e) => this.ShowHelp(MainForm.HelpAction.Main);

  private void OnContextHelpClick(object sender, EventArgs e)
  {
    this.ShowHelp(MainForm.HelpAction.Context);
  }

  private void ShowHelp(MainForm.HelpAction action)
  {
    if (!this.CanShowHelp(action))
      return;
    switch (action)
    {
      case MainForm.HelpAction.BestAvailable:
        if (this.CanShowHelp(MainForm.HelpAction.Context))
        {
          this.ShowHelp(MainForm.HelpAction.Context);
          break;
        }
        if (!this.CanShowHelp(MainForm.HelpAction.Main))
          break;
        this.ShowHelp(MainForm.HelpAction.Main);
        break;
      case MainForm.HelpAction.Main:
        Link.ShowTarget(LinkSupport.MainOnlineHelp);
        break;
      case MainForm.HelpAction.Context:
        Link.ShowTarget(this.mainTabView.ContextLink);
        break;
      default:
        throw new NotImplementedException("Unknown enumeration value");
    }
  }

  private bool CanShowHelp(MainForm.HelpAction action)
  {
    switch (action)
    {
      case MainForm.HelpAction.BestAvailable:
        return true;
      case MainForm.HelpAction.Main:
        Link mainOnlineHelp = LinkSupport.MainOnlineHelp;
        return !((Link) ref mainOnlineHelp).IsBroken;
      case MainForm.HelpAction.Context:
        Link contextLink = this.mainTabView.ContextLink;
        return !((Link) ref contextLink).IsBroken;
      default:
        throw new NotImplementedException("Unknown enumeration value");
    }
  }

  private void menuFullscreen_Click(object sender, EventArgs e)
  {
    this.SetFullscreen(!this.menuFullscreen.Checked);
  }

  private void menuItemFullScreen_Click(object sender, EventArgs e)
  {
    this.SetFullscreen(!this.menuFullscreen.Checked);
  }

  private void SetFullscreen(bool value)
  {
    this.SuspendLayout();
    if (value)
    {
      this.menuFullscreen.CheckState = CheckState.Checked;
      this.regularFormBorderStyle = this.FormBorderStyle;
      this.regularFormWindowState = this.WindowState;
      this.FormBorderStyle = FormBorderStyle.None;
      this.WindowState = FormWindowState.Minimized;
      this.WindowState = FormWindowState.Maximized;
      this.BringToFront();
      this.Activate();
    }
    else
    {
      this.menuFullscreen.CheckState = CheckState.Unchecked;
      this.FormBorderStyle = this.regularFormBorderStyle;
      this.WindowState = this.regularFormWindowState;
    }
    this.mainTabView.ShowSidebar = !value;
    this.Focus();
    this.ArrangeToolbars();
    this.UpdateConnectionPane();
    this.ResumeLayout();
  }

  private void ArrangeToolbars()
  {
    if (!this.menuFullscreen.Checked)
    {
      if (this.toolStripContainer.Contains((Control) this.navigationToolStrip))
        this.toolStripContainer.Controls.Remove((Control) this.navigationToolStrip);
      if (this.toolStripContainer.Contains((Control) this.toolStrip))
        this.toolStripContainer.Controls.Remove((Control) this.toolStrip);
      if (this.toolStripContainer.Contains((Control) this.findBar))
        this.toolStripContainer.Controls.Remove((Control) this.findBar);
      bool flag = false;
      if (!this.toolStripContainer2.Contains((Control) this.navigationToolStrip))
      {
        this.toolStripContainer2.Controls.Add((Control) this.navigationToolStrip);
        this.toolStrip.Dock = DockStyle.Fill;
        flag = true;
      }
      if (!this.toolStripContainer2.Contains((Control) this.toolStrip))
      {
        this.toolStripContainer2.Controls.Add((Control) this.toolStrip);
        this.toolStrip.Dock = DockStyle.Fill;
        flag = true;
      }
      if (!this.toolStripContainer2.Contains((Control) this.findBar))
      {
        this.toolStripContainer2.Controls.Add((Control) this.findBar);
        this.toolStrip.Dock = DockStyle.Fill;
        flag = true;
      }
      if (flag)
      {
        ((Control) this.findBar).BringToFront();
        this.fullScreenStrip.BringToFront();
        this.navigationToolStrip.BringToFront();
        this.menuStrip1.BringToFront();
        this.toolStrip.BringToFront();
      }
      this.toolStripContainer2.Visible = true;
      this.connectionsButton.Visible = false;
      this.statusSeparator1.Visible = false;
    }
    else
    {
      if (this.toolStripContainer2.Contains((Control) this.navigationToolStrip))
        this.toolStripContainer2.Controls.Remove((Control) this.navigationToolStrip);
      if (this.toolStripContainer2.Contains((Control) this.toolStrip))
        this.toolStripContainer2.Controls.Remove((Control) this.toolStrip);
      if (this.toolStripContainer2.Contains((Control) this.findBar))
        this.toolStripContainer2.Controls.Remove((Control) this.findBar);
      bool flag = false;
      if (!this.toolStripContainer.Contains((Control) this.navigationToolStrip))
      {
        this.toolStripContainer.Controls.Add((Control) this.navigationToolStrip);
        this.toolStrip.Dock = DockStyle.Right;
        flag = true;
      }
      if (!this.toolStripContainer.Contains((Control) this.toolStrip))
      {
        this.toolStripContainer.Controls.Add((Control) this.toolStrip);
        this.toolStrip.Dock = DockStyle.Right;
        flag = true;
      }
      if (!this.toolStripContainer.Contains((Control) this.findBar))
      {
        this.toolStripContainer.Controls.Add((Control) this.findBar);
        this.toolStrip.Dock = DockStyle.Right;
        flag = true;
      }
      if (flag)
      {
        this.fullScreenStrip.BringToFront();
        ((Control) this.findBar).BringToFront();
        this.navigationToolStrip.BringToFront();
        this.menuStrip1.BringToFront();
        this.toolStrip.BringToFront();
      }
      this.toolStripContainer2.Visible = false;
      this.connectionsButton.Visible = true;
      this.statusSeparator1.Visible = true;
    }
  }

  private void toolStripButtonRetryAutoConnect_Click(object sender, EventArgs e)
  {
    SapiManager.GlobalInstance.RefreshAutoConnect();
  }

  private void toolBarButtonOpen_Click(object sender, EventArgs e) => this.LogOpen();

  private void toolBarButtonPlayPause_Click(object sender, EventArgs e) => this.PlayOrPauseLog();

  private void toolBarButtonStop_Click(object sender, EventArgs e) => this.LogStop();

  private void toolBarButtonSeekStart_Click(object sender, EventArgs e) => this.LogSeekStart();

  private void toolBarButtonRewind_Click(object sender, EventArgs e) => this.LogRewind();

  private void toolBarButtonFastForward_Click(object sender, EventArgs e) => this.LogFastForward();

  private void toolBarButtonSeekEnd_Click(object sender, EventArgs e) => this.LogSeekEnd();

  private string MakeLabel()
  {
    ++this.labelNumber;
    return string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatUserEvent, (object) this.labelNumber);
  }

  private void AddUserEvent(bool showDialog)
  {
    string str = this.MakeLabel();
    if (showDialog)
    {
      using (LabelDialog labelDialog = new LabelDialog(str))
      {
        if (((Form) labelDialog).ShowDialog() != DialogResult.OK)
          return;
        str = labelDialog.Label;
      }
    }
    else
    {
      using (SoundPlayer soundPlayer = new SoundPlayer((Stream) Resources.button_click))
        soundPlayer.Play();
    }
    this.sapiManager.Sapi.LogFiles.LabelLog(str);
    StatusLog.Add(new StatusMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The user entered a User Event named \"{0}\" into the log file.", (object) str), (StatusMessageType) 2, (object) this));
  }

  private void toolBarButtonUserEventClick(object sender, EventArgs e)
  {
    this.AddUserEvent((Control.ModifierKeys & Keys.Shift) != 0);
  }

  private void UserEventLabelClick(object sender, EventArgs e) => this.AddUserEvent(false);

  private void MarkLogForUploadClick(object sender, EventArgs e)
  {
    ServerDataManager.GlobalInstance.AddUploadRequest(SapiManager.GlobalInstance.CurrentEngineSerialNumber, SapiManager.GlobalInstance.CurrentVehicleIdentification, SapiManager.GlobalInstance.CurrentLogFileInformation.LogFilePath);
    int num = (int) ControlHelpers.ShowMessageBox((Control) this, Resources.Message_MarkedLogForUpload, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
  }

  private void ShowLogFileInfo(object sender, EventArgs e)
  {
    using (LogFileInfoDialog logFileInfoDialog = new LogFileInfoDialog(SapiManager.GlobalInstance.LogFileChannels.LogFile))
    {
      int num = (int) logFileInfoDialog.ShowDialog();
    }
  }

  private void EditableUserEventLabelClick(object sender, EventArgs e) => this.AddUserEvent(true);

  private void toolStripButtonPrint_Click(object sender, EventArgs e) => this.Print();

  private void toolBarButtonCut_Click(object sender, EventArgs e) => this.Cut();

  private void toolBarButtonCopy_Click(object sender, EventArgs e) => this.Copy();

  private void toolBarButtonPaste_Click(object sender, EventArgs e) => this.Paste();

  private void toolBarButtonUndo_Click(object sender, EventArgs e) => this.Undo();

  private void OnConnectionsButtonClick(object sender, EventArgs e)
  {
    this.connectionsButton.Checked = !this.connectionsButton.Checked;
    this.UpdateConnectionPane();
  }

  private void UpdateConnectionPane()
  {
    if (!this.menuFullscreen.Checked)
      return;
    if (this.connectionsButton.Checked)
    {
      this.BeginInvoke((Delegate) (() => this.mainTabView.StatusView.Location = new Point(0, this.statusStrip.Top - this.mainTabView.StatusView.Height)));
      this.mainTabView.StatusView.Visible = true;
      this.mainTabView.StatusView.BringToFront();
    }
    else
      this.mainTabView.StatusView.Visible = false;
  }

  private void OnSeekTrackBarValueChanged(object sender, ValueChangedEventArgs e)
  {
    if (this.logFile == null || e.Action != null)
      return;
    this.logFile.Playing = false;
    DateTime dateTime = this.logFile.StartTime + TimeSpan.FromSeconds(this.toolBarTrackBarSeek.Value);
    if (!(this.logFile.CurrentTime != dateTime))
      return;
    this.logFile.CurrentTime = dateTime;
  }

  public bool CanRefreshView => this.mainTabView.CanRefreshView;

  public void RefreshView() => this.mainTabView.RefreshView();

  private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
  {
    if (!this.CanRefreshView)
      return;
    this.RefreshView();
  }

  private void toolBarButtonRefresh_Click(object sender, EventArgs e)
  {
    if (!this.CanRefreshView)
      return;
    this.RefreshView();
  }

  private void OnMenuPartCatalogClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.PartsCatalog);
  }

  private void OnMenuItemDDECVIAIClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.DDECVIAIManual);
  }

  private void OnMenuDDEC10AIClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.DDEC10AIManual);
  }

  private void OnMenuItemFcccEngineSupportClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.FcccEngineSupport);
  }

  private void OnMenuItemFcccOasisClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.FcccOasis);
  }

  private void OnMenuItemFcccRVChassisClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.FcccRVChassis);
  }

  private void OnMenuItemFcccS2B2ChassisClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.FcccS2B2Chassis);
  }

  private void OnMenuItemFcccEconicClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.FcccEconic);
  }

  private void OnMenuDDEC13AIClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.DDEC13AIManual);
  }

  private void OnMenuEuro4AIClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.Euro4AIManual);
  }

  private void menuItemDetroitTransmissionsAIClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.DetroitTransmissionsAIManual);
  }

  private void OnMenuDetroitTransmissionsRGClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.DetroitTransmissionsRGManual);
  }

  private void OnMenuItemDDEC10ReferenceGuideClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.DDEC10ReferenceGuide);
  }

  private void OnMenuItemDDEC13ReferenceGuideClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.DDEC13And16ReferenceGuide);
  }

  private void OnMenuDataMiningReportsClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.DataMiningReports);
  }

  private void OnHistoryBackClick(object sender, EventArgs e)
  {
    ViewHistory.GlobalInstance.GoBack((IContainerApplication) this);
  }

  private void OnHistoryForwardClick(object sender, EventArgs e)
  {
    ViewHistory.GlobalInstance.GoForward((IContainerApplication) this);
  }

  private void OnHistoryChanged(object sender, EventArgs e)
  {
    this.toolStripButtonBack.Enabled = this.backToolStripMenuItem.Enabled = ViewHistory.GlobalInstance.BackStack.Any<ViewHistoryItem>();
    this.toolStripButtonBack.ToolTipText = ViewHistory.GlobalInstance.BackStack.Any<ViewHistoryItem>() ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_BackTo0, (object) ViewHistory.GlobalInstance.BackStack.First<ViewHistoryItem>().DisplayName) : Resources.HistoryBack;
    this.toolStripButtonForward.Enabled = this.forwardToolStripMenuItem.Enabled = ViewHistory.GlobalInstance.ForwardStack.Any<ViewHistoryItem>();
    this.toolStripButtonForward.ToolTipText = ViewHistory.GlobalInstance.ForwardStack.Any<ViewHistoryItem>() ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_FowardTo0, (object) ViewHistory.GlobalInstance.ForwardStack.First<ViewHistoryItem>().DisplayName) : Resources.HistoryForward;
  }

  private static void AddHistoryItems(
    IEnumerable<ViewHistoryItem> items,
    ToolStripItemCollection menu)
  {
    foreach (ViewHistoryItem viewHistoryItem in items)
    {
      ToolStripItem toolStripItem = (ToolStripItem) new ToolStripMenuItem(viewHistoryItem.DisplayName);
      toolStripItem.Tag = (object) viewHistoryItem;
      menu.Add(toolStripItem);
    }
  }

  private void toolStripButtonBack_DropDownOpening(object sender, EventArgs e)
  {
    this.toolStripButtonBack.DropDownItems.Clear();
    MainForm.AddHistoryItems(ViewHistory.GlobalInstance.BackStack, this.toolStripButtonBack.DropDownItems);
  }

  private void toolStripButtonForward_DropDownOpening(object sender, EventArgs e)
  {
    this.toolStripButtonForward.DropDownItems.Clear();
    MainForm.AddHistoryItems(ViewHistory.GlobalInstance.ForwardStack, this.toolStripButtonForward.DropDownItems);
  }

  private void gotoToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
  {
    int index = 0;
    while (index < this.gotoToolStripMenuItem.DropDownItems.Count)
    {
      ToolStripItem dropDownItem = this.gotoToolStripMenuItem.DropDownItems[index];
      if (dropDownItem.Tag is ViewHistoryItem)
      {
        this.gotoToolStripMenuItem.DropDownItems.RemoveAt(index);
        dropDownItem.Dispose();
      }
      else
        ++index;
    }
    MainForm.AddHistoryItems(ViewHistory.GlobalInstance.ForwardStack.Reverse<ViewHistoryItem>(), this.gotoToolStripMenuItem.DropDownItems);
    ToolStripItem toolStripItem = (ToolStripItem) new ToolStripMenuItem(ViewHistory.GlobalInstance.Current.DisplayName);
    toolStripItem.Tag = (object) ViewHistory.GlobalInstance.Current;
    toolStripItem.Font = new Font(toolStripItem.Font, FontStyle.Bold);
    this.gotoToolStripMenuItem.DropDownItems.Add(toolStripItem);
    MainForm.AddHistoryItems(ViewHistory.GlobalInstance.BackStack, this.gotoToolStripMenuItem.DropDownItems);
  }

  private void gotoToolStripMenuItem_DropDownItemClicked(
    object sender,
    ToolStripItemClickedEventArgs e)
  {
    if (!(e.ClickedItem.Tag is ViewHistoryItem tag))
      return;
    ViewHistory.GlobalInstance.Navigate((IContainerApplication) this, tag);
  }

  private void toolStripButtonForward_DropDownItemClicked(
    object sender,
    ToolStripItemClickedEventArgs e)
  {
    ViewHistory.GlobalInstance.Navigate((IContainerApplication) this, e.ClickedItem.Tag as ViewHistoryItem);
  }

  private void toolStripButtonBack_DropDownItemClicked(
    object sender,
    ToolStripItemClickedEventArgs e)
  {
    ViewHistory.GlobalInstance.Navigate((IContainerApplication) this, e.ClickedItem.Tag as ViewHistoryItem);
  }

  private void menuFeedBack_Click(object sender, EventArgs e)
  {
    using (FeedbackDialog feedbackDialog = new FeedbackDialog())
    {
      int num = (int) feedbackDialog.ShowDialog();
    }
  }

  private void switchDataMiningProcessTagClick(object sender, EventArgs e)
  {
    SapiManager.SwitchDataMiningProcessTag();
  }

  private void OnMenuVehicleInformationClick(object sender, EventArgs e)
  {
    Link.ShowTarget(LinkSupport.VehicleInformationHome);
  }

  private void toolStripButtonFilter_Click(object sender, EventArgs e)
  {
    FilterDialog filterDialog = new FilterDialog();
    filterDialog.FilterList = this.activeFilterList;
    if (filterDialog.ShowDialog() != DialogResult.OK)
      return;
    this.UpdateFilterList(filterDialog.FilterList);
  }

  private void LoadSettingsFilterList()
  {
    List<FilterTypes> list = this.mainTabView.Filters.ToList<FilterTypes>();
    this.activeFilterList.Clear();
    if (list.Count == 0)
    {
      if (PanelBase.GetFiltered((FilterTypes) 1))
        this.activeFilterList.Add((FilterTypes) 1, true);
      else
        this.activeFilterList.Add((FilterTypes) 1, false);
    }
    else
    {
      foreach (FilterTypes key in list)
      {
        if (PanelBase.GetFiltered(key))
          this.activeFilterList.Add(key, true);
        else
          this.activeFilterList.Add(key, false);
      }
    }
    this.UpdateFilterButtonHighlight();
  }

  private void UpdateFilterList(Dictionary<FilterTypes, bool> newFilterValues)
  {
    foreach (KeyValuePair<FilterTypes, bool> newFilterValue in newFilterValues)
    {
      bool flag;
      if (!this.activeFilterList.TryGetValue(newFilterValue.Key, out flag))
      {
        this.activeFilterList.Add(newFilterValue.Key, newFilterValue.Value);
        PanelBase.SetFiltered(newFilterValue.Key, newFilterValue.Value);
      }
      else if (flag != newFilterValue.Value)
      {
        this.activeFilterList[newFilterValue.Key] = newFilterValue.Value;
        PanelBase.SetFiltered(newFilterValue.Key, newFilterValue.Value);
      }
    }
    this.UpdateFilterButtonHighlight();
  }

  private void UpdateFilterButtonHighlight()
  {
    if (this.activeFilterList.Values.Contains<bool>(false))
      this.toolStripButtonFilter.Image = (Image) Resources.funnel_enabled;
    else
      this.toolStripButtonFilter.Image = (Image) Resources.funnel;
    this.UpdateFilterButtonText();
  }

  private void mainTabView_FilteredContentChanged(object sender, EventArgs e)
  {
    this.UpdateFilterButtonText();
  }

  private void UpdateFilterButtonText()
  {
    string str1 = string.Empty;
    IFilterable filterableChild = this.mainTabView.FilterableChild;
    List<FilterTypes> list = this.activeFilterList.Where<KeyValuePair<FilterTypes, bool>>((Func<KeyValuePair<FilterTypes, bool>, bool>) (x => x.Value)).Select<KeyValuePair<FilterTypes, bool>, FilterTypes>((Func<KeyValuePair<FilterTypes, bool>, FilterTypes>) (x => x.Key)).ToList<FilterTypes>();
    string str2;
    if (list.Count == this.activeFilterList.Count)
    {
      str2 = Resources.Tooltip_FilterShowingAllContent;
    }
    else
    {
      string str3 = list.Count == 0 ? Resources.Tooltip_FilterShowingOnlyEssentialContent : string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Tooltip_Format_ShowingSelectedContent, (object) string.Join(", ", list.Select<FilterTypes, string>((Func<FilterTypes, string>) (f => FilterTypeExtensions.ToDisplayString(f)))));
      if (filterableChild != null)
      {
        str1 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Format_FilterButtonCount, (object) (filterableChild.TotalNumberOfFilterableItems - filterableChild.NumberOfItemsFiltered), (object) filterableChild.TotalNumberOfFilterableItems);
        str2 = str3 + Environment.NewLine + string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.Tooltip_Format_FilterCountDescription, (object) (filterableChild.TotalNumberOfFilterableItems - filterableChild.NumberOfItemsFiltered), (object) filterableChild.TotalNumberOfFilterableItems);
      }
      else
        str2 = str3 + Environment.NewLine + Resources.Tooltip_FilterCurrentViewDoesNotSupportFiltering;
    }
    if (this.toolStripButtonFilter == null || this.toolStripButtonFilter.IsDisposed)
      return;
    this.toolStripButtonFilter.DisplayStyle = !string.IsNullOrEmpty(str1) ? ToolStripItemDisplayStyle.ImageAndText : ToolStripItemDisplayStyle.Image;
    this.toolStripButtonFilter.Text = str1;
    this.toolStripButtonFilter.ToolTipText = str2;
  }

  public bool CanExpandAllItems => this.mainTabView.CanExpandAllItems;

  public bool CanCollapseAllItems => this.mainTabView.CanCollapseAllItems;

  public void ExpandAllItems() => this.mainTabView.ExpandAllItems();

  public void CollapseAllItems() => this.mainTabView.CollapseAllItems();

  private void toolStripButtonExpandAll_Click(object sender, EventArgs e)
  {
    if (!this.CanExpandAllItems)
      return;
    this.ExpandAllItems();
  }

  private void toolStripButtonCollapseAll_Click(object sender, EventArgs e)
  {
    if (!this.CanCollapseAllItems)
      return;
    this.CollapseAllItems();
  }

  private void UpdateToolStripButtonUnitsSystemTooltip()
  {
    if (this.toolStripButtonUnitsSystem == null || this.toolStripButtonUnitsSystem.IsDisposed)
      return;
    this.toolStripButtonUnitsSystem.ToolTipText = this.sapiManager.UserUnits == 1 ? Resources.Tooltip_UnitsSystem_English : Resources.Tootip_UnitsSystem_Metric;
  }

  private void toolStripButtonUnitsSystem_Click(object sender, EventArgs e)
  {
    this.sapiManager.SetUserUnits(this.sapiManager.UserUnits == 1 ? (UnitsSystemSelection) 2 : (UnitsSystemSelection) 1);
    this.UpdateToolStripButtonUnitsSystemTooltip();
    this.Invalidate(true);
  }

  private void toolStripButtonBusMonitor_Click(object sender, EventArgs e) => BusMonitorForm.Show();

  private void busMonitoringToolStripMenuItem_Click(object sender, EventArgs e)
  {
    BusMonitorForm.Show();
  }

  private void toolStripButtonMute_Click(object sender, EventArgs e)
  {
    this.toolStripMenuItemMute.Checked = this.toolStripButtonMute.Checked = !this.toolStripButtonMute.Checked;
    MainForm.ToggleMute(!this.toolStripButtonMute.Checked);
  }

  private void toolStripMenuItemMute_Click(object sender, EventArgs e)
  {
    this.toolStripButtonMute.Checked = this.toolStripMenuItemMute.Checked = !this.toolStripMenuItemMute.Checked;
    MainForm.ToggleMute(!this.toolStripMenuItemMute.Checked);
  }

  private static void ToggleMute(bool autoRead)
  {
    foreach (Channel channel in (ChannelBaseCollection) SapiManager.GlobalInstance.Sapi.Channels)
      channel.Instruments.AutoRead = channel.FaultCodes.AutoRead = channel.EcuInfos.AutoRead = autoRead;
  }

  private void OnMenuPrintFaultsClick(object sender, EventArgs e)
  {
    if (PrintHelper.Busy)
      return;
    using (PrintFaultsDialog printFaultsDialog = new PrintFaultsDialog())
    {
      if (printFaultsDialog.ShowDialog() != DialogResult.OK)
        return;
      PrintHelper.ShowPrintDialog("Faults", (IProvideHtml) printFaultsDialog, (IncludeInfo) 3);
    }
  }

  private delegate void VoidArgsDelegate();

  private enum HelpAction
  {
    BestAvailable,
    Main,
    Context,
  }
}
