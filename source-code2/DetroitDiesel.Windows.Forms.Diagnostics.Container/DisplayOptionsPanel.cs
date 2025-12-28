using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using DetroitDiesel.Windows.Forms.Themed;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal class DisplayOptionsPanel : OptionsPanel
{
	private class CurrentSystemCulture
	{
		private static readonly CurrentSystemCulture instance = new CurrentSystemCulture();

		public string DisplayName => Resources.UseSystemLanguage;

		public static CurrentSystemCulture Instance => instance;

		private CurrentSystemCulture()
		{
		}
	}

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
		base.MinAccessLevel = 1;
		InitializeComponent();
		base.HeaderImage = new Bitmap(GetType(), "option_theme.png");
		PopulateThemesCombo();
		PopulateFontSizeCombo();
		PopulateUnitsCombo();
		PopulateLocaleCombo();
		if (!ApplicationInformation.CanListByMessage)
		{
			comboBoxListBy.Visible = false;
			labelListBy.Visible = false;
		}
		if (!ApplicationInformation.CanViewInstrumentUpdateRate)
		{
			checkBoxViewUpdateRate.Visible = false;
		}
		checkBoxCoalesceFaultCodes.Visible = false;
	}

	private void PopulateUnitsCombo()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		foreach (UnitsSystemSelection unitChoice in SapiManager.UnitChoices)
		{
			comboBoxUnitSystem.Items.Add(unitChoice);
		}
	}

	private void PopulateThemesCombo()
	{
		foreach (ThemeDefinition themeDefinition in themeProvider.ThemeDefinitions)
		{
			comboBoxTheme.Items.Add(themeDefinition);
		}
	}

	private void PopulateFontSizeCombo()
	{
		comboBoxFontSize.Items.Add("50%");
		comboBoxFontSize.Items.Add("75%");
		comboBoxFontSize.Items.Add("100%");
		comboBoxFontSize.Items.Add("150%");
		comboBoxFontSize.Items.Add("200%");
	}

	private void PopulateLocaleCombo()
	{
		comboBoxLocale.DisplayMember = "DisplayName";
		comboBoxLocale.Items.Add(CurrentSystemCulture.Instance);
		foreach (CultureInfo item in LanguageManager.EnumerateSupportedCultures())
		{
			comboBoxLocale.Items.Add(item);
		}
	}

	private void UpdatePreview()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		if (comboBoxTheme.SelectedItem != null)
		{
			themePreview.Theme = new ThemeDefinition((ThemeDefinition)comboBoxTheme.SelectedItem);
		}
		try
		{
			if (comboBoxFontSize.SelectedItem != null)
			{
				string text = comboBoxFontSize.SelectedItem.ToString();
				text = text.Replace("%", "");
				themePreview.Theme.FontScale = float.Parse(text, CultureInfo.CurrentCulture) / 100f;
			}
		}
		catch (FormatException ex)
		{
			MessageBox.Show(ex.Message, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, ControlHelpers.IsRightToLeft((Control)this) ? (MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading) : ((MessageBoxOptions)0));
		}
		if (comboBoxLocale.SelectedItem != null)
		{
			if (comboBoxLocale.SelectedItem is CurrentSystemCulture)
			{
				themePreview.PreviewUICulture = CultureInfo.CurrentUICulture;
			}
			else
			{
				themePreview.PreviewUICulture = (CultureInfo)comboBoxLocale.SelectedItem;
			}
		}
	}

	protected override void OnLoad(EventArgs e)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected I4, but got Unknown
		comboBoxTheme.SelectedItem = themeProvider.ActiveTheme;
		comboBoxFontSize.SelectedItem = string.Format(CultureInfo.CurrentCulture, Resources.FormatPercentage, themeProvider.ActiveTheme.FontScale * 100.0);
		comboBoxUnitSystem.SelectedItem = sapi.UserUnits;
		if (string.IsNullOrEmpty(LanguageManager.PresentationCulture))
		{
			comboBoxLocale.SelectedItem = CurrentSystemCulture.Instance;
		}
		else
		{
			comboBoxLocale.SelectedItem = null;
			try
			{
				CultureInfo cultureInfo = CultureInfo.GetCultureInfo(LanguageManager.PresentationCulture);
				if (LanguageManager.IsSupported(LanguageManager.PresentationCulture))
				{
					comboBoxLocale.SelectedItem = cultureInfo;
				}
			}
			catch (ArgumentException)
			{
			}
		}
		UpdatePreview();
		if (ApplicationInformation.CanListByMessage)
		{
			comboBoxListBy.SelectedIndex = (int)SapiManager.GlobalInstance.ListBy;
		}
		if (ApplicationInformation.CanViewInstrumentUpdateRate)
		{
			checkBoxViewUpdateRate.Checked = SapiManager.GlobalInstance.DisplayUpdateRate;
		}
		base.OnLoad(e);
	}

	public override bool ApplySettings()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		bool flag = true;
		if (base.IsDirty)
		{
			if ((UnitsSystemSelection)comboBoxUnitSystem.SelectedItem != sapi.UserUnits)
			{
				sapi.SetUserUnits((UnitsSystemSelection)comboBoxUnitSystem.SelectedItem);
			}
			themeProvider.ActiveTheme = (ThemeDefinition)comboBoxTheme.SelectedItem;
			themeProvider.ActiveTheme.FontScale = themePreview.Theme.FontScale;
			if (comboBoxLocale.SelectedItem != null)
			{
				flag = ((!(comboBoxLocale.SelectedItem is CurrentSystemCulture)) ? SetLocale(((CultureInfo)comboBoxLocale.SelectedItem).Name) : SetLocale(string.Empty));
			}
			if (ApplicationInformation.CanListByMessage)
			{
				SapiManager.GlobalInstance.ListBy = (ListByType)comboBoxListBy.SelectedIndex;
			}
			if (ApplicationInformation.CanViewInstrumentUpdateRate)
			{
				SapiManager.GlobalInstance.DisplayUpdateRate = checkBoxViewUpdateRate.Checked;
			}
			if (flag)
			{
				flag = base.ApplySettings();
			}
		}
		return flag;
	}

	private bool SetLocale(string locale)
	{
		if (string.Equals(locale, LanguageManager.PresentationCulture) || (string.IsNullOrEmpty(locale) && string.IsNullOrEmpty(LanguageManager.PresentationCulture)))
		{
			return true;
		}
		bool flag = WarningMessageBox.WarnLanguageChangeNeedsRestart((IWin32Window)this, ApplicationInformation.ProductName);
		if (flag)
		{
			LanguageManager.PresentationCulture = locale;
		}
		return flag;
	}

	private void OnThemeSelectionChanged(object sender, EventArgs e)
	{
		MarkDirty();
		UpdatePreview();
	}

	private void OnFontSizeChanged(object sender, EventArgs e)
	{
		MarkDirty();
		UpdatePreview();
	}

	private void OnUnitSystemChanged(object sender, EventArgs e)
	{
		MarkDirty();
	}

	private void OnLocaleChanged(object sender, EventArgs e)
	{
		MarkDirty();
		UpdatePreview();
	}

	private void comboBoxListBy_SelectedIndexChanged(object sender, EventArgs e)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between I4 and Unknown
		if (comboBoxListBy.SelectedIndex != (int)SapiManager.GlobalInstance.ListBy)
		{
			MarkDirty();
		}
	}

	private void checkBoxViewUpdateRate_CheckedChanged(object sender, EventArgs e)
	{
		if (checkBoxViewUpdateRate.Checked != SapiManager.GlobalInstance.DisplayUpdateRate)
		{
			MarkDirty();
		}
	}

	private void checkBoxCoalesceFaultCodes_CheckedChanged(object sender, EventArgs e)
	{
		if (checkBoxCoalesceFaultCodes.Checked != SapiManager.GlobalInstance.CoalesceFaultCodes)
		{
			MarkDirty();
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Container.DisplayOptionsPanel));
		this.labelListBy = new System.Windows.Forms.Label();
		this.themePreview = new ThemePreview();
		this.comboBoxFontSize = new System.Windows.Forms.ComboBox();
		this.comboBoxTheme = new System.Windows.Forms.ComboBox();
		this.comboBoxUnitSystem = new System.Windows.Forms.ComboBox();
		this.comboBoxLocale = new System.Windows.Forms.ComboBox();
		this.comboBoxListBy = new System.Windows.Forms.ComboBox();
		this.checkBoxViewUpdateRate = new System.Windows.Forms.CheckBox();
		this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
		this.checkBoxCoalesceFaultCodes = new System.Windows.Forms.CheckBox();
		System.Windows.Forms.Label label = new System.Windows.Forms.Label();
		System.Windows.Forms.Label label2 = new System.Windows.Forms.Label();
		System.Windows.Forms.Label label3 = new System.Windows.Forms.Label();
		System.Windows.Forms.Label label4 = new System.Windows.Forms.Label();
		this.tableLayoutPanel.SuspendLayout();
		base.SuspendLayout();
		resources.ApplyResources(label, "labelTheme");
		label.Name = "labelTheme";
		resources.ApplyResources(label2, "labelFont");
		label2.Name = "labelFont";
		resources.ApplyResources(label3, "labelUnits");
		label3.Name = "labelUnits";
		resources.ApplyResources(label4, "labelLocale");
		label4.Name = "labelLocale";
		resources.ApplyResources(this.labelListBy, "labelListBy");
		this.labelListBy.Name = "labelListBy";
		resources.ApplyResources(this.themePreview, "themePreview");
		this.tableLayoutPanel.SetColumnSpan((System.Windows.Forms.Control)(object)this.themePreview, 4);
		((System.Windows.Forms.Control)(object)this.themePreview).Name = "themePreview";
		this.themePreview.PreviewUICulture = new System.Globalization.CultureInfo("en-US");
		resources.ApplyResources(this.comboBoxFontSize, "comboBoxFontSize");
		this.comboBoxFontSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxFontSize.FormattingEnabled = true;
		this.comboBoxFontSize.Name = "comboBoxFontSize";
		this.comboBoxFontSize.SelectedIndexChanged += new System.EventHandler(OnFontSizeChanged);
		resources.ApplyResources(this.comboBoxTheme, "comboBoxTheme");
		this.comboBoxTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxTheme.FormattingEnabled = true;
		this.comboBoxTheme.Name = "comboBoxTheme";
		this.comboBoxTheme.SelectedIndexChanged += new System.EventHandler(OnThemeSelectionChanged);
		resources.ApplyResources(this.comboBoxUnitSystem, "comboBoxUnitSystem");
		this.comboBoxUnitSystem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxUnitSystem.FormattingEnabled = true;
		this.comboBoxUnitSystem.Name = "comboBoxUnitSystem";
		this.comboBoxUnitSystem.SelectedIndexChanged += new System.EventHandler(OnUnitSystemChanged);
		resources.ApplyResources(this.comboBoxLocale, "comboBoxLocale");
		this.comboBoxLocale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxLocale.FormattingEnabled = true;
		this.comboBoxLocale.Name = "comboBoxLocale";
		this.comboBoxLocale.Sorted = true;
		this.comboBoxLocale.SelectedIndexChanged += new System.EventHandler(OnLocaleChanged);
		resources.ApplyResources(this.comboBoxListBy, "comboBoxListBy");
		this.comboBoxListBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxListBy.FormattingEnabled = true;
		this.comboBoxListBy.Items.AddRange(new object[2]
		{
			resources.GetString("comboBoxListBy.Items"),
			resources.GetString("comboBoxListBy.Items1")
		});
		this.comboBoxListBy.Name = "comboBoxListBy";
		this.comboBoxListBy.SelectedIndexChanged += new System.EventHandler(comboBoxListBy_SelectedIndexChanged);
		resources.ApplyResources(this.checkBoxViewUpdateRate, "checkBoxViewUpdateRate");
		this.tableLayoutPanel.SetColumnSpan(this.checkBoxViewUpdateRate, 2);
		this.checkBoxViewUpdateRate.Name = "checkBoxViewUpdateRate";
		this.checkBoxViewUpdateRate.UseVisualStyleBackColor = true;
		this.checkBoxViewUpdateRate.CheckedChanged += new System.EventHandler(checkBoxViewUpdateRate_CheckedChanged);
		resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
		this.tableLayoutPanel.Controls.Add(this.checkBoxCoalesceFaultCodes, 1, 3);
		this.tableLayoutPanel.Controls.Add((System.Windows.Forms.Control)(object)this.themePreview, 0, 0);
		this.tableLayoutPanel.Controls.Add(this.checkBoxViewUpdateRate, 2, 2);
		this.tableLayoutPanel.Controls.Add(label, 0, 1);
		this.tableLayoutPanel.Controls.Add(this.comboBoxListBy, 3, 1);
		this.tableLayoutPanel.Controls.Add(label2, 0, 2);
		this.tableLayoutPanel.Controls.Add(this.labelListBy, 2, 1);
		this.tableLayoutPanel.Controls.Add(label3, 0, 3);
		this.tableLayoutPanel.Controls.Add(this.comboBoxLocale, 1, 4);
		this.tableLayoutPanel.Controls.Add(this.comboBoxFontSize, 1, 2);
		this.tableLayoutPanel.Controls.Add(this.comboBoxUnitSystem, 1, 3);
		this.tableLayoutPanel.Controls.Add(label4, 0, 4);
		this.tableLayoutPanel.Controls.Add(this.comboBoxTheme, 1, 1);
		this.tableLayoutPanel.Name = "tableLayoutPanel";
		resources.ApplyResources(this.checkBoxCoalesceFaultCodes, "checkBoxCoalesceFaultCodes");
		this.tableLayoutPanel.SetColumnSpan(this.checkBoxCoalesceFaultCodes, 2);
		this.tableLayoutPanel.SetRowSpan(this.checkBoxCoalesceFaultCodes, 2);
		this.checkBoxCoalesceFaultCodes.Name = "checkBoxCoalesceFaultCodes";
		this.checkBoxCoalesceFaultCodes.UseVisualStyleBackColor = true;
		this.checkBoxCoalesceFaultCodes.CheckedChanged += new System.EventHandler(checkBoxCoalesceFaultCodes_CheckedChanged);
		resources.ApplyResources(this, "$this");
		base.Controls.Add(this.tableLayoutPanel);
		base.Name = "DisplayOptionsPanel";
		base.Controls.SetChildIndex(this.tableLayoutPanel, 0);
		this.tableLayoutPanel.ResumeLayout(false);
		this.tableLayoutPanel.PerformLayout();
		base.ResumeLayout(false);
	}
}
