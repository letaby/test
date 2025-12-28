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

namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public class TabbedView : ContextHelpControl, ISearchable, ISupportEdit, IProvideHtml, IRefreshable, IFilterable, ISupportExpandCollapseAll
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
		get
		{
			return splashProgressBar;
		}
		set
		{
			splashProgressBar = value;
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

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public int SelectedIndex
	{
		get
		{
			return placesBar.SelectedIndex;
		}
		set
		{
			placesBar.SelectedIndex = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public IList<IPlace> Tabs => placesBar.Places;

	public bool ShowSidebar
	{
		get
		{
			return leftPanel.Visible;
		}
		set
		{
			leftPanel.Visible = value;
			if (value)
			{
				connections.Dock = DockStyle.Fill;
				splitContainer.Panel2.Controls.Add(connections);
				connections.Visible = true;
			}
			else
			{
				connections.Visible = false;
				connections.Dock = DockStyle.None;
				((Control)this).Controls.Add(connections);
			}
		}
	}

	internal EcuStatusView StatusView => connections;

	internal ContextLinkButton ContextLinkButton => contextLinkButton;

	public bool SidebarEnabled
	{
		get
		{
			return leftPanel.Enabled;
		}
		set
		{
			leftPanel.Enabled = value;
		}
	}

	public bool CanSearch
	{
		get
		{
			bool result = false;
			Control control = currentPanel;
			ISearchable val = (ISearchable)(object)((control is ISearchable) ? control : null);
			if (val != null)
			{
				result = val.CanSearch;
			}
			return result;
		}
	}

	public bool CanUndo
	{
		get
		{
			editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
			return editSupport.CanUndo;
		}
	}

	public bool CanCopy
	{
		get
		{
			editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
			return editSupport.CanCopy;
		}
	}

	public bool CanDelete
	{
		get
		{
			editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
			return editSupport.CanDelete;
		}
	}

	public bool CanPaste
	{
		get
		{
			editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
			return editSupport.CanPaste;
		}
	}

	public bool CanSelectAll
	{
		get
		{
			editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
			return editSupport.CanSelectAll;
		}
	}

	public bool CanCut
	{
		get
		{
			editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
			return editSupport.CanCut;
		}
	}

	public bool CanProvideHtml => PrintHelper.CanProvideHtml(GetCurrentHtmlProvider());

	public string StyleSheet => PrintHelper.StyleSheet(GetCurrentHtmlProvider());

	public bool CanRefreshView
	{
		get
		{
			bool result = false;
			Control control = currentPanel;
			IRefreshable val = (IRefreshable)(object)((control is IRefreshable) ? control : null);
			if (val != null)
			{
				result = val.CanRefreshView;
			}
			return result;
		}
	}

	public IEnumerable<FilterTypes> Filters
	{
		get
		{
			IEnumerable<FilterTypes> enumerable = new List<FilterTypes>();
			foreach (object control in bodyPanel.Controls)
			{
				IFilterable val = (IFilterable)((control is IFilterable) ? control : null);
				if (val != null)
				{
					enumerable = enumerable.Union(val.Filters).ToList();
				}
			}
			return enumerable;
		}
	}

	public int NumberOfItemsFiltered => bodyPanel.Controls.OfType<IFilterable>().Sum((IFilterable filterableView) => filterableView.NumberOfItemsFiltered);

	public int TotalNumberOfFilterableItems => bodyPanel.Controls.OfType<IFilterable>().Sum((IFilterable filterableView) => filterableView.TotalNumberOfFilterableItems);

	public IFilterable FilterableChild
	{
		get
		{
			Control control = currentPanel;
			return (IFilterable)(object)((control is IFilterable) ? control : null);
		}
	}

	public bool CanExpandAllItems
	{
		get
		{
			bool result = false;
			Control control = currentPanel;
			ISupportExpandCollapseAll val = (ISupportExpandCollapseAll)(object)((control is ISupportExpandCollapseAll) ? control : null);
			if (val != null)
			{
				result = val.CanExpandAllItems;
			}
			return result;
		}
	}

	public bool CanCollapseAllItems
	{
		get
		{
			bool result = false;
			Control control = currentPanel;
			ISupportExpandCollapseAll val = (ISupportExpandCollapseAll)(object)((control is ISupportExpandCollapseAll) ? control : null);
			if (val != null)
			{
				result = val.CanCollapseAllItems;
			}
			return result;
		}
	}

	public event EventHandler FilteredContentChanged;

	public void SetBranding(BrandingBase branding)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		headerWatermark.Image = branding.HeaderWatermark;
		headerPanel.ForeColor = SystemColors.WindowText;
		headerPanel.BackColor = SystemColors.Window;
		placesBar.View = SettingsManager.GlobalInstance.GetValue<ViewTypes>("PlacesView", "MainWindow", (ViewTypes)3);
		placesBar.ButtonTextImageLayout = SettingsManager.GlobalInstance.GetValue<ButtonLayout>("PlacesLayout", "MainWindow", (ButtonLayout)4);
		UpdateBanner();
	}

	public void OnDispose(bool disposing)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!disposing)
		{
			return;
		}
		SettingsManager.GlobalInstance.SetValue<ViewTypes>("PlacesView", "MainWindow", placesBar.View, false);
		SettingsManager.GlobalInstance.SetValue<ButtonLayout>("PlacesLayout", "MainWindow", placesBar.ButtonTextImageLayout, false);
		foreach (IFilterable item in bodyPanel.Controls.OfType<IFilterable>())
		{
			item.FilteredContentChanged -= filterableItem_FilteredContentChanged;
		}
		foreach (IMenuProxy menuProxy in menuProxies)
		{
			if (menuProxy is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		ThemeProvider.GlobalInstance.ThemeChanged -= OnThemeChanged;
	}

	public TabbedView()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		InitializeComponent();
		contextLinkButton.Context = (IContextHelp)new ContextHelpChain((object)this, LinkSupport.MainOnlineHelp);
		ThemeProvider.GlobalInstance.ThemeChanged += OnThemeChanged;
	}

	protected override void OnLoad(EventArgs e)
	{
		UpdateBannerColors();
		BuildPlaces();
		UpdateScaling();
		if (!((Component)this).DesignMode)
		{
			UpdateVinAndEsnLabel();
			SapiManager.GlobalInstance.EquipmentTypeChanged += GlobalInstance_EquipmentTypeChanged;
			SapiManager.GlobalInstance.ChannelIdentificationChanged += GlobalInstance_ChannelIdentificationChanged;
			SapiManager.GlobalInstance.ConnectedUnitChanged += GlobalInstance_ConnectedUnitChanged;
			SapiManager.GlobalInstance.DisplayPinWarningDataItemChanged += GlobalInstance_DisplayPinWarningDataItemChanged;
			doIPState.Visible = ApplicationInformation.CanRollCallDoIP;
		}
		((ContainerControl)this).ParentForm.Shown += ParentForm_Shown;
		((ContainerControl)this).ParentForm.FormClosing += ParentForm_FormClosing;
		j1939State.VisibleChanged += J1939State_VisibleChanged;
		splitContainer.FixedPanel = FixedPanel.Panel1;
		splitContainer.SplitterDistance = SettingsManager.GlobalInstance.GetValue<int>("PlacesSplitterDistance", "MainWindow", splitContainer.SplitterDistance);
		((UserControl)this).OnLoad(e);
	}

	private void ParentForm_Shown(object sender, EventArgs e)
	{
		((Control)(object)this).Refresh();
		splitContainer.SplitterDistance = SettingsManager.GlobalInstance.GetValue<int>("PlacesSplitterDistance", "MainWindow", splitContainer.SplitterDistance);
		splitContainer.FixedPanel = FixedPanel.None;
	}

	private void J1939State_VisibleChanged(object sender, EventArgs e)
	{
		splitContainer.SplitterDistance = SettingsManager.GlobalInstance.GetValue<int>("PlacesSplitterDistance", "MainWindow", splitContainer.SplitterDistance);
	}

	private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		SettingsManager.GlobalInstance.SetValue<int>("PlacesSplitterDistance", "MainWindow", splitContainer.SplitterDistance, false);
	}

	private void GlobalInstance_ConnectedUnitChanged(object sender, EventArgs e)
	{
		UpdateVinAndEsnLabel();
	}

	private void GlobalInstance_ChannelIdentificationChanged(object sender, ChannelIdentificationChangedEventArgs e)
	{
		UpdateVinAndEsnLabel();
	}

	private void GlobalInstance_DisplayPinWarningDataItemChanged(object sender, DisplayPinWarningDataItemChangedEventArgs e)
	{
		if (e.DisplayWarning)
		{
			WarningsPanel.GlobalInstance.Add("TIER4PINERROR", MessageBoxIcon.Hand, (string)null, Resources.Tier4PinError_UserText, (EventHandler)Tier4PinError_Click);
		}
		else
		{
			WarningsPanel.GlobalInstance.Remove("TIER4PINERROR");
		}
	}

	private void GlobalInstance_EquipmentTypeChanged(object sender, EquipmentTypeChangedEventArgs e)
	{
		UpdateEquipmentLabel();
	}

	private void Tier4PinError_Click(object sender, EventArgs e)
	{
		ActionsMenuProxy.GlobalInstance.ShowDialog("Set Engine Serial Number/Product Identification Number", "Off-Highway", (object)null, false);
	}

	private void UpdateEquipmentLabel()
	{
		List<string> list = new List<string>();
		list.AddRange(from g in SapiManager.GlobalInstance.ConnectedEquipment.Where(delegate(EquipmentType c)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				ElectronicsFamily family = ((EquipmentType)(ref c)).Family;
				return ((ElectronicsFamily)(ref family)).Category != "Vehicle";
			}).GroupBy(delegate(EquipmentType e)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				ElectronicsFamily family = ((EquipmentType)(ref e)).Family;
				return ((ElectronicsFamily)(ref family)).Category;
			})
			select g.Key + ": " + string.Join(",", g.Select((EquipmentType et) => ((EquipmentType)(ref et)).CompleteName).ToArray()));
		equipmentLabel.Text = string.Join(Environment.NewLine, list.ToArray());
	}

	private static string BuildInfoContent(string separator, VinInformation vinInformation, string[] qualifiers)
	{
		List<string> list = new List<string>();
		foreach (string text in qualifiers)
		{
			string informationValue = vinInformation.GetInformationValue(text);
			if (!string.IsNullOrEmpty(informationValue))
			{
				list.Add(informationValue);
			}
		}
		return string.Join(separator, list);
	}

	private void equipmentLabel_MouseHover(object sender, EventArgs e)
	{
		IEnumerable<EquipmentType> connectedEquipment = SapiManager.GlobalInstance.ConnectedEquipment;
		if (connectedEquipment.Any())
		{
			string text = string.Join(Environment.NewLine, from g in connectedEquipment.GroupBy(delegate(EquipmentType eq)
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					ElectronicsFamily family = ((EquipmentType)(ref eq)).Family;
					return ((ElectronicsFamily)(ref family)).Category;
				})
				select g.Key + ": " + string.Join(",", g.Select((EquipmentType et) => ((EquipmentType)(ref et)).CompleteName).ToArray()));
			equipmentToolTip.Active = true;
			equipmentToolTip.Show(text, equipmentLabel, 32767);
		}
		else
		{
			equipmentToolTip.Active = false;
		}
	}

	private void vinInfoLabel_MouseHover(object sender, EventArgs e)
	{
		VinInformation val = null;
		if (!string.IsNullOrEmpty(displayedVin))
		{
			val = VinInformation.GetVinInformation(displayedVin);
		}
		if (val != null && val.InformationEntries.Any())
		{
			string text = string.Join(Environment.NewLine, val.InformationEntries.Select((KeyValuePair<string, string> ie) => VinDecodingInformation.GetTranslatedGroupName(ie.Key) + ": " + ie.Value));
			vinInfoToolTip.Active = true;
			vinInfoToolTip.Show(text, vinInfoLabel, 32767);
		}
		else
		{
			vinInfoToolTip.Active = false;
		}
	}

	private void UpdateVinAndEsnLabel()
	{
		string currentVehicleIdentification = SapiManager.GlobalInstance.CurrentVehicleIdentification;
		string currentEngineSerialNumber = SapiManager.GlobalInstance.CurrentEngineSerialNumber;
		if (currentVehicleIdentification != displayedVin)
		{
			VinInformation val = null;
			displayedVin = currentVehicleIdentification;
			if (!string.IsNullOrEmpty(currentVehicleIdentification))
			{
				vinLabel.Visible = true;
				vinLabel.Text = string.Format(CultureInfo.CurrentCulture, Resources.TabbedView_FormatVIN, currentVehicleIdentification);
				val = VinInformation.GetVinInformation(displayedVin);
			}
			else
			{
				vinLabel.Visible = false;
			}
			if (val != null && val.InformationEntries.Any())
			{
				vinInfoLabel.Text = BuildInfoContent(" ", val, new string[4] { "modelyear", "make", "model", "Chassis" });
				vinInfoLabel.Visible = true;
			}
			else
			{
				vinInfoLabel.Visible = false;
			}
		}
		vinLabel.ForeColor = (SapiManager.GlobalInstance.VehicleIdentificationInconsistency ? Color.Red : equipmentLabel.ForeColor);
		if (currentEngineSerialNumber != displayedEsn)
		{
			displayedEsn = currentEngineSerialNumber;
			if (!string.IsNullOrEmpty(currentEngineSerialNumber))
			{
				esnLabel.Visible = true;
				esnLabel.Text = string.Format(CultureInfo.CurrentCulture, Resources.TabbedView_FormatESN, currentEngineSerialNumber);
			}
			else
			{
				esnLabel.Visible = false;
			}
		}
		esnLabel.ForeColor = (SapiManager.GlobalInstance.EngineSerialInconsistency ? Color.Red : equipmentLabel.ForeColor);
		UpdateEquipmentLabel();
	}

	private void vinLabel_MouseHover(object sender, EventArgs e)
	{
		vinToolTip.Active = false;
		if (SapiManager.GlobalInstance.VehicleIdentificationInconsistency)
		{
			vinToolTip.Active = true;
			vinToolTip.Show(Resources.TabbedView_ToolTip_VIN, vinLabel, 5000);
		}
	}

	private void esnLabel_MouseHover(object sender, EventArgs e)
	{
		esnToolTip.Active = false;
		if (SapiManager.GlobalInstance.EngineSerialInconsistency)
		{
			esnToolTip.Active = true;
			esnToolTip.Show(Resources.TabbedView_ToolTip_ESN, esnLabel, 5000);
		}
	}

	private void contextMenu_Popup(object sender, EventArgs e)
	{
		copyVinCommand.Enabled = !string.IsNullOrEmpty(vinLabel.Text);
		copyEsnCommand.Enabled = !string.IsNullOrEmpty(esnLabel.Text);
	}

	private void copyVinCommand_Click(object sender, EventArgs e)
	{
		Clipboard.SetText(vinLabel.Text.Substring(5));
	}

	private void copyEsnCommand_Click(object sender, EventArgs e)
	{
		Clipboard.SetText(esnLabel.Text.Substring(5));
	}

	private void OnBannerSizeChanged(object sender, EventArgs e)
	{
		UpdateBanner();
	}

	private void BuildPlaces()
	{
		IEnumerable<string> enumerable = from f in new string[6] { "EcuInfo.dll", "EcuStatus.dll", "Troubleshooting.dll", "Instruments.dll", "Parameters.dll", "Reprogramming.dll" }
			select Path.Combine(Application.StartupPath, f) into p
			where File.Exists(p)
			select p;
		foreach (string item in enumerable)
		{
			Assembly assembly = FileManagement.LoadAssembly(item);
			try
			{
				AddPanels(assembly);
			}
			catch (FileLoadException)
			{
				AssemblyName name = Assembly.GetExecutingAssembly().GetName();
				AssemblyName name2 = assembly.GetName();
				if (name2.Version != name.Version)
				{
					ControlHelpers.ShowMessageBox(string.Format(CultureInfo.CurrentCulture, Resources.Program_FormatInconsistentInstallation, ApplicationInformation.ProductName, name.Name, name.Version, name2.Name, name2.Version), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
				}
				throw;
			}
		}
		if (placesBar.IdealWidth > splitter.SplitPosition)
		{
			splitter.SplitPosition = placesBar.IdealWidth + 1;
		}
		if (placesBar.Places.Count > placesBar.SelectedIndex && placesBar.Places.Count > 0 && !ChangePanel(placesBar.Places[placesBar.SelectedIndex]))
		{
			throw new InvalidOperationException("Failed to display initial panel.");
		}
	}

	private void AddPanels(Assembly assembly)
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (!(assembly != null))
		{
			return;
		}
		object[] customAttributes = assembly.GetCustomAttributes(typeof(PanelAttribute), inherit: false);
		if (customAttributes.Length == 0)
		{
			return;
		}
		bool flag = false;
		byte[] publicKey = ((object)this).GetType().Assembly.GetName().GetPublicKey();
		byte[] publicKey2 = assembly.GetName().GetPublicKey();
		if (publicKey.Length == publicKey2.Length)
		{
			flag = true;
			for (int i = 0; i < publicKey.Length && flag; i++)
			{
				if (publicKey[i] != publicKey2[i])
				{
					flag = false;
				}
			}
		}
		if (!flag)
		{
			return;
		}
		object[] array = customAttributes;
		for (int j = 0; j < array.Length; j++)
		{
			PanelAttribute val = (PanelAttribute)array[j];
			if (!ApplicationInformation.IsViewVisible(val.PositionIdentifier))
			{
				continue;
			}
			if (splashProgressBar != null)
			{
				splashProgressBar.OverallStatusText = string.Format(CultureInfo.CurrentCulture, Resources.StartupStatus_Format_LoadingView, val.Title);
			}
			IPlace place = placesBar.Add(val);
			if (val.MenuProxy != null)
			{
				object value = val.MenuProxy.GetProperty("GlobalInstance").GetValue(null, null);
				IMenuProxy val2 = (IMenuProxy)((value is IMenuProxy) ? value : null);
				val2.ContainerApplication = containerApplication;
				val2.Place = place;
				if (val2.Menu != null)
				{
					ToolStripManager.Merge(val2.Menu, containerApplication.Menu);
				}
				menuProxies.Add(val2);
			}
			if (val.Preload)
			{
				EnsurePanelCreated(place);
			}
		}
	}

	private void OnBannerTextChanged(object sender, EventArgs e)
	{
		UpdateBanner();
	}

	private void UpdateBanner()
	{
		bool visible = false;
		if (headerWatermark.Image != null)
		{
			Size size = headerWatermark.Image.Size;
			size.Width = headerPanel.Width - headerWatermark.Image.Width - ((Control)(object)contextLinkButton).Width - MarginWidth(headerWatermark) - MarginWidth(headerLabel) - MarginWidth(vinEsnTableLayout) - MarginWidth(vinLabel) - MarginWidth(equipmentLabel) - MarginWidth((Control)(object)contextLinkButton) - MarginWidth(tableLayoutPanelPicture);
			if (size.Width > 0)
			{
				int num = TextRenderer.MeasureText(headerLabel.Text, headerLabel.Font).Width + tableLayoutPanelPicture.Size.Width + Math.Max(TextRenderer.MeasureText(vinInfoLabel.Text, vinInfoLabel.Font).Width, TextRenderer.MeasureText(equipmentLabel.Text, equipmentLabel.Font).Width + TextRenderer.MeasureText(vinLabel.Text, vinLabel.Font).Width);
				visible = ((num <= size.Width) ? true : false);
			}
		}
		headerWatermark.Visible = visible;
		static int MarginWidth(Control control)
		{
			return control.Margin.Left + control.Margin.Right;
		}
	}

	private void UpdateBannerColors()
	{
		((Control)(object)contextLinkButton).ForeColor = headerPanel.ForeColor;
		contextLinkButton.FlatAppearance.MouseOverBackColor = (((double)headerPanel.BackColor.GetBrightness() > 0.75) ? ControlPaint.Dark(headerPanel.BackColor) : ControlPaint.LightLight(headerPanel.BackColor));
		headerPanel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 12f, FontStyle.Bold);
		equipmentLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 8f, FontStyle.Regular);
		vinLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 8f, FontStyle.Regular);
		esnLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 8f, FontStyle.Regular);
		vinInfoLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 8f, FontStyle.Bold);
	}

	[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	protected override void WndProc(ref Message m)
	{
		int msg = m.Msg;
		if (msg == 794)
		{
			UpdateBannerColors();
		}
		((UserControl)this).WndProc(ref m);
	}

	private void OnPlacesBarSelectionChanging(object sender, PlacesBarCancelEventArgs e)
	{
		((CancelEventArgs)(object)e).Cancel = !ChangePanel(e.Place);
	}

	private void OnContextLinkChanged(object sender, EventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		((ContextHelpControl)this).SetLink(LinkSupport.GetContextHelpLink((object)currentPanel));
	}

	private bool ChangePanel(IPlace place)
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		if (place != null)
		{
			((Control)this).SuspendLayout();
			((Control)(object)this).Cursor = Cursors.WaitCursor;
			Control control = EnsurePanelCreated(place);
			Control control2 = currentPanel;
			LinkSupport.UnsubscribeChanges((object)control2, (EventHandler)OnContextLinkChanged);
			ISupportActivation val = (ISupportActivation)(object)((control2 is ISupportActivation) ? control2 : null);
			if (val == null || val.Deactivate())
			{
				headerLabel.Text = control.Name;
				pictureBoxImage.Image = place.Image ?? null;
				control2?.Hide();
				currentPanel = control;
				control.Show();
				ViewHistory.GlobalInstance.Add(place);
				UsageTracking.MarkNavigationLocation(place);
				Control control3 = currentPanel;
				ISupportActivation val2 = (ISupportActivation)(object)((control3 is ISupportActivation) ? control3 : null);
				if (val2 != null)
				{
					val2.Activate();
				}
				LinkSupport.SubscribeChanges((object)control, (EventHandler)OnContextLinkChanged);
				((ContextHelpControl)this).SetLink(LinkSupport.GetContextHelpLink((object)control));
				control.Focus();
				result = true;
			}
			OnFilteredContentChanged();
			((Control)(object)this).Cursor = Cursors.Default;
			((Control)this).ResumeLayout(performLayout: true);
		}
		return result;
	}

	public Control EnsurePanelCreated(IPlace place)
	{
		Control control = place.Data as Control;
		if (control == null)
		{
			Type type = place.Data as Type;
			if (type != null)
			{
				control = Activator.CreateInstance(type) as Control;
				control.Hide();
				control.Name = place.Title;
				control.Dock = DockStyle.Fill;
				control.TabStop = true;
				place.Data = control;
				if (!bodyPanel.Controls.Contains(control))
				{
					bodyPanel.Controls.Add(control);
					IFilterable val = (IFilterable)(object)((control is IFilterable) ? control : null);
					if (val != null)
					{
						val.FilteredContentChanged += filterableItem_FilteredContentChanged;
					}
				}
				control.Size = bodyPanel.ClientSize;
			}
		}
		return control;
	}

	private void filterableItem_FilteredContentChanged(object sender, EventArgs e)
	{
		OnFilteredContentChanged();
	}

	public void SelectPlace(PanelIdentifier panelId, string location)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		placesBar.SelectMatch(panelId);
		if (!string.IsNullOrEmpty(location))
		{
			Control control = currentPanel;
			ISupportActivation val = (ISupportActivation)(object)((control is ISupportActivation) ? control : null);
			if (val != null)
			{
				val.SelectLocation(location);
			}
		}
	}

	public void SelectPlace(int index)
	{
		placesBar.SelectPlace(index);
	}

	public void SelectPlace(IPlace place)
	{
		placesBar.SelectMatch(place);
	}

	public void SelectNext()
	{
		placesBar.SelectNext();
	}

	public void SelectPrevious()
	{
		placesBar.SelectPrevious();
	}

	private void BodySizeChanged(object sender, EventArgs e)
	{
		UpdateScaling();
	}

	private void OnThemeChanged(object sender, EventArgs e)
	{
		UpdateScaling();
	}

	protected override void OnCreateControl()
	{
		((UserControl)this).OnCreateControl();
		using Graphics graphics = ((Control)this).CreateGraphics();
		dpiX = graphics.DpiX;
		dpiY = graphics.DpiY;
	}

	private void UpdateScaling()
	{
		float num = (float)bodyPanel.Width / 640f * 96f / dpiX;
		float num2 = (float)bodyPanel.Height / 480f * 96f / dpiY;
		float num3 = ((num < num2) ? num : num2);
		ThemeProvider.GlobalInstance.ActiveTheme.WindowScale = num3;
	}

	private IProvideHtml GetCurrentHtmlProvider()
	{
		IProvideHtml result = null;
		int selectedIndex = placesBar.SelectedIndex;
		if (0 <= selectedIndex && selectedIndex < Tabs.Count)
		{
			object data = Tabs[selectedIndex].Data;
			result = (IProvideHtml)((data is IProvideHtml) ? data : null);
		}
		return result;
	}

	private void OnConnectionItemActivate(object sender, EventArgs e)
	{
		placesBar.SelectMatch((PanelIdentifier)1);
	}

	public bool Search(string searchText, bool caseSensitive, FindMode direction)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		Control control = currentPanel;
		ISearchable val = (ISearchable)(object)((control is ISearchable) ? control : null);
		if (val != null)
		{
			result = val.Search(searchText, caseSensitive, direction);
		}
		return result;
	}

	public void Undo()
	{
		editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
		editSupport.Undo();
	}

	public void Copy()
	{
		editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
		editSupport.Copy();
	}

	public void Cut()
	{
		editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
		editSupport.Cut();
	}

	public void Delete()
	{
		editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
		editSupport.Delete();
	}

	public void Paste()
	{
		editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
		editSupport.Paste();
	}

	public void SelectAll()
	{
		editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
		editSupport.SelectAll();
	}

	public string ToHtml()
	{
		return PrintHelper.ToHtml(GetCurrentHtmlProvider());
	}

	public void RefreshView()
	{
		Control control = currentPanel;
		IRefreshable val = (IRefreshable)(object)((control is IRefreshable) ? control : null);
		if (val != null)
		{
			val.RefreshView();
		}
	}

	private void OnFilteredContentChanged()
	{
		if (this.FilteredContentChanged != null)
		{
			this.FilteredContentChanged(this, new EventArgs());
		}
	}

	public void ExpandAllItems()
	{
		Control control = currentPanel;
		ISupportExpandCollapseAll val = (ISupportExpandCollapseAll)(object)((control is ISupportExpandCollapseAll) ? control : null);
		if (val != null)
		{
			val.ExpandAllItems();
		}
	}

	public void CollapseAllItems()
	{
		Control control = currentPanel;
		ISupportExpandCollapseAll val = (ISupportExpandCollapseAll)(object)((control is ISupportExpandCollapseAll) ? control : null);
		if (val != null)
		{
			val.CollapseAllItems();
		}
	}

	protected override void Dispose(bool disposing)
	{
		OnDispose(disposing);
		((ContextHelpControl)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(TabbedView));
		placesBar = new PlacesBar();
		headerLabel = new Label();
		contextLinkButton = new ContextLinkButton();
		headerWatermark = new PictureBox();
		bodyPanel = new Panel();
		splitter = new Splitter();
		contentPanel = new Panel();
		warningsPanel = new WarningsPanel();
		headerPanel = new TableLayoutPanel();
		tableLayoutPanelPicture = new TableLayoutPanel();
		pictureBoxImage = new PictureBox();
		vinEsnTableLayout = new TableLayoutPanel();
		vinInfoLabel = new Label();
		contextMenu = new ContextMenu();
		copyVinCommand = new MenuItem();
		copyEsnCommand = new MenuItem();
		vinLabel = new Label();
		esnLabel = new Label();
		equipmentLabel = new Label();
		leftPanel = new Panel();
		tableLayoutPanelLeft = new TableLayoutPanel();
		splitContainer = new SplitContainer();
		connections = new EcuStatusView();
		stateLayoutPanel = new TableLayoutPanel();
		doIPState = new ConnectionState();
		j1939State = new ConnectionState();
		j1708State = new ConnectionState();
		((ISupportInitialize)headerWatermark).BeginInit();
		contentPanel.SuspendLayout();
		headerPanel.SuspendLayout();
		tableLayoutPanelPicture.SuspendLayout();
		((ISupportInitialize)pictureBoxImage).BeginInit();
		vinEsnTableLayout.SuspendLayout();
		leftPanel.SuspendLayout();
		tableLayoutPanelLeft.SuspendLayout();
		((ISupportInitialize)splitContainer).BeginInit();
		splitContainer.Panel1.SuspendLayout();
		splitContainer.Panel2.SuspendLayout();
		splitContainer.SuspendLayout();
		stateLayoutPanel.SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)(object)placesBar).BackColor = SystemColors.Window;
		componentResourceManager.ApplyResources(placesBar, "placesBar");
		((Control)(object)placesBar).ForeColor = SystemColors.WindowText;
		((Control)(object)placesBar).Name = "placesBar";
		placesBar.SelectionChanging += OnPlacesBarSelectionChanging;
		componentResourceManager.ApplyResources(headerLabel, "headerLabel");
		headerLabel.Name = "headerLabel";
		headerLabel.UseCompatibleTextRendering = true;
		headerLabel.UseMnemonic = false;
		headerLabel.TextChanged += OnBannerTextChanged;
		componentResourceManager.ApplyResources(contextLinkButton, "contextLinkButton");
		contextLinkButton.FlatAppearance.BorderSize = 0;
		contextLinkButton.FlatStyle = FlatStyle.Flat;
		contextLinkButton.ImageAlign = ContentAlignment.MiddleCenter;
		((Control)(object)contextLinkButton).Name = "contextLinkButton";
		contextLinkButton.ShowImage = true;
		contextLinkButton.TextAlign = ContentAlignment.MiddleLeft;
		componentResourceManager.ApplyResources(headerWatermark, "headerWatermark");
		headerWatermark.Name = "headerWatermark";
		headerWatermark.TabStop = false;
		componentResourceManager.ApplyResources(bodyPanel, "bodyPanel");
		bodyPanel.Name = "bodyPanel";
		bodyPanel.SizeChanged += BodySizeChanged;
		componentResourceManager.ApplyResources(splitter, "splitter");
		splitter.Name = "splitter";
		splitter.TabStop = false;
		contentPanel.Controls.Add(bodyPanel);
		contentPanel.Controls.Add((Control)(object)warningsPanel);
		contentPanel.Controls.Add(headerPanel);
		componentResourceManager.ApplyResources(contentPanel, "contentPanel");
		contentPanel.Name = "contentPanel";
		componentResourceManager.ApplyResources(warningsPanel, "warningsPanel");
		((Control)(object)warningsPanel).BackColor = Color.LightYellow;
		((Control)(object)warningsPanel).Name = "warningsPanel";
		componentResourceManager.ApplyResources(headerPanel, "headerPanel");
		headerPanel.BackColor = SystemColors.ControlDarkDark;
		headerPanel.Controls.Add(headerWatermark, 5, 0);
		headerPanel.Controls.Add((Control)(object)contextLinkButton, 3, 0);
		headerPanel.Controls.Add(headerLabel, 1, 0);
		headerPanel.Controls.Add(tableLayoutPanelPicture, 0, 0);
		headerPanel.Controls.Add(vinEsnTableLayout, 4, 0);
		headerPanel.Name = "headerPanel";
		headerPanel.SizeChanged += OnBannerSizeChanged;
		componentResourceManager.ApplyResources(tableLayoutPanelPicture, "tableLayoutPanelPicture");
		tableLayoutPanelPicture.Controls.Add(pictureBoxImage, 1, 1);
		tableLayoutPanelPicture.Name = "tableLayoutPanelPicture";
		componentResourceManager.ApplyResources(pictureBoxImage, "pictureBoxImage");
		pictureBoxImage.Name = "pictureBoxImage";
		pictureBoxImage.TabStop = false;
		componentResourceManager.ApplyResources(vinEsnTableLayout, "vinEsnTableLayout");
		vinEsnTableLayout.Controls.Add(vinInfoLabel, 0, 1);
		vinEsnTableLayout.Controls.Add(vinLabel, 0, 2);
		vinEsnTableLayout.Controls.Add(esnLabel, 0, 3);
		vinEsnTableLayout.Controls.Add(equipmentLabel, 1, 2);
		vinEsnTableLayout.Name = "vinEsnTableLayout";
		componentResourceManager.ApplyResources(vinInfoLabel, "vinInfoLabel");
		vinEsnTableLayout.SetColumnSpan(vinInfoLabel, 2);
		vinInfoLabel.ContextMenu = contextMenu;
		vinInfoLabel.Name = "vinInfoLabel";
		vinInfoLabel.UseMnemonic = false;
		vinInfoLabel.MouseHover += vinInfoLabel_MouseHover;
		contextMenu.MenuItems.AddRange(new MenuItem[2] { copyVinCommand, copyEsnCommand });
		contextMenu.Popup += contextMenu_Popup;
		copyVinCommand.Index = 0;
		componentResourceManager.ApplyResources(copyVinCommand, "copyVinCommand");
		copyVinCommand.Click += copyVinCommand_Click;
		copyEsnCommand.Index = 1;
		componentResourceManager.ApplyResources(copyEsnCommand, "copyEsnCommand");
		copyEsnCommand.Click += copyEsnCommand_Click;
		componentResourceManager.ApplyResources(vinLabel, "vinLabel");
		vinLabel.ContextMenu = contextMenu;
		vinLabel.Name = "vinLabel";
		vinLabel.UseMnemonic = false;
		vinLabel.MouseHover += vinLabel_MouseHover;
		componentResourceManager.ApplyResources(esnLabel, "esnLabel");
		esnLabel.ContextMenu = contextMenu;
		esnLabel.Name = "esnLabel";
		esnLabel.UseMnemonic = false;
		esnLabel.MouseHover += esnLabel_MouseHover;
		componentResourceManager.ApplyResources(equipmentLabel, "equipmentLabel");
		equipmentLabel.Name = "equipmentLabel";
		vinEsnTableLayout.SetRowSpan(equipmentLabel, 2);
		equipmentLabel.UseMnemonic = false;
		equipmentLabel.MouseHover += equipmentLabel_MouseHover;
		leftPanel.Controls.Add(tableLayoutPanelLeft);
		componentResourceManager.ApplyResources(leftPanel, "leftPanel");
		leftPanel.Name = "leftPanel";
		componentResourceManager.ApplyResources(tableLayoutPanelLeft, "tableLayoutPanelLeft");
		tableLayoutPanelLeft.Controls.Add(splitContainer, 0, 0);
		tableLayoutPanelLeft.Controls.Add(stateLayoutPanel, 0, 1);
		tableLayoutPanelLeft.Name = "tableLayoutPanelLeft";
		componentResourceManager.ApplyResources(splitContainer, "splitContainer");
		splitContainer.Name = "splitContainer";
		splitContainer.Panel1.Controls.Add((Control)(object)placesBar);
		splitContainer.Panel2.Controls.Add(connections);
		connections.BackColor = SystemColors.Window;
		connections.BorderStyle = BorderStyle.FixedSingle;
		connections.Cursor = Cursors.Arrow;
		componentResourceManager.ApplyResources(connections, "connections");
		connections.ForeColor = SystemColors.WindowText;
		connections.Name = "connections";
		connections.ItemActivate += OnConnectionItemActivate;
		componentResourceManager.ApplyResources(stateLayoutPanel, "stateLayoutPanel");
		stateLayoutPanel.Controls.Add(doIPState, 2, 0);
		stateLayoutPanel.Controls.Add(j1939State, 1, 0);
		stateLayoutPanel.Controls.Add(j1708State, 0, 0);
		stateLayoutPanel.Name = "stateLayoutPanel";
		componentResourceManager.ApplyResources(doIPState, "doIPState");
		doIPState.Name = "doIPState";
		doIPState.Protocol = (Protocol)13400;
		componentResourceManager.ApplyResources(j1939State, "j1939State");
		j1939State.Name = "j1939State";
		j1939State.Protocol = (Protocol)71993;
		componentResourceManager.ApplyResources(j1708State, "j1708State");
		j1708State.Name = "j1708State";
		j1708State.Protocol = (Protocol)71432;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)this).Controls.Add(contentPanel);
		((Control)this).Controls.Add(splitter);
		((Control)this).Controls.Add(leftPanel);
		((Control)this).Name = "TabbedView";
		((ISupportInitialize)headerWatermark).EndInit();
		contentPanel.ResumeLayout(performLayout: false);
		contentPanel.PerformLayout();
		headerPanel.ResumeLayout(performLayout: false);
		headerPanel.PerformLayout();
		tableLayoutPanelPicture.ResumeLayout(performLayout: false);
		((ISupportInitialize)pictureBoxImage).EndInit();
		vinEsnTableLayout.ResumeLayout(performLayout: false);
		vinEsnTableLayout.PerformLayout();
		leftPanel.ResumeLayout(performLayout: false);
		tableLayoutPanelLeft.ResumeLayout(performLayout: false);
		tableLayoutPanelLeft.PerformLayout();
		splitContainer.Panel1.ResumeLayout(performLayout: false);
		splitContainer.Panel2.ResumeLayout(performLayout: false);
		((ISupportInitialize)splitContainer).EndInit();
		splitContainer.ResumeLayout(performLayout: false);
		stateLayoutPanel.ResumeLayout(performLayout: false);
		stateLayoutPanel.PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
