using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.DataHub.Properties;

namespace DetroitDiesel.DataHub;

public class PasswordFleetDataPagesChangePasswordDialog : Form
{
	private IContainer components;

	private Button okButton;

	private Button cancelButton;

	private PasswordBox textBoxOldPassword;

	private PasswordBox textBoxNewPassword;

	private PasswordBox textBoxConfirmNewPassword;

	private Label labelOldPassword;

	private Label lblNewPassword;

	private Label lblConfirmPassword;

	public string OldPassword => ((Control)(object)textBoxOldPassword).Text;

	public string NewPassword => ((Control)(object)textBoxNewPassword).Text;

	public PasswordFleetDataPagesChangePasswordDialog(bool passwordRequired)
	{
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		((Control)(object)textBoxOldPassword).Enabled = passwordRequired;
		((TextBox)(object)textBoxOldPassword).UseSystemPasswordChar = true;
		((TextBox)(object)textBoxNewPassword).UseSystemPasswordChar = true;
		((TextBox)(object)textBoxConfirmNewPassword).UseSystemPasswordChar = true;
		((TextBoxBase)(object)textBoxOldPassword).MaxLength = 6;
		((TextBoxBase)(object)textBoxNewPassword).MaxLength = 6;
		((TextBoxBase)(object)textBoxConfirmNewPassword).MaxLength = 6;
	}

	protected override void OnFormClosing(FormClosingEventArgs e)
	{
		if (base.DialogResult == DialogResult.OK)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (string.Compare(((Control)(object)textBoxNewPassword).Text, ((Control)(object)textBoxConfirmNewPassword).Text, StringComparison.CurrentCulture) != 0)
			{
				stringBuilder.Append(Resource.ChangeDataPagesPassword_PasswordNotSame);
				((Control)(object)textBoxNewPassword).Focus();
				e.Cancel = true;
			}
			if (!string.IsNullOrEmpty(((Control)(object)textBoxNewPassword).Text) && !((Control)(object)textBoxNewPassword).Text.All(char.IsLetterOrDigit))
			{
				stringBuilder.Append(Resource.ChangeDataPagesPassword_PasswordMustContains);
				((Control)(object)textBoxNewPassword).Text = string.Empty;
				((Control)(object)textBoxConfirmNewPassword).Text = string.Empty;
				((Control)(object)textBoxNewPassword).Focus();
				e.Cancel = true;
			}
			if (stringBuilder.Length > 0)
			{
				ControlHelpers.ShowMessageBox((Control)this, stringBuilder.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
		}
		base.OnFormClosing(e);
	}

	private void textBoxNewPassword_KeyPress(object sender, KeyPressEventArgs e)
	{
		e.Handled = !char.IsControl(e.KeyChar) && !char.IsLetterOrDigit(e.KeyChar);
	}

	private void textBoxConfirmNewPassword_KeyPress(object sender, KeyPressEventArgs e)
	{
		e.Handled = !char.IsControl(e.KeyChar) && !char.IsLetterOrDigit(e.KeyChar);
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
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.DataHub.PasswordFleetDataPagesChangePasswordDialog));
		this.okButton = new System.Windows.Forms.Button();
		this.cancelButton = new System.Windows.Forms.Button();
		this.textBoxOldPassword = new PasswordBox();
		this.textBoxNewPassword = new PasswordBox();
		this.textBoxConfirmNewPassword = new PasswordBox();
		this.labelOldPassword = new System.Windows.Forms.Label();
		this.lblNewPassword = new System.Windows.Forms.Label();
		this.lblConfirmPassword = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
		componentResourceManager.ApplyResources(this.okButton, "okButton");
		this.okButton.Name = "okButton";
		this.okButton.UseVisualStyleBackColor = true;
		this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		componentResourceManager.ApplyResources(this.cancelButton, "cancelButton");
		this.cancelButton.Name = "cancelButton";
		this.cancelButton.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this.textBoxOldPassword, "textBoxOldPassword");
		((System.Windows.Forms.Control)(object)this.textBoxOldPassword).Name = "textBoxOldPassword";
		componentResourceManager.ApplyResources(this.textBoxNewPassword, "textBoxNewPassword");
		((System.Windows.Forms.Control)(object)this.textBoxNewPassword).Name = "textBoxNewPassword";
		((System.Windows.Forms.Control)(object)this.textBoxNewPassword).KeyPress += new System.Windows.Forms.KeyPressEventHandler(textBoxNewPassword_KeyPress);
		componentResourceManager.ApplyResources(this.textBoxConfirmNewPassword, "textBoxConfirmNewPassword");
		((System.Windows.Forms.Control)(object)this.textBoxConfirmNewPassword).Name = "textBoxConfirmNewPassword";
		((System.Windows.Forms.Control)(object)this.textBoxConfirmNewPassword).KeyPress += new System.Windows.Forms.KeyPressEventHandler(textBoxConfirmNewPassword_KeyPress);
		componentResourceManager.ApplyResources(this.labelOldPassword, "labelOldPassword");
		this.labelOldPassword.Name = "labelOldPassword";
		componentResourceManager.ApplyResources(this.lblNewPassword, "lblNewPassword");
		this.lblNewPassword.Name = "lblNewPassword";
		componentResourceManager.ApplyResources(this.lblConfirmPassword, "lblConfirmPassword");
		this.lblConfirmPassword.Name = "lblConfirmPassword";
		base.AcceptButton = this.okButton;
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.cancelButton;
		base.Controls.Add(this.lblConfirmPassword);
		base.Controls.Add(this.lblNewPassword);
		base.Controls.Add(this.labelOldPassword);
		base.Controls.Add((System.Windows.Forms.Control)(object)this.textBoxConfirmNewPassword);
		base.Controls.Add((System.Windows.Forms.Control)(object)this.textBoxNewPassword);
		base.Controls.Add(this.okButton);
		base.Controls.Add(this.cancelButton);
		base.Controls.Add((System.Windows.Forms.Control)(object)this.textBoxOldPassword);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "PasswordFleetDataPagesChangePasswordDialog";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
