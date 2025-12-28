using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Container;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;

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
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		versionBox.Text = ApplicationInformation.Version;
		computerIdTextBox.Text = ApplicationInformation.ComputerId;
		pictureLogo.BackColor = ApplicationInformation.Branding.LogoBackColor;
		pictureLogo.Image = ApplicationInformation.Branding.Logo;
		base.Icon = ApplicationInformation.Branding.ProductIcon;
		ServerClient.GlobalInstance.Complete += OnServerClientComplete;
		IsKeyDeregistered = SapiManager.GlobalInstance.RegistrationKey.Equals("deregister", StringComparison.OrdinalIgnoreCase);
		registationKey.Text = (IsKeyDeregistered ? string.Empty : SapiManager.GlobalInstance.RegistrationKey);
		UpdateLicenseRegistrationUI();
	}

	public static bool ShowRegistrationDialog()
	{
		using ServerRegistrationDialog serverRegistrationDialog = new ServerRegistrationDialog();
		return serverRegistrationDialog.ShowDialog() == DialogResult.OK;
	}

	private void OnExitButtonClick(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

	private void OnRegisterButtonClick(object sender, EventArgs e)
	{
		SapiManager.GlobalInstance.RegistrationKey = registationKey.Text.ToUpper(CultureInfo.InvariantCulture).Trim();
		ServerRegistration.GlobalInstance.ComputerDescription = computerDescription.Text.Trim();
		if (!ServerClient.GlobalInstance.InUse)
		{
			ServerClient.GlobalInstance.Go((Collection<UnitInformation>)null, (Collection<UnitInformation>)null);
		}
	}

	private void OnServerClientComplete(object sender, ClientConnectionCompleteEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if ((int)e.Status == 0)
		{
			base.DialogResult = DialogResult.Cancel;
			Close();
		}
		else if (ServerRegistration.GlobalInstance.Valid)
		{
			base.DialogResult = DialogResult.OK;
			Close();
		}
	}

	private void buttonOptions_Click(object sender, EventArgs e)
	{
		OptionsDialog optionsDialog = new OptionsDialog(new ServerOptionsPanel());
		ServerClient.GlobalInstance.Complete -= OnServerClientComplete;
		optionsDialog.ShowDialog();
		ServerClient.GlobalInstance.Complete += OnServerClientComplete;
		optionsDialog.Dispose();
		UpdateLicenseRegistrationUI();
	}

	private void OnRegistationKeyTextChanged(object sender, EventArgs e)
	{
		registerButton.Enabled = false;
		if (validRegistrationKeyFormat.IsMatch(registationKey.Text.Trim()) || string.IsNullOrEmpty(registationKey.Text.Trim()) || IsKeyDeregistered)
		{
			registerButton.Enabled = true;
		}
	}

	private void UpdateLicenseRegistrationUI()
	{
		bool flag = ServerRegistration.GlobalInstance.ToolLicenseExpirationDate != DateTime.MinValue && ServerRegistration.GlobalInstance.RegisteredApplicationName == ApplicationInformation.Branding.ApplicationName;
		computerDescription.Text = ServerRegistration.GlobalInstance.ComputerDescription;
		if (string.IsNullOrEmpty(computerDescription.Text))
		{
			computerDescription.Text = SettingsManager.ReadRegistryKey("ComputerDescription");
		}
		bool visible = !flag || IsKeyDeregistered;
		registationKey.Visible = visible;
		registrationKeyLabel.Visible = visible;
		compterDescriptionLabel.Visible = visible;
		computerDescription.Visible = visible;
		optionalLabel.Visible = visible;
		string format = Resources.FormatServerRegistrationDialog;
		if (flag && !IsKeyDeregistered)
		{
			format = ((!(DateTime.Today < ServerRegistration.GlobalInstance.ToolLicenseExpirationDate)) ? Resources.ServerRegistrationHasExpired : Resources.ServerConnectionHasExpired);
		}
		licenseLabel.Text = string.Format(CultureInfo.CurrentCulture, format, ApplicationInformation.ProductName);
	}

	private void computerDescription_TextChanged(object sender, EventArgs e)
	{
		if (!validComputerDescriptionCharacters.IsMatch(computerDescription.Text))
		{
			computerDescription.Text = previouslyValidatedText;
			computerDescription.SelectionStart = computerDescription.Text.Length;
		}
		else
		{
			previouslyValidatedText = computerDescription.Text;
		}
	}

	protected override void Dispose(bool disposing)
	{
		ServerClient.GlobalInstance.Complete -= OnServerClientComplete;
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Settings.ServerRegistrationDialog));
		this.exitButton = new System.Windows.Forms.Button();
		this.registerButton = new System.Windows.Forms.Button();
		this.buttonOptions = new System.Windows.Forms.Button();
		this.computerIdLabel = new System.Windows.Forms.Label();
		this.computerIdTextBox = new System.Windows.Forms.TextBox();
		this.versionLabel = new System.Windows.Forms.Label();
		this.versionBox = new System.Windows.Forms.TextBox();
		this.pictureLogo = new System.Windows.Forms.PictureBox();
		this.licenseLabel = new System.Windows.Forms.Label();
		this.registationKey = new System.Windows.Forms.TextBox();
		this.registrationKeyLabel = new System.Windows.Forms.Label();
		this.compterDescriptionLabel = new System.Windows.Forms.Label();
		this.computerDescription = new System.Windows.Forms.TextBox();
		this.optionalLabel = new System.Windows.Forms.Label();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		((System.ComponentModel.ISupportInitialize)this.pictureLogo).BeginInit();
		this.tableLayoutPanel1.SuspendLayout();
		base.SuspendLayout();
		resources.ApplyResources(this.exitButton, "exitButton");
		this.exitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.exitButton.Name = "exitButton";
		this.exitButton.Click += new System.EventHandler(OnExitButtonClick);
		resources.ApplyResources(this.registerButton, "registerButton");
		this.registerButton.Name = "registerButton";
		this.registerButton.UseVisualStyleBackColor = true;
		this.registerButton.Click += new System.EventHandler(OnRegisterButtonClick);
		resources.ApplyResources(this.buttonOptions, "buttonOptions");
		this.buttonOptions.Name = "buttonOptions";
		this.buttonOptions.UseVisualStyleBackColor = true;
		this.buttonOptions.Click += new System.EventHandler(buttonOptions_Click);
		resources.ApplyResources(this.computerIdLabel, "computerIdLabel");
		this.computerIdLabel.Name = "computerIdLabel";
		this.tableLayoutPanel1.SetColumnSpan(this.computerIdTextBox, 4);
		resources.ApplyResources(this.computerIdTextBox, "computerIdTextBox");
		this.computerIdTextBox.Name = "computerIdTextBox";
		this.computerIdTextBox.ReadOnly = true;
		resources.ApplyResources(this.versionLabel, "versionLabel");
		this.versionLabel.Name = "versionLabel";
		this.tableLayoutPanel1.SetColumnSpan(this.versionBox, 4);
		resources.ApplyResources(this.versionBox, "versionBox");
		this.versionBox.Name = "versionBox";
		this.versionBox.ReadOnly = true;
		this.pictureLogo.BackColor = System.Drawing.SystemColors.Control;
		this.pictureLogo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		resources.ApplyResources(this.pictureLogo, "pictureLogo");
		this.pictureLogo.Name = "pictureLogo";
		this.pictureLogo.TabStop = false;
		resources.ApplyResources(this.licenseLabel, "licenseLabel");
		this.tableLayoutPanel1.SetColumnSpan(this.licenseLabel, 4);
		this.licenseLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
		this.licenseLabel.Name = "licenseLabel";
		this.tableLayoutPanel1.SetColumnSpan(this.registationKey, 4);
		resources.ApplyResources(this.registationKey, "registationKey");
		this.registationKey.Name = "registationKey";
		this.registationKey.TextChanged += new System.EventHandler(OnRegistationKeyTextChanged);
		resources.ApplyResources(this.registrationKeyLabel, "registrationKeyLabel");
		this.registrationKeyLabel.Name = "registrationKeyLabel";
		resources.ApplyResources(this.compterDescriptionLabel, "compterDescriptionLabel");
		this.compterDescriptionLabel.Name = "compterDescriptionLabel";
		this.tableLayoutPanel1.SetColumnSpan(this.computerDescription, 4);
		resources.ApplyResources(this.computerDescription, "computerDescription");
		this.computerDescription.Name = "computerDescription";
		this.computerDescription.TextChanged += new System.EventHandler(computerDescription_TextChanged);
		resources.ApplyResources(this.optionalLabel, "optionalLabel");
		this.optionalLabel.Name = "optionalLabel";
		this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
		resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
		this.tableLayoutPanel1.Controls.Add(this.versionLabel, 0, 1);
		this.tableLayoutPanel1.Controls.Add(this.computerIdLabel, 0, 2);
		this.tableLayoutPanel1.Controls.Add(this.registrationKeyLabel, 0, 3);
		this.tableLayoutPanel1.Controls.Add(this.compterDescriptionLabel, 0, 4);
		this.tableLayoutPanel1.Controls.Add(this.optionalLabel, 0, 5);
		this.tableLayoutPanel1.Controls.Add(this.computerDescription, 1, 4);
		this.tableLayoutPanel1.Controls.Add(this.licenseLabel, 1, 0);
		this.tableLayoutPanel1.Controls.Add(this.registationKey, 1, 3);
		this.tableLayoutPanel1.Controls.Add(this.versionBox, 1, 1);
		this.tableLayoutPanel1.Controls.Add(this.computerIdTextBox, 1, 2);
		this.tableLayoutPanel1.Controls.Add(this.registerButton, 2, 7);
		this.tableLayoutPanel1.Controls.Add(this.buttonOptions, 3, 7);
		this.tableLayoutPanel1.Controls.Add(this.exitButton, 4, 7);
		this.tableLayoutPanel1.Controls.Add(this.pictureLogo, 0, 0);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		base.AcceptButton = this.registerButton;
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.exitButton;
		base.ControlBox = false;
		base.Controls.Add(this.tableLayoutPanel1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "ServerRegistrationDialog";
		((System.ComponentModel.ISupportInitialize)this.pictureLogo).EndInit();
		this.tableLayoutPanel1.ResumeLayout(false);
		this.tableLayoutPanel1.PerformLayout();
		base.ResumeLayout(false);
	}
}
