// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Settings.ServerRegistrationDialog
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Common;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Container;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Settings;

internal class ServerRegistrationDialog : Form
{
  private readonly Regex validRegistrationKeyFormat = new Regex("^([a-zA-Z0-9]{4}-){2}([a-zA-Z0-9]{4})$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
  private readonly Regex validComputerDescriptionCharacters = new Regex("^[a-zA-Z0-9\\s-_.]*$");
  private const string DeregisterKey = "deregister";
  private string previouslyValidatedText = string.Empty;
  private IContainer components;
  private Button exitButton;
  private Button registerButton;
  private Button buttonOptions;
  private Label computerIdLabel;
  private TextBox computerIdTextBox;
  private Label versionLabel;
  private TextBox versionBox;
  private PictureBox pictureLogo;
  private Label licenseLabel;
  private TextBox registationKey;
  private Label registrationKeyLabel;
  private Label compterDescriptionLabel;
  private TextBox computerDescription;
  private Label optionalLabel;
  private TableLayoutPanel tableLayoutPanel1;

  private bool IsKeyDeregistered { get; set; }

  private ServerRegistrationDialog()
  {
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    this.versionBox.Text = ApplicationInformation.Version;
    this.computerIdTextBox.Text = ApplicationInformation.ComputerId;
    this.pictureLogo.BackColor = ApplicationInformation.Branding.LogoBackColor;
    this.pictureLogo.Image = ApplicationInformation.Branding.Logo;
    this.Icon = ApplicationInformation.Branding.ProductIcon;
    ServerClient.GlobalInstance.Complete += new EventHandler<ClientConnectionCompleteEventArgs>(this.OnServerClientComplete);
    this.IsKeyDeregistered = SapiManager.GlobalInstance.RegistrationKey.Equals("deregister", StringComparison.OrdinalIgnoreCase);
    this.registationKey.Text = this.IsKeyDeregistered ? string.Empty : SapiManager.GlobalInstance.RegistrationKey;
    this.UpdateLicenseRegistrationUI();
  }

  public static bool ShowRegistrationDialog()
  {
    using (ServerRegistrationDialog registrationDialog = new ServerRegistrationDialog())
      return registrationDialog.ShowDialog() == DialogResult.OK;
  }

  private void OnExitButtonClick(object sender, EventArgs e)
  {
    this.DialogResult = DialogResult.Cancel;
    this.Close();
  }

  private void OnRegisterButtonClick(object sender, EventArgs e)
  {
    SapiManager.GlobalInstance.RegistrationKey = this.registationKey.Text.ToUpper(CultureInfo.InvariantCulture).Trim();
    ServerRegistration.GlobalInstance.ComputerDescription = this.computerDescription.Text.Trim();
    if (ServerClient.GlobalInstance.InUse)
      return;
    ServerClient.GlobalInstance.Go((Collection<UnitInformation>) null, (Collection<UnitInformation>) null);
  }

  private void OnServerClientComplete(object sender, ClientConnectionCompleteEventArgs e)
  {
    if (e.Status == null)
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }
    else
    {
      if (!ServerRegistration.GlobalInstance.Valid)
        return;
      this.DialogResult = DialogResult.OK;
      this.Close();
    }
  }

  private void buttonOptions_Click(object sender, EventArgs e)
  {
    OptionsDialog optionsDialog = new OptionsDialog((OptionsPanel) new ServerOptionsPanel());
    ServerClient.GlobalInstance.Complete -= new EventHandler<ClientConnectionCompleteEventArgs>(this.OnServerClientComplete);
    int num = (int) optionsDialog.ShowDialog();
    ServerClient.GlobalInstance.Complete += new EventHandler<ClientConnectionCompleteEventArgs>(this.OnServerClientComplete);
    optionsDialog.Dispose();
    this.UpdateLicenseRegistrationUI();
  }

  private void OnRegistationKeyTextChanged(object sender, EventArgs e)
  {
    this.registerButton.Enabled = false;
    if (!this.validRegistrationKeyFormat.IsMatch(this.registationKey.Text.Trim()) && !string.IsNullOrEmpty(this.registationKey.Text.Trim()) && !this.IsKeyDeregistered)
      return;
    this.registerButton.Enabled = true;
  }

  private void UpdateLicenseRegistrationUI()
  {
    bool flag1 = ServerRegistration.GlobalInstance.ToolLicenseExpirationDate != DateTime.MinValue && ServerRegistration.GlobalInstance.RegisteredApplicationName == ApplicationInformation.Branding.ApplicationName;
    this.computerDescription.Text = ServerRegistration.GlobalInstance.ComputerDescription;
    if (string.IsNullOrEmpty(this.computerDescription.Text))
      this.computerDescription.Text = SettingsManager.ReadRegistryKey("ComputerDescription");
    bool flag2 = !flag1 || this.IsKeyDeregistered;
    this.registationKey.Visible = flag2;
    this.registrationKeyLabel.Visible = flag2;
    this.compterDescriptionLabel.Visible = flag2;
    this.computerDescription.Visible = flag2;
    this.optionalLabel.Visible = flag2;
    string format = Resources.FormatServerRegistrationDialog;
    if (flag1 && !this.IsKeyDeregistered)
      format = !(DateTime.Today < ServerRegistration.GlobalInstance.ToolLicenseExpirationDate) ? Resources.ServerRegistrationHasExpired : Resources.ServerConnectionHasExpired;
    this.licenseLabel.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, (object) ApplicationInformation.ProductName);
  }

  private void computerDescription_TextChanged(object sender, EventArgs e)
  {
    if (!this.validComputerDescriptionCharacters.IsMatch(this.computerDescription.Text))
    {
      this.computerDescription.Text = this.previouslyValidatedText;
      this.computerDescription.SelectionStart = this.computerDescription.Text.Length;
    }
    else
      this.previouslyValidatedText = this.computerDescription.Text;
  }

  protected override void Dispose(bool disposing)
  {
    ServerClient.GlobalInstance.Complete -= new EventHandler<ClientConnectionCompleteEventArgs>(this.OnServerClientComplete);
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ServerRegistrationDialog));
    this.exitButton = new Button();
    this.registerButton = new Button();
    this.buttonOptions = new Button();
    this.computerIdLabel = new Label();
    this.computerIdTextBox = new TextBox();
    this.versionLabel = new Label();
    this.versionBox = new TextBox();
    this.pictureLogo = new PictureBox();
    this.licenseLabel = new Label();
    this.registationKey = new TextBox();
    this.registrationKeyLabel = new Label();
    this.compterDescriptionLabel = new Label();
    this.computerDescription = new TextBox();
    this.optionalLabel = new Label();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    ((ISupportInitialize) this.pictureLogo).BeginInit();
    this.tableLayoutPanel1.SuspendLayout();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) this.exitButton, "exitButton");
    this.exitButton.DialogResult = DialogResult.Cancel;
    this.exitButton.Name = "exitButton";
    this.exitButton.Click += new EventHandler(this.OnExitButtonClick);
    componentResourceManager.ApplyResources((object) this.registerButton, "registerButton");
    this.registerButton.Name = "registerButton";
    this.registerButton.UseVisualStyleBackColor = true;
    this.registerButton.Click += new EventHandler(this.OnRegisterButtonClick);
    componentResourceManager.ApplyResources((object) this.buttonOptions, "buttonOptions");
    this.buttonOptions.Name = "buttonOptions";
    this.buttonOptions.UseVisualStyleBackColor = true;
    this.buttonOptions.Click += new EventHandler(this.buttonOptions_Click);
    componentResourceManager.ApplyResources((object) this.computerIdLabel, "computerIdLabel");
    this.computerIdLabel.Name = "computerIdLabel";
    this.tableLayoutPanel1.SetColumnSpan((Control) this.computerIdTextBox, 4);
    componentResourceManager.ApplyResources((object) this.computerIdTextBox, "computerIdTextBox");
    this.computerIdTextBox.Name = "computerIdTextBox";
    this.computerIdTextBox.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.versionLabel, "versionLabel");
    this.versionLabel.Name = "versionLabel";
    this.tableLayoutPanel1.SetColumnSpan((Control) this.versionBox, 4);
    componentResourceManager.ApplyResources((object) this.versionBox, "versionBox");
    this.versionBox.Name = "versionBox";
    this.versionBox.ReadOnly = true;
    this.pictureLogo.BackColor = SystemColors.Control;
    this.pictureLogo.BorderStyle = BorderStyle.FixedSingle;
    componentResourceManager.ApplyResources((object) this.pictureLogo, "pictureLogo");
    this.pictureLogo.Name = "pictureLogo";
    this.pictureLogo.TabStop = false;
    componentResourceManager.ApplyResources((object) this.licenseLabel, "licenseLabel");
    this.tableLayoutPanel1.SetColumnSpan((Control) this.licenseLabel, 4);
    this.licenseLabel.FlatStyle = FlatStyle.System;
    this.licenseLabel.Name = "licenseLabel";
    this.tableLayoutPanel1.SetColumnSpan((Control) this.registationKey, 4);
    componentResourceManager.ApplyResources((object) this.registationKey, "registationKey");
    this.registationKey.Name = "registationKey";
    this.registationKey.TextChanged += new EventHandler(this.OnRegistationKeyTextChanged);
    componentResourceManager.ApplyResources((object) this.registrationKeyLabel, "registrationKeyLabel");
    this.registrationKeyLabel.Name = "registrationKeyLabel";
    componentResourceManager.ApplyResources((object) this.compterDescriptionLabel, "compterDescriptionLabel");
    this.compterDescriptionLabel.Name = "compterDescriptionLabel";
    this.tableLayoutPanel1.SetColumnSpan((Control) this.computerDescription, 4);
    componentResourceManager.ApplyResources((object) this.computerDescription, "computerDescription");
    this.computerDescription.Name = "computerDescription";
    this.computerDescription.TextChanged += new EventHandler(this.computerDescription_TextChanged);
    componentResourceManager.ApplyResources((object) this.optionalLabel, "optionalLabel");
    this.optionalLabel.Name = "optionalLabel";
    this.tableLayoutPanel1.BackColor = SystemColors.Control;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    this.tableLayoutPanel1.Controls.Add((Control) this.versionLabel, 0, 1);
    this.tableLayoutPanel1.Controls.Add((Control) this.computerIdLabel, 0, 2);
    this.tableLayoutPanel1.Controls.Add((Control) this.registrationKeyLabel, 0, 3);
    this.tableLayoutPanel1.Controls.Add((Control) this.compterDescriptionLabel, 0, 4);
    this.tableLayoutPanel1.Controls.Add((Control) this.optionalLabel, 0, 5);
    this.tableLayoutPanel1.Controls.Add((Control) this.computerDescription, 1, 4);
    this.tableLayoutPanel1.Controls.Add((Control) this.licenseLabel, 1, 0);
    this.tableLayoutPanel1.Controls.Add((Control) this.registationKey, 1, 3);
    this.tableLayoutPanel1.Controls.Add((Control) this.versionBox, 1, 1);
    this.tableLayoutPanel1.Controls.Add((Control) this.computerIdTextBox, 1, 2);
    this.tableLayoutPanel1.Controls.Add((Control) this.registerButton, 2, 7);
    this.tableLayoutPanel1.Controls.Add((Control) this.buttonOptions, 3, 7);
    this.tableLayoutPanel1.Controls.Add((Control) this.exitButton, 4, 7);
    this.tableLayoutPanel1.Controls.Add((Control) this.pictureLogo, 0, 0);
    this.tableLayoutPanel1.Name = "tableLayoutPanel1";
    this.AcceptButton = (IButtonControl) this.registerButton;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.CancelButton = (IButtonControl) this.exitButton;
    this.ControlBox = false;
    this.Controls.Add((Control) this.tableLayoutPanel1);
    this.FormBorderStyle = FormBorderStyle.FixedDialog;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (ServerRegistrationDialog);
    ((ISupportInitialize) this.pictureLogo).EndInit();
    this.tableLayoutPanel1.ResumeLayout(false);
    this.tableLayoutPanel1.PerformLayout();
    this.ResumeLayout(false);
  }
}
