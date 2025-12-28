// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.TabbedView
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Product;
using DetroitDiesel.Reflection;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using DetroitDiesel.Windows.Forms.Themed;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public class TabbedView : 
  ContextHelpControl,
  ISearchable,
  ISupportEdit,
  IProvideHtml,
  IRefreshable,
  IFilterable,
  ISupportExpandCollapseAll
{
  private Collection<IMenuProxy> menuProxies = new Collection<IMenuProxy>();
  private EditSupportHelper editSupport = new EditSupportHelper();
  private IProgressBar splashProgressBar;
  private ToolTip vinToolTip = new ToolTip();
  private ToolTip esnToolTip = new ToolTip();
  private ToolTip equipmentToolTip = new ToolTip();
  private ToolTip vinInfoToolTip = new ToolTip();
  private IContainerApplication containerApplication;
  private string displayedVin;
  private string displayedEsn;
  private float dpiX = 96f;
  private float dpiY = 96f;
  private Label headerLabel;
  private WarningsPanel warningsPanel;
  private Panel bodyPanel;
  private Splitter splitter;
  private Panel contentPanel;
  private Control currentPanel;
  private Panel leftPanel;
  private EcuStatusView connections;
  private PlacesBar placesBar;
  private PictureBox headerWatermark;
  private ContextLinkButton contextLinkButton;
  private TableLayoutPanel headerPanel;
  private PictureBox pictureBoxImage;
  private TableLayoutPanel tableLayoutPanelPicture;
  private ConnectionState j1708State;
  private TableLayoutPanel stateLayoutPanel;
  private ConnectionState j1939State;
  private TableLayoutPanel vinEsnTableLayout;
  private Label vinLabel;
  private Label esnLabel;
  private ContextMenu contextMenu;
  private MenuItem copyVinCommand;
  private MenuItem copyEsnCommand;
  private Label vinInfoLabel;
  private Label equipmentLabel;
  private ConnectionState doIPState;
  private SplitContainer splitContainer;
  private TableLayoutPanel tableLayoutPanelLeft;

  public IProgressBar SplashProgressBar
  {
    get => this.splashProgressBar;
    set => this.splashProgressBar = value;
  }

  public IContainerApplication ContainerApplication
  {
    get => this.containerApplication;
    set => this.containerApplication = value;
  }

  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  [Browsable(false)]
  public int SelectedIndex
  {
    get => this.placesBar.SelectedIndex;
    set => this.placesBar.SelectedIndex = value;
  }

  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  [Browsable(false)]
  public IList<IPlace> Tabs => this.placesBar.Places;

  public void SetBranding(BrandingBase branding)
  {
    this.headerWatermark.Image = branding.HeaderWatermark;
    this.headerPanel.ForeColor = SystemColors.WindowText;
    this.headerPanel.BackColor = SystemColors.Window;
    this.placesBar.View = SettingsManager.GlobalInstance.GetValue<ViewTypes>("PlacesView", "MainWindow", (ViewTypes) 3);
    this.placesBar.ButtonTextImageLayout = SettingsManager.GlobalInstance.GetValue<ButtonLayout>("PlacesLayout", "MainWindow", (ButtonLayout) 4);
    this.UpdateBanner();
  }

  public void OnDispose(bool disposing)
  {
    if (!disposing)
      return;
    SettingsManager.GlobalInstance.SetValue<ViewTypes>("PlacesView", "MainWindow", this.placesBar.View, false);
    SettingsManager.GlobalInstance.SetValue<ButtonLayout>("PlacesLayout", "MainWindow", this.placesBar.ButtonTextImageLayout, false);
    foreach (IFilterable ifilterable in this.bodyPanel.Controls.OfType<IFilterable>())
      ifilterable.FilteredContentChanged -= new EventHandler(this.filterableItem_FilteredContentChanged);
    foreach (IMenuProxy menuProxy in this.menuProxies)
    {
      if (menuProxy is IDisposable disposable)
        disposable.Dispose();
    }
    ThemeProvider.GlobalInstance.ThemeChanged -= new EventHandler(this.OnThemeChanged);
  }

  public bool ShowSidebar
  {
    get => this.leftPanel.Visible;
    set
    {
      this.leftPanel.Visible = value;
      if (value)
      {
        this.connections.Dock = DockStyle.Fill;
        this.splitContainer.Panel2.Controls.Add((Control) this.connections);
        this.connections.Visible = true;
      }
      else
      {
        this.connections.Visible = false;
        this.connections.Dock = DockStyle.None;
        ((Control) this).Controls.Add((Control) this.connections);
      }
    }
  }

  internal EcuStatusView StatusView => this.connections;

  internal ContextLinkButton ContextLinkButton => this.contextLinkButton;

  public bool SidebarEnabled
  {
    get => this.leftPanel.Enabled;
    set => this.leftPanel.Enabled = value;
  }

  public TabbedView()
  {
    this.InitializeComponent();
    this.contextLinkButton.Context = (IContextHelp) new ContextHelpChain((object) this, LinkSupport.MainOnlineHelp);
    ThemeProvider.GlobalInstance.ThemeChanged += new EventHandler(this.OnThemeChanged);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    this.UpdateBannerColors();
    this.BuildPlaces();
    this.UpdateScaling();
    if (!((Component) this).DesignMode)
    {
      this.UpdateVinAndEsnLabel();
      SapiManager.GlobalInstance.EquipmentTypeChanged += new EventHandler<EquipmentTypeChangedEventArgs>(this.GlobalInstance_EquipmentTypeChanged);
      SapiManager.GlobalInstance.ChannelIdentificationChanged += new EventHandler<ChannelIdentificationChangedEventArgs>(this.GlobalInstance_ChannelIdentificationChanged);
      SapiManager.GlobalInstance.ConnectedUnitChanged += new EventHandler(this.GlobalInstance_ConnectedUnitChanged);
      SapiManager.GlobalInstance.DisplayPinWarningDataItemChanged += new EventHandler<DisplayPinWarningDataItemChangedEventArgs>(this.GlobalInstance_DisplayPinWarningDataItemChanged);
      this.doIPState.Visible = ApplicationInformation.CanRollCallDoIP;
    }
    ((ContainerControl) this).ParentForm.Shown += new EventHandler(this.ParentForm_Shown);
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.ParentForm_FormClosing);
    this.j1939State.VisibleChanged += new EventHandler(this.J1939State_VisibleChanged);
    this.splitContainer.FixedPanel = FixedPanel.Panel1;
    this.splitContainer.SplitterDistance = SettingsManager.GlobalInstance.GetValue<int>("PlacesSplitterDistance", "MainWindow", this.splitContainer.SplitterDistance);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void ParentForm_Shown(object sender, EventArgs e)
  {
    ((Control) this).Refresh();
    this.splitContainer.SplitterDistance = SettingsManager.GlobalInstance.GetValue<int>("PlacesSplitterDistance", "MainWindow", this.splitContainer.SplitterDistance);
    this.splitContainer.FixedPanel = FixedPanel.None;
  }

  private void J1939State_VisibleChanged(object sender, EventArgs e)
  {
    this.splitContainer.SplitterDistance = SettingsManager.GlobalInstance.GetValue<int>("PlacesSplitterDistance", "MainWindow", this.splitContainer.SplitterDistance);
  }

  private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
  {
    SettingsManager.GlobalInstance.SetValue<int>("PlacesSplitterDistance", "MainWindow", this.splitContainer.SplitterDistance, false);
  }

  private void GlobalInstance_ConnectedUnitChanged(object sender, EventArgs e)
  {
    this.UpdateVinAndEsnLabel();
  }

  private void GlobalInstance_ChannelIdentificationChanged(
    object sender,
    ChannelIdentificationChangedEventArgs e)
  {
    this.UpdateVinAndEsnLabel();
  }

  private void GlobalInstance_DisplayPinWarningDataItemChanged(
    object sender,
    DisplayPinWarningDataItemChangedEventArgs e)
  {
    if (e.DisplayWarning)
      WarningsPanel.GlobalInstance.Add("TIER4PINERROR", MessageBoxIcon.Hand, (string) null, Resources.Tier4PinError_UserText, new EventHandler(this.Tier4PinError_Click));
    else
      WarningsPanel.GlobalInstance.Remove("TIER4PINERROR");
  }

  private void GlobalInstance_EquipmentTypeChanged(object sender, EquipmentTypeChangedEventArgs e)
  {
    this.UpdateEquipmentLabel();
  }

  private void Tier4PinError_Click(object sender, EventArgs e)
  {
    ActionsMenuProxy.GlobalInstance.ShowDialog("Set Engine Serial Number/Product Identification Number", "Off-Highway", (object) null, false);
  }

  private void UpdateEquipmentLabel()
  {
    List<string> stringList = new List<string>();
    stringList.AddRange(SapiManager.GlobalInstance.ConnectedEquipment.Where<EquipmentType>((Func<EquipmentType, bool>) (c =>
    {
      ElectronicsFamily family = ((EquipmentType) ref c).Family;
      return ((ElectronicsFamily) ref family).Category != "Vehicle";
    })).GroupBy<EquipmentType, string>((Func<EquipmentType, string>) (e =>
    {
      ElectronicsFamily family = ((EquipmentType) ref e).Family;
      return ((ElectronicsFamily) ref family).Category;
    })).Select<IGrouping<string, EquipmentType>, string>((Func<IGrouping<string, EquipmentType>, string>) (g => $"{g.Key}: {string.Join(",", g.Select<EquipmentType, string>((Func<EquipmentType, string>) (et => ((EquipmentType) ref et).CompleteName)).ToArray<string>())}")));
    this.equipmentLabel.Text = string.Join(Environment.NewLine, stringList.ToArray());
  }

  private static string BuildInfoContent(
    string separator,
    VinInformation vinInformation,
    string[] qualifiers)
  {
    List<string> values = new List<string>();
    foreach (string qualifier in qualifiers)
    {
      string informationValue = vinInformation.GetInformationValue(qualifier);
      if (!string.IsNullOrEmpty(informationValue))
        values.Add(informationValue);
    }
    return string.Join(separator, (IEnumerable<string>) values);
  }

  private void equipmentLabel_MouseHover(object sender, EventArgs e)
  {
    IEnumerable<EquipmentType> connectedEquipment = SapiManager.GlobalInstance.ConnectedEquipment;
    if (connectedEquipment.Any<EquipmentType>())
    {
      string text = string.Join(Environment.NewLine, connectedEquipment.GroupBy<EquipmentType, string>((Func<EquipmentType, string>) (eq =>
      {
        ElectronicsFamily family = ((EquipmentType) ref eq).Family;
        return ((ElectronicsFamily) ref family).Category;
      })).Select<IGrouping<string, EquipmentType>, string>((Func<IGrouping<string, EquipmentType>, string>) (g => $"{g.Key}: {string.Join(",", g.Select<EquipmentType, string>((Func<EquipmentType, string>) (et => ((EquipmentType) ref et).CompleteName)).ToArray<string>())}")));
      this.equipmentToolTip.Active = true;
      this.equipmentToolTip.Show(text, (IWin32Window) this.equipmentLabel, (int) short.MaxValue);
    }
    else
      this.equipmentToolTip.Active = false;
  }

  private void vinInfoLabel_MouseHover(object sender, EventArgs e)
  {
    VinInformation vinInformation = (VinInformation) null;
    if (!string.IsNullOrEmpty(this.displayedVin))
      vinInformation = VinInformation.GetVinInformation(this.displayedVin);
    if (vinInformation != null && vinInformation.InformationEntries.Any<KeyValuePair<string, string>>())
    {
      string text = string.Join(Environment.NewLine, vinInformation.InformationEntries.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (ie => $"{VinDecodingInformation.GetTranslatedGroupName(ie.Key)}: {ie.Value}")));
      this.vinInfoToolTip.Active = true;
      this.vinInfoToolTip.Show(text, (IWin32Window) this.vinInfoLabel, (int) short.MaxValue);
    }
    else
      this.vinInfoToolTip.Active = false;
  }

  private void UpdateVinAndEsnLabel()
  {
    string vehicleIdentification = SapiManager.GlobalInstance.CurrentVehicleIdentification;
    string engineSerialNumber = SapiManager.GlobalInstance.CurrentEngineSerialNumber;
    if (vehicleIdentification != this.displayedVin)
    {
      VinInformation vinInformation = (VinInformation) null;
      this.displayedVin = vehicleIdentification;
      if (!string.IsNullOrEmpty(vehicleIdentification))
      {
        this.vinLabel.Visible = true;
        this.vinLabel.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TabbedView_FormatVIN, (object) vehicleIdentification);
        vinInformation = VinInformation.GetVinInformation(this.displayedVin);
      }
      else
        this.vinLabel.Visible = false;
      if (vinInformation != null && vinInformation.InformationEntries.Any<KeyValuePair<string, string>>())
      {
        this.vinInfoLabel.Text = TabbedView.BuildInfoContent(" ", vinInformation, new string[4]
        {
          "modelyear",
          "make",
          "model",
          "Chassis"
        });
        this.vinInfoLabel.Visible = true;
      }
      else
        this.vinInfoLabel.Visible = false;
    }
    this.vinLabel.ForeColor = SapiManager.GlobalInstance.VehicleIdentificationInconsistency ? Color.Red : this.equipmentLabel.ForeColor;
    if (engineSerialNumber != this.displayedEsn)
    {
      this.displayedEsn = engineSerialNumber;
      if (!string.IsNullOrEmpty(engineSerialNumber))
      {
        this.esnLabel.Visible = true;
        this.esnLabel.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TabbedView_FormatESN, (object) engineSerialNumber);
      }
      else
        this.esnLabel.Visible = false;
    }
    this.esnLabel.ForeColor = SapiManager.GlobalInstance.EngineSerialInconsistency ? Color.Red : this.equipmentLabel.ForeColor;
    this.UpdateEquipmentLabel();
  }

  private void vinLabel_MouseHover(object sender, EventArgs e)
  {
    this.vinToolTip.Active = false;
    if (!SapiManager.GlobalInstance.VehicleIdentificationInconsistency)
      return;
    this.vinToolTip.Active = true;
    this.vinToolTip.Show(Resources.TabbedView_ToolTip_VIN, (IWin32Window) this.vinLabel, 5000);
  }

  private void esnLabel_MouseHover(object sender, EventArgs e)
  {
    this.esnToolTip.Active = false;
    if (!SapiManager.GlobalInstance.EngineSerialInconsistency)
      return;
    this.esnToolTip.Active = true;
    this.esnToolTip.Show(Resources.TabbedView_ToolTip_ESN, (IWin32Window) this.esnLabel, 5000);
  }

  private void contextMenu_Popup(object sender, EventArgs e)
  {
    this.copyVinCommand.Enabled = !string.IsNullOrEmpty(this.vinLabel.Text);
    this.copyEsnCommand.Enabled = !string.IsNullOrEmpty(this.esnLabel.Text);
  }

  private void copyVinCommand_Click(object sender, EventArgs e)
  {
    Clipboard.SetText(this.vinLabel.Text.Substring(5));
  }

  private void copyEsnCommand_Click(object sender, EventArgs e)
  {
    Clipboard.SetText(this.esnLabel.Text.Substring(5));
  }

  private void OnBannerSizeChanged(object sender, EventArgs e) => this.UpdateBanner();

  private void BuildPlaces()
  {
    foreach (string str in ((IEnumerable<string>) new string[6]
    {
      "EcuInfo.dll",
      "EcuStatus.dll",
      "Troubleshooting.dll",
      "Instruments.dll",
      "Parameters.dll",
      "Reprogramming.dll"
    }).Select<string, string>((Func<string, string>) (f => Path.Combine(Application.StartupPath, f))).Where<string>((Func<string, bool>) (p => File.Exists(p))))
    {
      Assembly assembly = FileManagement.LoadAssembly(str);
      try
      {
        this.AddPanels(assembly);
      }
      catch (FileLoadException ex)
      {
        AssemblyName name1 = Assembly.GetExecutingAssembly().GetName();
        AssemblyName name2 = assembly.GetName();
        if (name2.Version != name1.Version)
        {
          int num = (int) ControlHelpers.ShowMessageBox(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Program_FormatInconsistentInstallation, (object) ApplicationInformation.ProductName, (object) name1.Name, (object) name1.Version, (object) name2.Name, (object) name2.Version), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
        }
        throw;
      }
    }
    if (this.placesBar.IdealWidth > this.splitter.SplitPosition)
      this.splitter.SplitPosition = this.placesBar.IdealWidth + 1;
    if (this.placesBar.Places.Count > this.placesBar.SelectedIndex && this.placesBar.Places.Count > 0 && !this.ChangePanel(this.placesBar.Places[this.placesBar.SelectedIndex]))
      throw new InvalidOperationException("Failed to display initial panel.");
  }

  private void AddPanels(Assembly assembly)
  {
    if (!(assembly != (Assembly) null))
      return;
    object[] customAttributes = assembly.GetCustomAttributes(typeof (PanelAttribute), false);
    if (customAttributes.Length == 0)
      return;
    bool flag = false;
    byte[] publicKey1 = ((object) this).GetType().Assembly.GetName().GetPublicKey();
    byte[] publicKey2 = assembly.GetName().GetPublicKey();
    if (publicKey1.Length == publicKey2.Length)
    {
      flag = true;
      for (int index = 0; index < publicKey1.Length & flag; ++index)
      {
        if ((int) publicKey1[index] != (int) publicKey2[index])
          flag = false;
      }
    }
    if (!flag)
      return;
    foreach (PanelAttribute panelAttribute in customAttributes)
    {
      if (ApplicationInformation.IsViewVisible(panelAttribute.PositionIdentifier))
      {
        if (this.splashProgressBar != null)
          this.splashProgressBar.OverallStatusText = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.StartupStatus_Format_LoadingView, (object) panelAttribute.Title);
        IPlace place = this.placesBar.Add(panelAttribute);
        if (panelAttribute.MenuProxy != (System.Type) null)
        {
          IMenuProxy imenuProxy = panelAttribute.MenuProxy.GetProperty("GlobalInstance").GetValue((object) null, (object[]) null) as IMenuProxy;
          imenuProxy.ContainerApplication = this.containerApplication;
          imenuProxy.Place = place;
          if (imenuProxy.Menu != null)
            ToolStripManager.Merge(imenuProxy.Menu, this.containerApplication.Menu);
          this.menuProxies.Add(imenuProxy);
        }
        if (panelAttribute.Preload)
          this.EnsurePanelCreated(place);
      }
    }
  }

  private void OnBannerTextChanged(object sender, EventArgs e) => this.UpdateBanner();

  private void UpdateBanner()
  {
    bool flag = false;
    if (this.headerWatermark.Image != null)
    {
      Size size1 = this.headerWatermark.Image.Size with
      {
        Width = this.headerPanel.Width - this.headerWatermark.Image.Width - ((Control) this.contextLinkButton).Width - MarginWidth((Control) this.headerWatermark) - MarginWidth((Control) this.headerLabel) - MarginWidth((Control) this.vinEsnTableLayout) - MarginWidth((Control) this.vinLabel) - MarginWidth((Control) this.equipmentLabel) - MarginWidth((Control) this.contextLinkButton) - MarginWidth((Control) this.tableLayoutPanelPicture)
      };
      if (size1.Width > 0)
      {
        int width1 = TextRenderer.MeasureText(this.headerLabel.Text, this.headerLabel.Font).Width;
        Size size2 = this.tableLayoutPanelPicture.Size;
        int width2 = size2.Width;
        int num1 = width1 + width2;
        size2 = TextRenderer.MeasureText(this.vinInfoLabel.Text, this.vinInfoLabel.Font);
        int width3 = size2.Width;
        size2 = TextRenderer.MeasureText(this.equipmentLabel.Text, this.equipmentLabel.Font);
        int width4 = size2.Width;
        size2 = TextRenderer.MeasureText(this.vinLabel.Text, this.vinLabel.Font);
        int width5 = size2.Width;
        int val2 = width4 + width5;
        int num2 = Math.Max(width3, val2);
        flag = num1 + num2 <= size1.Width;
      }
    }
    this.headerWatermark.Visible = flag;

    static int MarginWidth(Control control)
    {
      Padding margin = control.Margin;
      int left = margin.Left;
      margin = control.Margin;
      int right = margin.Right;
      return left + right;
    }
  }

  private void UpdateBannerColors()
  {
    ((Control) this.contextLinkButton).ForeColor = this.headerPanel.ForeColor;
    this.contextLinkButton.FlatAppearance.MouseOverBackColor = (double) this.headerPanel.BackColor.GetBrightness() > 0.75 ? ControlPaint.Dark(this.headerPanel.BackColor) : ControlPaint.LightLight(this.headerPanel.BackColor);
    this.headerPanel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 12f, FontStyle.Bold);
    this.equipmentLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 8f, FontStyle.Regular);
    this.vinLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 8f, FontStyle.Regular);
    this.esnLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 8f, FontStyle.Regular);
    this.vinInfoLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 8f, FontStyle.Bold);
  }

  [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
  protected virtual void WndProc(ref Message m)
  {
    if (m.Msg == 794)
      this.UpdateBannerColors();
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).WndProc(ref m));
  }

  private void OnPlacesBarSelectionChanging(object sender, PlacesBarCancelEventArgs e)
  {
    ((CancelEventArgs) e).Cancel = !this.ChangePanel(e.Place);
  }

  private void OnContextLinkChanged(object sender, EventArgs e)
  {
    this.SetLink(LinkSupport.GetContextHelpLink((object) this.currentPanel));
  }

  private bool ChangePanel(IPlace place)
  {
    bool flag = false;
    if (place != null)
    {
      ((Control) this).SuspendLayout();
      ((Control) this).Cursor = Cursors.WaitCursor;
      Control control = this.EnsurePanelCreated(place);
      Control currentPanel1 = this.currentPanel;
      LinkSupport.UnsubscribeChanges((object) currentPanel1, new EventHandler(this.OnContextLinkChanged));
      if (!(currentPanel1 is ISupportActivation isupportActivation) || isupportActivation.Deactivate())
      {
        this.headerLabel.Text = control.Name;
        this.pictureBoxImage.Image = place.Image ?? (Image) null;
        currentPanel1?.Hide();
        this.currentPanel = control;
        control.Show();
        ViewHistory.GlobalInstance.Add(place);
        UsageTracking.MarkNavigationLocation(place);
        if (this.currentPanel is ISupportActivation currentPanel2)
          currentPanel2.Activate();
        LinkSupport.SubscribeChanges((object) control, new EventHandler(this.OnContextLinkChanged));
        this.SetLink(LinkSupport.GetContextHelpLink((object) control));
        control.Focus();
        flag = true;
      }
      this.OnFilteredContentChanged();
      ((Control) this).Cursor = Cursors.Default;
      ((Control) this).ResumeLayout(true);
    }
    return flag;
  }

  public Control EnsurePanelCreated(IPlace place)
  {
    if (!(place.Data is Control control))
    {
      System.Type data = place.Data as System.Type;
      if (data != (System.Type) null)
      {
        control = Activator.CreateInstance(data) as Control;
        control.Hide();
        control.Name = place.Title;
        control.Dock = DockStyle.Fill;
        control.TabStop = true;
        place.Data = (object) control;
        if (!this.bodyPanel.Controls.Contains(control))
        {
          this.bodyPanel.Controls.Add(control);
          if (control is IFilterable ifilterable)
            ifilterable.FilteredContentChanged += new EventHandler(this.filterableItem_FilteredContentChanged);
        }
        control.Size = this.bodyPanel.ClientSize;
      }
    }
    return control;
  }

  private void filterableItem_FilteredContentChanged(object sender, EventArgs e)
  {
    this.OnFilteredContentChanged();
  }

  public void SelectPlace(PanelIdentifier panelId, string location)
  {
    this.placesBar.SelectMatch(panelId);
    if (string.IsNullOrEmpty(location) || !(this.currentPanel is ISupportActivation currentPanel))
      return;
    currentPanel.SelectLocation(location);
  }

  public void SelectPlace(int index) => this.placesBar.SelectPlace(index);

  public void SelectPlace(IPlace place) => this.placesBar.SelectMatch(place);

  public void SelectNext() => this.placesBar.SelectNext();

  public void SelectPrevious() => this.placesBar.SelectPrevious();

  private void BodySizeChanged(object sender, EventArgs e) => this.UpdateScaling();

  private void OnThemeChanged(object sender, EventArgs e) => this.UpdateScaling();

  protected virtual void OnCreateControl()
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnCreateControl());
    using (Graphics graphics = ((Control) this).CreateGraphics())
    {
      this.dpiX = graphics.DpiX;
      this.dpiY = graphics.DpiY;
    }
  }

  private void UpdateScaling()
  {
    float num1 = (float) ((double) this.bodyPanel.Width / 640.0 * 96.0) / this.dpiX;
    float num2 = (float) ((double) this.bodyPanel.Height / 480.0 * 96.0) / this.dpiY;
    ThemeProvider.GlobalInstance.ActiveTheme.WindowScale = (double) num1 < (double) num2 ? (double) num1 : (double) num2;
  }

  private IProvideHtml GetCurrentHtmlProvider()
  {
    IProvideHtml currentHtmlProvider = (IProvideHtml) null;
    int selectedIndex = this.placesBar.SelectedIndex;
    if (0 <= selectedIndex && selectedIndex < this.Tabs.Count)
      currentHtmlProvider = this.Tabs[selectedIndex].Data as IProvideHtml;
    return currentHtmlProvider;
  }

  private void OnConnectionItemActivate(object sender, EventArgs e)
  {
    this.placesBar.SelectMatch((PanelIdentifier) 1);
  }

  public bool Search(string searchText, bool caseSensitive, FindMode direction)
  {
    bool flag = false;
    if (this.currentPanel is ISearchable currentPanel)
      flag = currentPanel.Search(searchText, caseSensitive, direction);
    return flag;
  }

  public bool CanSearch
  {
    get
    {
      bool canSearch = false;
      if (this.currentPanel is ISearchable currentPanel)
        canSearch = currentPanel.CanSearch;
      return canSearch;
    }
  }

  public bool CanUndo
  {
    get
    {
      this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
      return this.editSupport.CanUndo;
    }
  }

  public void Undo()
  {
    // ISSUE: explicit non-virtual call
    this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
    this.editSupport.Undo();
  }

  public bool CanCopy
  {
    get
    {
      this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
      return this.editSupport.CanCopy;
    }
  }

  public void Copy()
  {
    // ISSUE: explicit non-virtual call
    this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
    this.editSupport.Copy();
  }

  public bool CanDelete
  {
    get
    {
      this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
      return this.editSupport.CanDelete;
    }
  }

  public bool CanPaste
  {
    get
    {
      this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
      return this.editSupport.CanPaste;
    }
  }

  public void Cut()
  {
    // ISSUE: explicit non-virtual call
    this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
    this.editSupport.Cut();
  }

  public bool CanSelectAll
  {
    get
    {
      this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
      return this.editSupport.CanSelectAll;
    }
  }

  public void Delete()
  {
    // ISSUE: explicit non-virtual call
    this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
    this.editSupport.Delete();
  }

  public bool CanCut
  {
    get
    {
      this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
      return this.editSupport.CanCut;
    }
  }

  public void Paste()
  {
    // ISSUE: explicit non-virtual call
    this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
    this.editSupport.Paste();
  }

  public void SelectAll()
  {
    // ISSUE: explicit non-virtual call
    this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
    this.editSupport.SelectAll();
  }

  public bool CanProvideHtml => PrintHelper.CanProvideHtml(this.GetCurrentHtmlProvider());

  public string ToHtml() => PrintHelper.ToHtml(this.GetCurrentHtmlProvider());

  public string StyleSheet => PrintHelper.StyleSheet(this.GetCurrentHtmlProvider());

  public bool CanRefreshView
  {
    get
    {
      bool canRefreshView = false;
      if (this.currentPanel is IRefreshable currentPanel)
        canRefreshView = currentPanel.CanRefreshView;
      return canRefreshView;
    }
  }

  public void RefreshView()
  {
    if (!(this.currentPanel is IRefreshable currentPanel))
      return;
    currentPanel.RefreshView();
  }

  public IEnumerable<FilterTypes> Filters
  {
    get
    {
      IEnumerable<FilterTypes> first = (IEnumerable<FilterTypes>) new List<FilterTypes>();
      foreach (object control in (ArrangedElementCollection) this.bodyPanel.Controls)
      {
        if (control is IFilterable ifilterable)
          first = (IEnumerable<FilterTypes>) first.Union<FilterTypes>(ifilterable.Filters).ToList<FilterTypes>();
      }
      return first;
    }
  }

  public int NumberOfItemsFiltered
  {
    get
    {
      return this.bodyPanel.Controls.OfType<IFilterable>().Sum<IFilterable>((Func<IFilterable, int>) (filterableView => filterableView.NumberOfItemsFiltered));
    }
  }

  public int TotalNumberOfFilterableItems
  {
    get
    {
      return this.bodyPanel.Controls.OfType<IFilterable>().Sum<IFilterable>((Func<IFilterable, int>) (filterableView => filterableView.TotalNumberOfFilterableItems));
    }
  }

  public IFilterable FilterableChild => this.currentPanel as IFilterable;

  public event EventHandler FilteredContentChanged;

  private void OnFilteredContentChanged()
  {
    if (this.FilteredContentChanged == null)
      return;
    this.FilteredContentChanged((object) this, new EventArgs());
  }

  public bool CanExpandAllItems
  {
    get
    {
      bool canExpandAllItems = false;
      if (this.currentPanel is ISupportExpandCollapseAll currentPanel)
        canExpandAllItems = currentPanel.CanExpandAllItems;
      return canExpandAllItems;
    }
  }

  public bool CanCollapseAllItems
  {
    get
    {
      bool collapseAllItems = false;
      if (this.currentPanel is ISupportExpandCollapseAll currentPanel)
        collapseAllItems = currentPanel.CanCollapseAllItems;
      return collapseAllItems;
    }
  }

  public void ExpandAllItems()
  {
    if (!(this.currentPanel is ISupportExpandCollapseAll currentPanel))
      return;
    currentPanel.ExpandAllItems();
  }

  public void CollapseAllItems()
  {
    if (!(this.currentPanel is ISupportExpandCollapseAll currentPanel))
      return;
    currentPanel.CollapseAllItems();
  }

  protected virtual void Dispose(bool disposing)
  {
    this.OnDispose(disposing);
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (TabbedView));
    this.placesBar = new PlacesBar();
    this.headerLabel = new Label();
    this.contextLinkButton = new ContextLinkButton();
    this.headerWatermark = new PictureBox();
    this.bodyPanel = new Panel();
    this.splitter = new Splitter();
    this.contentPanel = new Panel();
    this.warningsPanel = new WarningsPanel();
    this.headerPanel = new TableLayoutPanel();
    this.tableLayoutPanelPicture = new TableLayoutPanel();
    this.pictureBoxImage = new PictureBox();
    this.vinEsnTableLayout = new TableLayoutPanel();
    this.vinInfoLabel = new Label();
    this.contextMenu = new ContextMenu();
    this.copyVinCommand = new MenuItem();
    this.copyEsnCommand = new MenuItem();
    this.vinLabel = new Label();
    this.esnLabel = new Label();
    this.equipmentLabel = new Label();
    this.leftPanel = new Panel();
    this.tableLayoutPanelLeft = new TableLayoutPanel();
    this.splitContainer = new SplitContainer();
    this.connections = new EcuStatusView();
    this.stateLayoutPanel = new TableLayoutPanel();
    this.doIPState = new ConnectionState();
    this.j1939State = new ConnectionState();
    this.j1708State = new ConnectionState();
    ((ISupportInitialize) this.headerWatermark).BeginInit();
    this.contentPanel.SuspendLayout();
    this.headerPanel.SuspendLayout();
    this.tableLayoutPanelPicture.SuspendLayout();
    ((ISupportInitialize) this.pictureBoxImage).BeginInit();
    this.vinEsnTableLayout.SuspendLayout();
    this.leftPanel.SuspendLayout();
    this.tableLayoutPanelLeft.SuspendLayout();
    this.splitContainer.BeginInit();
    this.splitContainer.Panel1.SuspendLayout();
    this.splitContainer.Panel2.SuspendLayout();
    this.splitContainer.SuspendLayout();
    this.stateLayoutPanel.SuspendLayout();
    ((Control) this).SuspendLayout();
    ((Control) this.placesBar).BackColor = SystemColors.Window;
    componentResourceManager.ApplyResources((object) this.placesBar, "placesBar");
    ((Control) this.placesBar).ForeColor = SystemColors.WindowText;
    ((Control) this.placesBar).Name = "placesBar";
    this.placesBar.SelectionChanging += new EventHandler<PlacesBarCancelEventArgs>(this.OnPlacesBarSelectionChanging);
    componentResourceManager.ApplyResources((object) this.headerLabel, "headerLabel");
    this.headerLabel.Name = "headerLabel";
    this.headerLabel.UseCompatibleTextRendering = true;
    this.headerLabel.UseMnemonic = false;
    this.headerLabel.TextChanged += new EventHandler(this.OnBannerTextChanged);
    componentResourceManager.ApplyResources((object) this.contextLinkButton, "contextLinkButton");
    this.contextLinkButton.FlatAppearance.BorderSize = 0;
    this.contextLinkButton.FlatStyle = FlatStyle.Flat;
    this.contextLinkButton.ImageAlign = ContentAlignment.MiddleCenter;
    ((Control) this.contextLinkButton).Name = "contextLinkButton";
    this.contextLinkButton.ShowImage = true;
    this.contextLinkButton.TextAlign = ContentAlignment.MiddleLeft;
    componentResourceManager.ApplyResources((object) this.headerWatermark, "headerWatermark");
    this.headerWatermark.Name = "headerWatermark";
    this.headerWatermark.TabStop = false;
    componentResourceManager.ApplyResources((object) this.bodyPanel, "bodyPanel");
    this.bodyPanel.Name = "bodyPanel";
    this.bodyPanel.SizeChanged += new EventHandler(this.BodySizeChanged);
    componentResourceManager.ApplyResources((object) this.splitter, "splitter");
    this.splitter.Name = "splitter";
    this.splitter.TabStop = false;
    this.contentPanel.Controls.Add((Control) this.bodyPanel);
    this.contentPanel.Controls.Add((Control) this.warningsPanel);
    this.contentPanel.Controls.Add((Control) this.headerPanel);
    componentResourceManager.ApplyResources((object) this.contentPanel, "contentPanel");
    this.contentPanel.Name = "contentPanel";
    componentResourceManager.ApplyResources((object) this.warningsPanel, "warningsPanel");
    ((Control) this.warningsPanel).BackColor = Color.LightYellow;
    ((Control) this.warningsPanel).Name = "warningsPanel";
    componentResourceManager.ApplyResources((object) this.headerPanel, "headerPanel");
    this.headerPanel.BackColor = SystemColors.ControlDarkDark;
    this.headerPanel.Controls.Add((Control) this.headerWatermark, 5, 0);
    this.headerPanel.Controls.Add((Control) this.contextLinkButton, 3, 0);
    this.headerPanel.Controls.Add((Control) this.headerLabel, 1, 0);
    this.headerPanel.Controls.Add((Control) this.tableLayoutPanelPicture, 0, 0);
    this.headerPanel.Controls.Add((Control) this.vinEsnTableLayout, 4, 0);
    this.headerPanel.Name = "headerPanel";
    this.headerPanel.SizeChanged += new EventHandler(this.OnBannerSizeChanged);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelPicture, "tableLayoutPanelPicture");
    this.tableLayoutPanelPicture.Controls.Add((Control) this.pictureBoxImage, 1, 1);
    this.tableLayoutPanelPicture.Name = "tableLayoutPanelPicture";
    componentResourceManager.ApplyResources((object) this.pictureBoxImage, "pictureBoxImage");
    this.pictureBoxImage.Name = "pictureBoxImage";
    this.pictureBoxImage.TabStop = false;
    componentResourceManager.ApplyResources((object) this.vinEsnTableLayout, "vinEsnTableLayout");
    this.vinEsnTableLayout.Controls.Add((Control) this.vinInfoLabel, 0, 1);
    this.vinEsnTableLayout.Controls.Add((Control) this.vinLabel, 0, 2);
    this.vinEsnTableLayout.Controls.Add((Control) this.esnLabel, 0, 3);
    this.vinEsnTableLayout.Controls.Add((Control) this.equipmentLabel, 1, 2);
    this.vinEsnTableLayout.Name = "vinEsnTableLayout";
    componentResourceManager.ApplyResources((object) this.vinInfoLabel, "vinInfoLabel");
    this.vinEsnTableLayout.SetColumnSpan((Control) this.vinInfoLabel, 2);
    this.vinInfoLabel.ContextMenu = this.contextMenu;
    this.vinInfoLabel.Name = "vinInfoLabel";
    this.vinInfoLabel.UseMnemonic = false;
    this.vinInfoLabel.MouseHover += new EventHandler(this.vinInfoLabel_MouseHover);
    this.contextMenu.MenuItems.AddRange(new MenuItem[2]
    {
      this.copyVinCommand,
      this.copyEsnCommand
    });
    this.contextMenu.Popup += new EventHandler(this.contextMenu_Popup);
    this.copyVinCommand.Index = 0;
    componentResourceManager.ApplyResources((object) this.copyVinCommand, "copyVinCommand");
    this.copyVinCommand.Click += new EventHandler(this.copyVinCommand_Click);
    this.copyEsnCommand.Index = 1;
    componentResourceManager.ApplyResources((object) this.copyEsnCommand, "copyEsnCommand");
    this.copyEsnCommand.Click += new EventHandler(this.copyEsnCommand_Click);
    componentResourceManager.ApplyResources((object) this.vinLabel, "vinLabel");
    this.vinLabel.ContextMenu = this.contextMenu;
    this.vinLabel.Name = "vinLabel";
    this.vinLabel.UseMnemonic = false;
    this.vinLabel.MouseHover += new EventHandler(this.vinLabel_MouseHover);
    componentResourceManager.ApplyResources((object) this.esnLabel, "esnLabel");
    this.esnLabel.ContextMenu = this.contextMenu;
    this.esnLabel.Name = "esnLabel";
    this.esnLabel.UseMnemonic = false;
    this.esnLabel.MouseHover += new EventHandler(this.esnLabel_MouseHover);
    componentResourceManager.ApplyResources((object) this.equipmentLabel, "equipmentLabel");
    this.equipmentLabel.Name = "equipmentLabel";
    this.vinEsnTableLayout.SetRowSpan((Control) this.equipmentLabel, 2);
    this.equipmentLabel.UseMnemonic = false;
    this.equipmentLabel.MouseHover += new EventHandler(this.equipmentLabel_MouseHover);
    this.leftPanel.Controls.Add((Control) this.tableLayoutPanelLeft);
    componentResourceManager.ApplyResources((object) this.leftPanel, "leftPanel");
    this.leftPanel.Name = "leftPanel";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelLeft, "tableLayoutPanelLeft");
    this.tableLayoutPanelLeft.Controls.Add((Control) this.splitContainer, 0, 0);
    this.tableLayoutPanelLeft.Controls.Add((Control) this.stateLayoutPanel, 0, 1);
    this.tableLayoutPanelLeft.Name = "tableLayoutPanelLeft";
    componentResourceManager.ApplyResources((object) this.splitContainer, "splitContainer");
    this.splitContainer.Name = "splitContainer";
    this.splitContainer.Panel1.Controls.Add((Control) this.placesBar);
    this.splitContainer.Panel2.Controls.Add((Control) this.connections);
    this.connections.BackColor = SystemColors.Window;
    this.connections.BorderStyle = BorderStyle.FixedSingle;
    this.connections.Cursor = Cursors.Arrow;
    componentResourceManager.ApplyResources((object) this.connections, "connections");
    this.connections.ForeColor = SystemColors.WindowText;
    this.connections.Name = "connections";
    this.connections.ItemActivate += new EventHandler(this.OnConnectionItemActivate);
    componentResourceManager.ApplyResources((object) this.stateLayoutPanel, "stateLayoutPanel");
    this.stateLayoutPanel.Controls.Add((Control) this.doIPState, 2, 0);
    this.stateLayoutPanel.Controls.Add((Control) this.j1939State, 1, 0);
    this.stateLayoutPanel.Controls.Add((Control) this.j1708State, 0, 0);
    this.stateLayoutPanel.Name = "stateLayoutPanel";
    componentResourceManager.ApplyResources((object) this.doIPState, "doIPState");
    this.doIPState.Name = "doIPState";
    this.doIPState.Protocol = (Protocol) 13400;
    componentResourceManager.ApplyResources((object) this.j1939State, "j1939State");
    this.j1939State.Name = "j1939State";
    this.j1939State.Protocol = (Protocol) 71993;
    componentResourceManager.ApplyResources((object) this.j1708State, "j1708State");
    this.j1708State.Name = "j1708State";
    this.j1708State.Protocol = (Protocol) 71432;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((ContainerControl) this).AutoScaleMode = AutoScaleMode.Font;
    ((Control) this).Controls.Add((Control) this.contentPanel);
    ((Control) this).Controls.Add((Control) this.splitter);
    ((Control) this).Controls.Add((Control) this.leftPanel);
    ((Control) this).Name = nameof (TabbedView);
    ((ISupportInitialize) this.headerWatermark).EndInit();
    this.contentPanel.ResumeLayout(false);
    this.contentPanel.PerformLayout();
    this.headerPanel.ResumeLayout(false);
    this.headerPanel.PerformLayout();
    this.tableLayoutPanelPicture.ResumeLayout(false);
    ((ISupportInitialize) this.pictureBoxImage).EndInit();
    this.vinEsnTableLayout.ResumeLayout(false);
    this.vinEsnTableLayout.PerformLayout();
    this.leftPanel.ResumeLayout(false);
    this.tableLayoutPanelLeft.ResumeLayout(false);
    this.tableLayoutPanelLeft.PerformLayout();
    this.splitContainer.Panel1.ResumeLayout(false);
    this.splitContainer.Panel2.ResumeLayout(false);
    this.splitContainer.EndInit();
    this.splitContainer.ResumeLayout(false);
    this.stateLayoutPanel.ResumeLayout(false);
    this.stateLayoutPanel.PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
