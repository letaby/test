// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.FleetInfoOptionsPanel
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.DataHub;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
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
    this.MinAccessLevel = 1;
    this.InitializeComponent();
    this.HeaderImage = (Image) Resources.FleetInfoHeaderImage;
    this.textBoxCompanyName.MaxLength = 30;
    this.textBoxAddress.MaxLength = 30;
    this.textBoxCity.MaxLength = 30;
    this.textBoxState.MaxLength = 2;
    foreach (object timeZone in (IEnumerable<FleetTimeZone>) FleetInformation.TimeZones)
      this.comboBoxTimeZone.Items.Add(timeZone);
  }

  private void OnFleetTimeZoneChanged(object sender, EventArgs e) => this.MarkDirty();

  private void OnModified(object sender, EventArgs e) => this.MarkDirty();

  private void checkBoxAutoDataPageExtraction_CheckedChanged(object sender, EventArgs e)
  {
    this.MarkDirty();
  }

  protected override void OnLoad(EventArgs e)
  {
    this.fleetInformation = FleetInformation.Load();
    this.textBoxCompanyName.Text = this.fleetInformation.CompanyName;
    this.textBoxAddress.Text = this.fleetInformation.Address;
    this.textBoxCity.Text = this.fleetInformation.City;
    this.textBoxState.Text = this.fleetInformation.State;
    this.maskedTextBoxZipCode.Text = this.fleetInformation.ZipCode;
    this.maskedTextBoxTelephoneNumber.Text = this.fleetInformation.TelephoneNumber;
    this.comboBoxTimeZone.SelectedItem = (object) this.fleetInformation.TimeZone;
    this.checkBoxAutoDataPageExtraction.Checked = ExtractionManager.GlobalInstance.AutoExtractDataPagesAtConnection;
    base.OnLoad(e);
  }

  public override bool ApplySettings()
  {
    this.fleetInformation.CompanyName = this.textBoxCompanyName.Text;
    this.fleetInformation.Address = this.textBoxAddress.Text;
    this.fleetInformation.City = this.textBoxCity.Text;
    this.fleetInformation.State = this.textBoxState.Text;
    this.fleetInformation.TelephoneNumber = this.maskedTextBoxTelephoneNumber.Text;
    this.fleetInformation.ZipCode = this.maskedTextBoxZipCode.Text;
    if (this.comboBoxTimeZone.SelectedItem != null)
      this.fleetInformation.TimeZone = (FleetTimeZone) this.comboBoxTimeZone.SelectedItem;
    ExtractionManager.GlobalInstance.AutoExtractDataPagesAtConnection = this.checkBoxAutoDataPageExtraction.Checked;
    FleetInformation.Save(this.fleetInformation);
    return base.ApplySettings();
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (FleetInfoOptionsPanel));
    this.textBoxCompanyName = new TextBox();
    this.textBoxAddress = new TextBox();
    this.textBoxCity = new TextBox();
    this.maskedTextBoxZipCode = new MaskedTextBox();
    this.maskedTextBoxTelephoneNumber = new MaskedTextBox();
    this.comboBoxTimeZone = new ComboBox();
    this.textBoxState = new TextBox();
    this.checkBoxAutoDataPageExtraction = new CheckBox();
    TableLayoutPanel child = new TableLayoutPanel();
    Label label1 = new Label();
    Label label2 = new Label();
    Label label3 = new Label();
    Label label4 = new Label();
    Label label5 = new Label();
    Label label6 = new Label();
    Label label7 = new Label();
    child.SuspendLayout();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) child, "fleetInformationTable");
    child.Controls.Add((Control) label1, 0, 0);
    child.Controls.Add((Control) label2, 0, 1);
    child.Controls.Add((Control) this.textBoxCompanyName, 1, 0);
    child.Controls.Add((Control) this.textBoxAddress, 1, 1);
    child.Controls.Add((Control) label3, 0, 2);
    child.Controls.Add((Control) this.textBoxCity, 1, 2);
    child.Controls.Add((Control) label4, 0, 3);
    child.Controls.Add((Control) label5, 2, 2);
    child.Controls.Add((Control) this.maskedTextBoxZipCode, 1, 3);
    child.Controls.Add((Control) label6, 0, 4);
    child.Controls.Add((Control) this.maskedTextBoxTelephoneNumber, 1, 4);
    child.Controls.Add((Control) label7, 0, 6);
    child.Controls.Add((Control) this.comboBoxTimeZone, 1, 6);
    child.Controls.Add((Control) this.textBoxState, 3, 2);
    child.Controls.Add((Control) this.checkBoxAutoDataPageExtraction, 0, 8);
    child.Name = "fleetInformationTable";
    componentResourceManager.ApplyResources((object) label1, "labelCompanyName");
    label1.Name = "labelCompanyName";
    componentResourceManager.ApplyResources((object) label2, "labelAddress");
    label2.Name = "labelAddress";
    componentResourceManager.ApplyResources((object) this.textBoxCompanyName, "textBoxCompanyName");
    this.textBoxCompanyName.Name = "textBoxCompanyName";
    this.textBoxCompanyName.ModifiedChanged += new EventHandler(this.OnModified);
    componentResourceManager.ApplyResources((object) this.textBoxAddress, "textBoxAddress");
    this.textBoxAddress.Name = "textBoxAddress";
    this.textBoxAddress.ModifiedChanged += new EventHandler(this.OnModified);
    componentResourceManager.ApplyResources((object) label3, "labelCity");
    label3.Name = "labelCity";
    componentResourceManager.ApplyResources((object) this.textBoxCity, "textBoxCity");
    this.textBoxCity.Name = "textBoxCity";
    this.textBoxCity.ModifiedChanged += new EventHandler(this.OnModified);
    componentResourceManager.ApplyResources((object) label4, "labelZipCode");
    label4.Name = "labelZipCode";
    componentResourceManager.ApplyResources((object) label5, "labelState");
    label5.Name = "labelState";
    this.maskedTextBoxZipCode.AllowPromptAsInput = false;
    this.maskedTextBoxZipCode.HidePromptOnLeave = true;
    componentResourceManager.ApplyResources((object) this.maskedTextBoxZipCode, "maskedTextBoxZipCode");
    this.maskedTextBoxZipCode.Name = "maskedTextBoxZipCode";
    this.maskedTextBoxZipCode.ModifiedChanged += new EventHandler(this.OnModified);
    componentResourceManager.ApplyResources((object) label6, "labelPhoneNumber");
    label6.Name = "labelPhoneNumber";
    this.maskedTextBoxTelephoneNumber.AllowPromptAsInput = false;
    this.maskedTextBoxTelephoneNumber.HidePromptOnLeave = true;
    componentResourceManager.ApplyResources((object) this.maskedTextBoxTelephoneNumber, "maskedTextBoxTelephoneNumber");
    this.maskedTextBoxTelephoneNumber.Name = "maskedTextBoxTelephoneNumber";
    this.maskedTextBoxTelephoneNumber.ModifiedChanged += new EventHandler(this.OnModified);
    componentResourceManager.ApplyResources((object) label7, "labelFleetTimeZone");
    label7.Name = "labelFleetTimeZone";
    this.comboBoxTimeZone.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxTimeZone.FormattingEnabled = true;
    componentResourceManager.ApplyResources((object) this.comboBoxTimeZone, "comboBoxTimeZone");
    this.comboBoxTimeZone.Name = "comboBoxTimeZone";
    this.comboBoxTimeZone.SelectedIndexChanged += new EventHandler(this.OnFleetTimeZoneChanged);
    this.textBoxState.AutoCompleteCustomSource.AddRange(new string[51]
    {
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource1"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource2"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource3"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource4"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource5"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource6"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource7"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource8"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource9"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource10"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource11"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource12"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource13"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource14"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource15"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource16"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource17"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource18"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource19"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource20"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource21"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource22"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource23"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource24"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource25"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource26"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource27"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource28"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource29"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource30"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource31"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource32"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource33"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource34"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource35"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource36"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource37"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource38"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource39"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource40"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource41"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource42"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource43"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource44"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource45"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource46"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource47"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource48"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource49"),
      componentResourceManager.GetString("textBoxState.AutoCompleteCustomSource50")
    });
    this.textBoxState.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
    this.textBoxState.AutoCompleteSource = AutoCompleteSource.CustomSource;
    this.textBoxState.CharacterCasing = CharacterCasing.Upper;
    componentResourceManager.ApplyResources((object) this.textBoxState, "textBoxState");
    this.textBoxState.Name = "textBoxState";
    this.textBoxState.ModifiedChanged += new EventHandler(this.OnModified);
    componentResourceManager.ApplyResources((object) this.checkBoxAutoDataPageExtraction, "checkBoxAutoDataPageExtraction");
    child.SetColumnSpan((Control) this.checkBoxAutoDataPageExtraction, 4);
    this.checkBoxAutoDataPageExtraction.Name = "checkBoxAutoDataPageExtraction";
    this.checkBoxAutoDataPageExtraction.UseVisualStyleBackColor = true;
    this.checkBoxAutoDataPageExtraction.CheckedChanged += new EventHandler(this.checkBoxAutoDataPageExtraction_CheckedChanged);
    componentResourceManager.ApplyResources((object) this, "$this");
    this.Controls.Add((Control) child);
    this.Name = nameof (FleetInfoOptionsPanel);
    this.Controls.SetChildIndex((Control) child, 0);
    child.ResumeLayout(false);
    child.PerformLayout();
    this.ResumeLayout(false);
  }
}
