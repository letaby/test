using System;
using System.ComponentModel;
using System.Windows.Forms;
using DetroitDiesel.DataHub;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal class FleetInfoOptionsPanel : OptionsPanel
{
	private FleetInformation fleetInformation;

	private IContainer components;

	private TextBox textBoxCompanyName;

	private TextBox textBoxAddress;

	private TextBox textBoxCity;

	private MaskedTextBox maskedTextBoxZipCode;

	private MaskedTextBox maskedTextBoxTelephoneNumber;

	private ComboBox comboBoxTimeZone;

	private TextBox textBoxState;

	private CheckBox checkBoxAutoDataPageExtraction;

	public FleetInfoOptionsPanel()
	{
		base.MinAccessLevel = 1;
		InitializeComponent();
		base.HeaderImage = Resources.FleetInfoHeaderImage;
		textBoxCompanyName.MaxLength = 30;
		textBoxAddress.MaxLength = 30;
		textBoxCity.MaxLength = 30;
		textBoxState.MaxLength = 2;
		foreach (FleetTimeZone timeZone in FleetInformation.TimeZones)
		{
			comboBoxTimeZone.Items.Add(timeZone);
		}
	}

	private void OnFleetTimeZoneChanged(object sender, EventArgs e)
	{
		MarkDirty();
	}

	private void OnModified(object sender, EventArgs e)
	{
		MarkDirty();
	}

	private void checkBoxAutoDataPageExtraction_CheckedChanged(object sender, EventArgs e)
	{
		MarkDirty();
	}

	protected override void OnLoad(EventArgs e)
	{
		fleetInformation = FleetInformation.Load();
		textBoxCompanyName.Text = fleetInformation.CompanyName;
		textBoxAddress.Text = fleetInformation.Address;
		textBoxCity.Text = fleetInformation.City;
		textBoxState.Text = fleetInformation.State;
		maskedTextBoxZipCode.Text = fleetInformation.ZipCode;
		maskedTextBoxTelephoneNumber.Text = fleetInformation.TelephoneNumber;
		comboBoxTimeZone.SelectedItem = fleetInformation.TimeZone;
		checkBoxAutoDataPageExtraction.Checked = ExtractionManager.GlobalInstance.AutoExtractDataPagesAtConnection;
		base.OnLoad(e);
	}

	public override bool ApplySettings()
	{
		fleetInformation.CompanyName = textBoxCompanyName.Text;
		fleetInformation.Address = textBoxAddress.Text;
		fleetInformation.City = textBoxCity.Text;
		fleetInformation.State = textBoxState.Text;
		fleetInformation.TelephoneNumber = maskedTextBoxTelephoneNumber.Text;
		fleetInformation.ZipCode = maskedTextBoxZipCode.Text;
		if (comboBoxTimeZone.SelectedItem != null)
		{
			fleetInformation.TimeZone = (FleetTimeZone)comboBoxTimeZone.SelectedItem;
		}
		ExtractionManager.GlobalInstance.AutoExtractDataPagesAtConnection = checkBoxAutoDataPageExtraction.Checked;
		FleetInformation.Save(fleetInformation);
		return base.ApplySettings();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Container.FleetInfoOptionsPanel));
		this.textBoxCompanyName = new System.Windows.Forms.TextBox();
		this.textBoxAddress = new System.Windows.Forms.TextBox();
		this.textBoxCity = new System.Windows.Forms.TextBox();
		this.maskedTextBoxZipCode = new System.Windows.Forms.MaskedTextBox();
		this.maskedTextBoxTelephoneNumber = new System.Windows.Forms.MaskedTextBox();
		this.comboBoxTimeZone = new System.Windows.Forms.ComboBox();
		this.textBoxState = new System.Windows.Forms.TextBox();
		this.checkBoxAutoDataPageExtraction = new System.Windows.Forms.CheckBox();
		System.Windows.Forms.TableLayoutPanel tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
		System.Windows.Forms.Label label = new System.Windows.Forms.Label();
		System.Windows.Forms.Label label2 = new System.Windows.Forms.Label();
		System.Windows.Forms.Label label3 = new System.Windows.Forms.Label();
		System.Windows.Forms.Label label4 = new System.Windows.Forms.Label();
		System.Windows.Forms.Label label5 = new System.Windows.Forms.Label();
		System.Windows.Forms.Label label6 = new System.Windows.Forms.Label();
		System.Windows.Forms.Label label7 = new System.Windows.Forms.Label();
		tableLayoutPanel.SuspendLayout();
		base.SuspendLayout();
		resources.ApplyResources(tableLayoutPanel, "fleetInformationTable");
		tableLayoutPanel.Controls.Add(label, 0, 0);
		tableLayoutPanel.Controls.Add(label2, 0, 1);
		tableLayoutPanel.Controls.Add(this.textBoxCompanyName, 1, 0);
		tableLayoutPanel.Controls.Add(this.textBoxAddress, 1, 1);
		tableLayoutPanel.Controls.Add(label3, 0, 2);
		tableLayoutPanel.Controls.Add(this.textBoxCity, 1, 2);
		tableLayoutPanel.Controls.Add(label4, 0, 3);
		tableLayoutPanel.Controls.Add(label5, 2, 2);
		tableLayoutPanel.Controls.Add(this.maskedTextBoxZipCode, 1, 3);
		tableLayoutPanel.Controls.Add(label6, 0, 4);
		tableLayoutPanel.Controls.Add(this.maskedTextBoxTelephoneNumber, 1, 4);
		tableLayoutPanel.Controls.Add(label7, 0, 6);
		tableLayoutPanel.Controls.Add(this.comboBoxTimeZone, 1, 6);
		tableLayoutPanel.Controls.Add(this.textBoxState, 3, 2);
		tableLayoutPanel.Controls.Add(this.checkBoxAutoDataPageExtraction, 0, 8);
		tableLayoutPanel.Name = "fleetInformationTable";
		resources.ApplyResources(label, "labelCompanyName");
		label.Name = "labelCompanyName";
		resources.ApplyResources(label2, "labelAddress");
		label2.Name = "labelAddress";
		resources.ApplyResources(this.textBoxCompanyName, "textBoxCompanyName");
		this.textBoxCompanyName.Name = "textBoxCompanyName";
		this.textBoxCompanyName.ModifiedChanged += new System.EventHandler(OnModified);
		resources.ApplyResources(this.textBoxAddress, "textBoxAddress");
		this.textBoxAddress.Name = "textBoxAddress";
		this.textBoxAddress.ModifiedChanged += new System.EventHandler(OnModified);
		resources.ApplyResources(label3, "labelCity");
		label3.Name = "labelCity";
		resources.ApplyResources(this.textBoxCity, "textBoxCity");
		this.textBoxCity.Name = "textBoxCity";
		this.textBoxCity.ModifiedChanged += new System.EventHandler(OnModified);
		resources.ApplyResources(label4, "labelZipCode");
		label4.Name = "labelZipCode";
		resources.ApplyResources(label5, "labelState");
		label5.Name = "labelState";
		this.maskedTextBoxZipCode.AllowPromptAsInput = false;
		this.maskedTextBoxZipCode.HidePromptOnLeave = true;
		resources.ApplyResources(this.maskedTextBoxZipCode, "maskedTextBoxZipCode");
		this.maskedTextBoxZipCode.Name = "maskedTextBoxZipCode";
		this.maskedTextBoxZipCode.ModifiedChanged += new System.EventHandler(OnModified);
		resources.ApplyResources(label6, "labelPhoneNumber");
		label6.Name = "labelPhoneNumber";
		this.maskedTextBoxTelephoneNumber.AllowPromptAsInput = false;
		this.maskedTextBoxTelephoneNumber.HidePromptOnLeave = true;
		resources.ApplyResources(this.maskedTextBoxTelephoneNumber, "maskedTextBoxTelephoneNumber");
		this.maskedTextBoxTelephoneNumber.Name = "maskedTextBoxTelephoneNumber";
		this.maskedTextBoxTelephoneNumber.ModifiedChanged += new System.EventHandler(OnModified);
		resources.ApplyResources(label7, "labelFleetTimeZone");
		label7.Name = "labelFleetTimeZone";
		this.comboBoxTimeZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxTimeZone.FormattingEnabled = true;
		resources.ApplyResources(this.comboBoxTimeZone, "comboBoxTimeZone");
		this.comboBoxTimeZone.Name = "comboBoxTimeZone";
		this.comboBoxTimeZone.SelectedIndexChanged += new System.EventHandler(OnFleetTimeZoneChanged);
		this.textBoxState.AutoCompleteCustomSource.AddRange(new string[51]
		{
			resources.GetString("textBoxState.AutoCompleteCustomSource"),
			resources.GetString("textBoxState.AutoCompleteCustomSource1"),
			resources.GetString("textBoxState.AutoCompleteCustomSource2"),
			resources.GetString("textBoxState.AutoCompleteCustomSource3"),
			resources.GetString("textBoxState.AutoCompleteCustomSource4"),
			resources.GetString("textBoxState.AutoCompleteCustomSource5"),
			resources.GetString("textBoxState.AutoCompleteCustomSource6"),
			resources.GetString("textBoxState.AutoCompleteCustomSource7"),
			resources.GetString("textBoxState.AutoCompleteCustomSource8"),
			resources.GetString("textBoxState.AutoCompleteCustomSource9"),
			resources.GetString("textBoxState.AutoCompleteCustomSource10"),
			resources.GetString("textBoxState.AutoCompleteCustomSource11"),
			resources.GetString("textBoxState.AutoCompleteCustomSource12"),
			resources.GetString("textBoxState.AutoCompleteCustomSource13"),
			resources.GetString("textBoxState.AutoCompleteCustomSource14"),
			resources.GetString("textBoxState.AutoCompleteCustomSource15"),
			resources.GetString("textBoxState.AutoCompleteCustomSource16"),
			resources.GetString("textBoxState.AutoCompleteCustomSource17"),
			resources.GetString("textBoxState.AutoCompleteCustomSource18"),
			resources.GetString("textBoxState.AutoCompleteCustomSource19"),
			resources.GetString("textBoxState.AutoCompleteCustomSource20"),
			resources.GetString("textBoxState.AutoCompleteCustomSource21"),
			resources.GetString("textBoxState.AutoCompleteCustomSource22"),
			resources.GetString("textBoxState.AutoCompleteCustomSource23"),
			resources.GetString("textBoxState.AutoCompleteCustomSource24"),
			resources.GetString("textBoxState.AutoCompleteCustomSource25"),
			resources.GetString("textBoxState.AutoCompleteCustomSource26"),
			resources.GetString("textBoxState.AutoCompleteCustomSource27"),
			resources.GetString("textBoxState.AutoCompleteCustomSource28"),
			resources.GetString("textBoxState.AutoCompleteCustomSource29"),
			resources.GetString("textBoxState.AutoCompleteCustomSource30"),
			resources.GetString("textBoxState.AutoCompleteCustomSource31"),
			resources.GetString("textBoxState.AutoCompleteCustomSource32"),
			resources.GetString("textBoxState.AutoCompleteCustomSource33"),
			resources.GetString("textBoxState.AutoCompleteCustomSource34"),
			resources.GetString("textBoxState.AutoCompleteCustomSource35"),
			resources.GetString("textBoxState.AutoCompleteCustomSource36"),
			resources.GetString("textBoxState.AutoCompleteCustomSource37"),
			resources.GetString("textBoxState.AutoCompleteCustomSource38"),
			resources.GetString("textBoxState.AutoCompleteCustomSource39"),
			resources.GetString("textBoxState.AutoCompleteCustomSource40"),
			resources.GetString("textBoxState.AutoCompleteCustomSource41"),
			resources.GetString("textBoxState.AutoCompleteCustomSource42"),
			resources.GetString("textBoxState.AutoCompleteCustomSource43"),
			resources.GetString("textBoxState.AutoCompleteCustomSource44"),
			resources.GetString("textBoxState.AutoCompleteCustomSource45"),
			resources.GetString("textBoxState.AutoCompleteCustomSource46"),
			resources.GetString("textBoxState.AutoCompleteCustomSource47"),
			resources.GetString("textBoxState.AutoCompleteCustomSource48"),
			resources.GetString("textBoxState.AutoCompleteCustomSource49"),
			resources.GetString("textBoxState.AutoCompleteCustomSource50")
		});
		this.textBoxState.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
		this.textBoxState.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
		this.textBoxState.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		resources.ApplyResources(this.textBoxState, "textBoxState");
		this.textBoxState.Name = "textBoxState";
		this.textBoxState.ModifiedChanged += new System.EventHandler(OnModified);
		resources.ApplyResources(this.checkBoxAutoDataPageExtraction, "checkBoxAutoDataPageExtraction");
		tableLayoutPanel.SetColumnSpan(this.checkBoxAutoDataPageExtraction, 4);
		this.checkBoxAutoDataPageExtraction.Name = "checkBoxAutoDataPageExtraction";
		this.checkBoxAutoDataPageExtraction.UseVisualStyleBackColor = true;
		this.checkBoxAutoDataPageExtraction.CheckedChanged += new System.EventHandler(checkBoxAutoDataPageExtraction_CheckedChanged);
		resources.ApplyResources(this, "$this");
		base.Controls.Add(tableLayoutPanel);
		base.Name = "FleetInfoOptionsPanel";
		base.Controls.SetChildIndex(tableLayoutPanel, 0);
		tableLayoutPanel.ResumeLayout(false);
		tableLayoutPanel.PerformLayout();
		base.ResumeLayout(false);
	}
}
