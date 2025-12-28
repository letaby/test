// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.MenuProxy
// Assembly: Reprogramming, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 6E09671B-250E-411A-80FC-C490A3A17075
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Reprogramming.dll

using DetroitDiesel.Common;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public sealed class MenuProxy : IMenuProxy, IDisposable
{
  private static MenuProxy globalInstance = new MenuProxy();
  private ToolStrip toolStrip;
  private ToolStripMenuItem menuTools;
  private ToolStripMenuItem menuViewStationLog;
  private IContainerApplication containerApplication;
  private IPlace place;

  public static MenuProxy GlobalInstance => MenuProxy.globalInstance;

  public ReprogrammingView ReprogrammingView { get; set; }

  private MenuProxy()
  {
    this.menuTools = new ToolStripMenuItem(Resources.CommandTools);
    this.menuViewStationLog = new ToolStripMenuItem(Resources.CommandViewStationLog);
    this.menuViewStationLog.Click += new EventHandler(this.menuViewStationLog_Click);
    this.menuTools.DropDownItems.Add((ToolStripItem) new ToolStripSeparator());
    this.menuTools.DropDownItems.Add((ToolStripItem) this.menuViewStationLog);
    this.toolStrip = new ToolStrip();
    this.toolStrip.Items.Add((ToolStripItem) this.menuTools);
    this.menuTools.MergeAction = MergeAction.MatchOnly;
    this.menuViewStationLog.OwnerChanged += new EventHandler(this.menuViewStationLog_OwnerChanged);
    ServerDataManager.GlobalInstance.DataUpdated += new EventHandler(this.GlobalInstance_DataUpdated);
    SapiManager.GlobalInstance.ChannelIdentificationChanged += new EventHandler<ChannelIdentificationChangedEventArgs>(this.SapiManager_ChannelIdentificationChanged);
  }

  private void menuViewStationLog_OwnerChanged(object sender, EventArgs e)
  {
    if (!(this.menuViewStationLog.OwnerItem is ToolStripMenuItem ownerItem))
      return;
    ownerItem.DropDownOpening += new EventHandler(this.menuTools_DropDownOpening);
  }

  private void menuViewStationLog_Click(object sender, EventArgs e)
  {
    int num = (int) CustomMessageBox.Show((IWin32Window) null, StationLogManager.Read(), Resources.CaptionStationLog, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, (CustomMessageBoxOptions) 2);
  }

  private void menuTools_DropDownOpening(object sender, EventArgs e)
  {
    this.menuViewStationLog.Enabled = File.Exists(Directories.StationLogFile);
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
    this.menuViewStationLog.Dispose();
    this.menuTools.Dispose();
    this.toolStrip.Dispose();
  }

  public IPlace Place
  {
    get => this.place;
    set => this.place = value;
  }

  public IContainerApplication ContainerApplication
  {
    get => this.containerApplication;
    set => this.containerApplication = value;
  }

  ToolStrip IMenuProxy.Menu => this.toolStrip;

  private void GlobalInstance_DataUpdated(object sender, EventArgs e)
  {
    this.CheckAutomaticOperations();
  }

  private void SapiManager_ChannelIdentificationChanged(
    object sender,
    ChannelIdentificationChangedEventArgs e)
  {
    this.CheckAutomaticOperations();
  }

  internal void CheckAutomaticOperations()
  {
    if (WarningsPanel.GlobalInstance == null)
      return;
    UnitInformation connectedUnitInformation = ProgrammingData.ConnectedUnitInformation;
    if (connectedUnitInformation != null && connectedUnitInformation.InAutomaticOperation && !ServerDataManager.GlobalInstance.Programming)
    {
      string empty = string.Empty;
      AutomaticOperation automaticOperation = connectedUnitInformation.CurrentAutomaticOperation;
      string str;
      if (automaticOperation.Channel != null)
      {
        if (connectedUnitInformation.AutomaticOperations.Count > 1)
          str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.WarningPanelMessageFormat_TheAutomaticReprogrammingOperation0IsRequiredForTheConnectedVehicleClickHereToStartTheOperationForTheDevice1Step2Of3, (object) automaticOperation.Reason, (object) automaticOperation.Device, (object) (automaticOperation.Index + 1), (object) connectedUnitInformation.AutomaticOperations.Count);
        else
          str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.WarningPanelMessageFormat_TheAutomaticReprogrammingOperation0IsRequiredForTheConnectedVehicleClickHereToStartTheOperationForTheDevice1, (object) automaticOperation.Reason, (object) automaticOperation.Device);
      }
      else if (connectedUnitInformation.AutomaticOperations.Count > 1)
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.WarningPanelMessageFormat_TheAutomaticReprogrammingOperation0IsRequiredForTheConnectedVehicleConnectTheDevice1ToProceedStep2Of3, (object) automaticOperation.Reason, (object) automaticOperation.Device, (object) (automaticOperation.Index + 1), (object) connectedUnitInformation.AutomaticOperations.Count);
      else
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.WarningPanelMessageFormat_TheAutomaticReprogrammingOperation0IsRequiredForTheConnectedVehicleConnectTheDevice1ToProceed, (object) automaticOperation.Reason, (object) automaticOperation.Device);
      WarningsPanel.GlobalInstance.Add("automaticoperation", MessageBoxIcon.Exclamation, string.Empty, str, new EventHandler(this.AutomaticProgramDeviceClickHandler));
    }
    else
      WarningsPanel.GlobalInstance.Remove("automaticoperation");
  }

  private void AutomaticProgramDeviceClickHandler(object sender, EventArgs e)
  {
    if (this.ReprogrammingView == null)
      this.ContainerApplication.PreloadPlace(this.Place);
    try
    {
      ProgrammingData forConnectedUnit = ProgrammingData.CreateFromAutomaticOperationForConnectedUnit();
      if (forConnectedUnit == null)
        return;
      this.ContainerApplication.SelectPlace(this.Place);
      this.ReprogrammingView.SelectedUnit = forConnectedUnit.Unit;
      this.ReprogrammingView.ProgrammingData = Enumerable.Repeat<ProgrammingData>(forConnectedUnit, 1);
      this.ReprogrammingView.AdvanceToLastPage();
    }
    catch (DataException ex)
    {
      int num = (int) ControlHelpers.ShowMessageBox((Control) this.ReprogrammingView, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
    }
  }
}
