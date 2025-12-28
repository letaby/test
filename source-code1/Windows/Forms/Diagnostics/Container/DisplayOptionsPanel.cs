// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.DisplayOptionsPanel
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Common;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using DetroitDiesel.Windows.Forms.Themed;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal class DisplayOptionsPanel : OptionsPanel
{
  private readonly SapiManager sapi = SapiManager.GlobalInstance;
  private readonly ThemeProvider themeProvider = ThemeProvider.GlobalInstance;
  private IContainer components;
  private ThemePreview themePreview;
  private ComboBox comboBoxFontSize;
  private ComboBox comboBoxTheme;
  private ComboBox comboBoxUnitSystem;
  private ComboBox comboBoxLocale;
  private ComboBox comboBoxListBy;
  private Label labelListBy;
  private CheckBox checkBoxViewUpdateRate;
  private TableLayoutPanel tableLayoutPanel;
  private CheckBox checkBoxCoalesceFaultCodes;

  public DisplayOptionsPanel()
  {
    this.MinAccessLevel = 1;
    this.InitializeComponent();
    this.HeaderImage = (Image) new Bitmap(this.GetType(), "option_theme.png");
    this.PopulateThemesCombo();
    this.PopulateFontSizeCombo();
    this.PopulateUnitsCombo();
    this.PopulateLocaleCombo();
    if (!ApplicationInformation.CanListByMessage)
    {
      this.comboBoxListBy.Visible = false;
      this.labelListBy.Visible = false;
    }
    if (!ApplicationInformation.CanViewInstrumentUpdateRate)
      this.checkBoxViewUpdateRate.Visible = false;
    this.checkBoxCoalesceFaultCodes.Visible = false;
  }

  private void PopulateUnitsCombo()
  {
    foreach (UnitsSystemSelection unitChoice in (IEnumerable<UnitsSystemSelection>) SapiManager.UnitChoices)
      this.comboBoxUnitSystem.Items.Add((object) unitChoice);
  }

  private void PopulateThemesCombo()
  {
    foreach (object themeDefinition in (IEnumerable<ThemeDefinition>) this.themeProvider.ThemeDefinitions)
      this.comboBoxTheme.Items.Add(themeDefinition);
  }

  private void PopulateFontSizeCombo()
  {
    this.comboBoxFontSize.Items.Add((object) "50%");
    this.comboBoxFontSize.Items.Add((object) "75%");
    this.comboBoxFontSize.Items.Add((object) "100%");
    this.comboBoxFontSize.Items.Add((object) "150%");
    this.comboBoxFontSize.Items.Add((object) "200%");
  }

  private void PopulateLocaleCombo()
  {
    this.comboBoxLocale.DisplayMember = "DisplayName";
    this.comboBoxLocale.Items.Add((object) DisplayOptionsPanel.CurrentSystemCulture.Instance);
    foreach (object supportedCulture in LanguageManager.EnumerateSupportedCultures())
      this.comboBoxLocale.Items.Add(supportedCulture);
  }

  private void UpdatePreview()
  {
    if (this.comboBoxTheme.SelectedItem != null)
      this.themePreview.Theme = new ThemeDefinition((ThemeDefinition) this.comboBoxTheme.SelectedItem);
    try
    {
      if (this.comboBoxFontSize.SelectedItem != null)
        this.themePreview.Theme.FontScale = (double) float.Parse(this.comboBoxFontSize.SelectedItem.ToString().Replace("%", ""), (IFormatProvider) CultureInfo.CurrentCulture) / 100.0;
    }
    catch (FormatException ex)
    {
      int num = (int) MessageBox.Show(ex.Message, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, ControlHelpers.IsRightToLeft((Control) this) ? MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading : (MessageBoxOptions) 0);
    }
    if (this.comboBoxLocale.SelectedItem == null)
      return;
    if (this.comboBoxLocale.SelectedItem is DisplayOptionsPanel.CurrentSystemCulture)
      this.themePreview.PreviewUICulture = CultureInfo.CurrentUICulture;
    else
      this.themePreview.PreviewUICulture = (CultureInfo) this.comboBoxLocale.SelectedItem;
  }

  protected override void OnLoad(EventArgs e)
  {
    this.comboBoxTheme.SelectedItem = (object) this.themeProvider.ActiveTheme;
    this.comboBoxFontSize.SelectedItem = (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatPercentage, (object) (this.themeProvider.ActiveTheme.FontScale * 100.0));
    this.comboBoxUnitSystem.SelectedItem = (object) this.sapi.UserUnits;
    if (string.IsNullOrEmpty(LanguageManager.PresentationCulture))
    {
      this.comboBoxLocale.SelectedItem = (object) DisplayOptionsPanel.CurrentSystemCulture.Instance;
    }
    else
    {
      this.comboBoxLocale.SelectedItem = (object) null;
      try
      {
        CultureInfo cultureInfo = CultureInfo.GetCultureInfo(LanguageManager.PresentationCulture);
        if (LanguageManager.IsSupported(LanguageManager.PresentationCulture))
          this.comboBoxLocale.SelectedItem = (object) cultureInfo;
      }
      catch (ArgumentException ex)
      {
      }
    }
    this.UpdatePreview();
    if (ApplicationInformation.CanListByMessage)
      this.comboBoxListBy.SelectedIndex = (int) SapiManager.GlobalInstance.ListBy;
    if (ApplicationInformation.CanViewInstrumentUpdateRate)
      this.checkBoxViewUpdateRate.Checked = SapiManager.GlobalInstance.DisplayUpdateRate;
    base.OnLoad(e);
  }

  public override bool ApplySettings()
  {
    bool flag = true;
    if (this.IsDirty)
    {
      if ((UnitsSystemSelection) this.comboBoxUnitSystem.SelectedItem != this.sapi.UserUnits)
        this.sapi.SetUserUnits((UnitsSystemSelection) this.comboBoxUnitSystem.SelectedItem);
      this.themeProvider.ActiveTheme = (ThemeDefinition) this.comboBoxTheme.SelectedItem;
      this.themeProvider.ActiveTheme.FontScale = this.themePreview.Theme.FontScale;
      if (this.comboBoxLocale.SelectedItem != null)
        flag = !(this.comboBoxLocale.SelectedItem is DisplayOptionsPanel.CurrentSystemCulture) ? this.SetLocale(((CultureInfo) this.comboBoxLocale.SelectedItem).Name) : this.SetLocale(string.Empty);
      if (ApplicationInformation.CanListByMessage)
        SapiManager.GlobalInstance.ListBy = (ListByType) this.comboBoxListBy.SelectedIndex;
      if (ApplicationInformation.CanViewInstrumentUpdateRate)
        SapiManager.GlobalInstance.DisplayUpdateRate = this.checkBoxViewUpdateRate.Checked;
      if (flag)
        flag = base.ApplySettings();
    }
    return flag;
  }

  private bool SetLocale(string locale)
  {
    if (string.Equals(locale, LanguageManager.PresentationCulture) || string.IsNullOrEmpty(locale) && string.IsNullOrEmpty(LanguageManager.PresentationCulture))
      return true;
    bool flag = WarningMessageBox.WarnLanguageChangeNeedsRestart((IWin32Window) this, ApplicationInformation.ProductName);
    if (flag)
      LanguageManager.PresentationCulture = locale;
    return flag;
  }

  private void OnThemeSelectionChanged(object sender, EventArgs e)
  {
    this.MarkDirty();
    this.UpdatePreview();
  }

  private void OnFontSizeChanged(object sender, EventArgs e)
  {
    this.MarkDirty();
    this.UpdatePreview();
  }

  private void OnUnitSystemChanged(object sender, EventArgs e) => this.MarkDirty();

  private void OnLocaleChanged(object sender, EventArgs e)
  {
    this.MarkDirty();
    this.UpdatePreview();
  }

  private void comboBoxListBy_SelectedIndexChanged(object sender, EventArgs e)
  {
    if (this.comboBoxListBy.SelectedIndex == SapiManager.GlobalInstance.ListBy)
      return;
    this.MarkDirty();
  }

  private void checkBoxViewUpdateRate_CheckedChanged(object sender, EventArgs e)
  {
    if (this.checkBoxViewUpdateRate.Checked == SapiManager.GlobalInstance.DisplayUpdateRate)
      return;
    this.MarkDirty();
  }

  private void checkBoxCoalesceFaultCodes_CheckedChanged(object sender, EventArgs e)
  {
    if (this.checkBoxCoalesceFaultCodes.Checked == SapiManager.GlobalInstance.CoalesceFaultCodes)
      return;
    this.MarkDirty();
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (DisplayOptionsPanel));
    this.labelListBy = new Label();
    this.themePreview = new ThemePreview();
    this.comboBoxFontSize = new ComboBox();
    this.comboBoxTheme = new ComboBox();
    this.comboBoxUnitSystem = new ComboBox();
    this.comboBoxLocale = new ComboBox();
    this.comboBoxListBy = new ComboBox();
    this.checkBoxViewUpdateRate = new CheckBox();
    this.tableLayoutPanel = new TableLayoutPanel();
    this.checkBoxCoalesceFaultCodes = new CheckBox();
    Label label1 = new Label();
    Label label2 = new Label();
    Label label3 = new Label();
    Label label4 = new Label();
    this.tableLayoutPanel.SuspendLayout();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) label1, "labelTheme");
    label1.Name = "labelTheme";
    componentResourceManager.ApplyResources((object) label2, "labelFont");
    label2.Name = "labelFont";
    componentResourceManager.ApplyResources((object) label3, "labelUnits");
    label3.Name = "labelUnits";
    componentResourceManager.ApplyResources((object) label4, "labelLocale");
    label4.Name = "labelLocale";
    componentResourceManager.ApplyResources((object) this.labelListBy, "labelListBy");
    this.labelListBy.Name = "labelListBy";
    componentResourceManager.ApplyResources((object) this.themePreview, "themePreview");
    this.tableLayoutPanel.SetColumnSpan((Control) this.themePreview, 4);
    ((Control) this.themePreview).Name = "themePreview";
    this.themePreview.PreviewUICulture = new CultureInfo("en-US");
    componentResourceManager.ApplyResources((object) this.comboBoxFontSize, "comboBoxFontSize");
    this.comboBoxFontSize.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxFontSize.FormattingEnabled = true;
    this.comboBoxFontSize.Name = "comboBoxFontSize";
    this.comboBoxFontSize.SelectedIndexChanged += new EventHandler(this.OnFontSizeChanged);
    componentResourceManager.ApplyResources((object) this.comboBoxTheme, "comboBoxTheme");
    this.comboBoxTheme.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxTheme.FormattingEnabled = true;
    this.comboBoxTheme.Name = "comboBoxTheme";
    this.comboBoxTheme.SelectedIndexChanged += new EventHandler(this.OnThemeSelectionChanged);
    componentResourceManager.ApplyResources((object) this.comboBoxUnitSystem, "comboBoxUnitSystem");
    this.comboBoxUnitSystem.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxUnitSystem.FormattingEnabled = true;
    this.comboBoxUnitSystem.Name = "comboBoxUnitSystem";
    this.comboBoxUnitSystem.SelectedIndexChanged += new EventHandler(this.OnUnitSystemChanged);
    componentResourceManager.ApplyResources((object) this.comboBoxLocale, "comboBoxLocale");
    this.comboBoxLocale.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxLocale.FormattingEnabled = true;
    this.comboBoxLocale.Name = "comboBoxLocale";
    this.comboBoxLocale.Sorted = true;
    this.comboBoxLocale.SelectedIndexChanged += new EventHandler(this.OnLocaleChanged);
    componentResourceManager.ApplyResources((object) this.comboBoxListBy, "comboBoxListBy");
    this.comboBoxListBy.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxListBy.FormattingEnabled = true;
    this.comboBoxListBy.Items.AddRange(new object[2]
    {
      (object) componentResourceManager.GetString("comboBoxListBy.Items"),
      (object) componentResourceManager.GetString("comboBoxListBy.Items1")
    });
    this.comboBoxListBy.Name = "comboBoxListBy";
    this.comboBoxListBy.SelectedIndexChanged += new EventHandler(this.comboBoxListBy_SelectedIndexChanged);
    componentResourceManager.ApplyResources((object) this.checkBoxViewUpdateRate, "checkBoxViewUpdateRate");
    this.tableLayoutPanel.SetColumnSpan((Control) this.checkBoxViewUpdateRate, 2);
    this.checkBoxViewUpdateRate.Name = "checkBoxViewUpdateRate";
    this.checkBoxViewUpdateRate.UseVisualStyleBackColor = true;
    this.checkBoxViewUpdateRate.CheckedChanged += new EventHandler(this.checkBoxViewUpdateRate_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    this.tableLayoutPanel.Controls.Add((Control) this.checkBoxCoalesceFaultCodes, 1, 3);
    this.tableLayoutPanel.Controls.Add((Control) this.themePreview, 0, 0);
    this.tableLayoutPanel.Controls.Add((Control) this.checkBoxViewUpdateRate, 2, 2);
    this.tableLayoutPanel.Controls.Add((Control) label1, 0, 1);
    this.tableLayoutPanel.Controls.Add((Control) this.comboBoxListBy, 3, 1);
    this.tableLayoutPanel.Controls.Add((Control) label2, 0, 2);
    this.tableLayoutPanel.Controls.Add((Control) this.labelListBy, 2, 1);
    this.tableLayoutPanel.Controls.Add((Control) label3, 0, 3);
    this.tableLayoutPanel.Controls.Add((Control) this.comboBoxLocale, 1, 4);
    this.tableLayoutPanel.Controls.Add((Control) this.comboBoxFontSize, 1, 2);
    this.tableLayoutPanel.Controls.Add((Control) this.comboBoxUnitSystem, 1, 3);
    this.tableLayoutPanel.Controls.Add((Control) label4, 0, 4);
    this.tableLayoutPanel.Controls.Add((Control) this.comboBoxTheme, 1, 1);
    this.tableLayoutPanel.Name = "tableLayoutPanel";
    componentResourceManager.ApplyResources((object) this.checkBoxCoalesceFaultCodes, "checkBoxCoalesceFaultCodes");
    this.tableLayoutPanel.SetColumnSpan((Control) this.checkBoxCoalesceFaultCodes, 2);
    this.tableLayoutPanel.SetRowSpan((Control) this.checkBoxCoalesceFaultCodes, 2);
    this.checkBoxCoalesceFaultCodes.Name = "checkBoxCoalesceFaultCodes";
    this.checkBoxCoalesceFaultCodes.UseVisualStyleBackColor = true;
    this.checkBoxCoalesceFaultCodes.CheckedChanged += new EventHandler(this.checkBoxCoalesceFaultCodes_CheckedChanged);
    componentResourceManager.ApplyResources((object) this, "$this");
    this.Controls.Add((Control) this.tableLayoutPanel);
    this.Name = nameof (DisplayOptionsPanel);
    this.Controls.SetChildIndex((Control) this.tableLayoutPanel, 0);
    this.tableLayoutPanel.ResumeLayout(false);
    this.tableLayoutPanel.PerformLayout();
    this.ResumeLayout(false);
  }

  private class CurrentSystemCulture
  {
    private static readonly DisplayOptionsPanel.CurrentSystemCulture instance = new DisplayOptionsPanel.CurrentSystemCulture();

    private CurrentSystemCulture()
    {
    }

    public string DisplayName => Resources.UseSystemLanguage;

    public static DisplayOptionsPanel.CurrentSystemCulture Instance
    {
      get => DisplayOptionsPanel.CurrentSystemCulture.instance;
    }
  }
}
