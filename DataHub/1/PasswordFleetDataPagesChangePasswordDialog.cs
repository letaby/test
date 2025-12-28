// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.DataHub.PasswordFleetDataPagesChangePasswordDialog
// Assembly: DataHub, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 89346980-C6E7-48B1-88DD-CF29796E810E
// Assembly location: C:\Users\petra\Downloads\Архив (2)\DataHub.dll

using DetroitDiesel.Common;
using DetroitDiesel.DataHub.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

#nullable disable
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

  public PasswordFleetDataPagesChangePasswordDialog(bool passwordRequired)
  {
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    ((Control) this.textBoxOldPassword).Enabled = passwordRequired;
    ((TextBox) this.textBoxOldPassword).UseSystemPasswordChar = true;
    ((TextBox) this.textBoxNewPassword).UseSystemPasswordChar = true;
    ((TextBox) this.textBoxConfirmNewPassword).UseSystemPasswordChar = true;
    ((TextBoxBase) this.textBoxOldPassword).MaxLength = 6;
    ((TextBoxBase) this.textBoxNewPassword).MaxLength = 6;
    ((TextBoxBase) this.textBoxConfirmNewPassword).MaxLength = 6;
  }

  public string OldPassword => ((Control) this.textBoxOldPassword).Text;

  public string NewPassword => ((Control) this.textBoxNewPassword).Text;

  protected override void OnFormClosing(FormClosingEventArgs e)
  {
    if (this.DialogResult == DialogResult.OK)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (string.Compare(((Control) this.textBoxNewPassword).Text, ((Control) this.textBoxConfirmNewPassword).Text, StringComparison.CurrentCulture) != 0)
      {
        stringBuilder.Append(Resource.ChangeDataPagesPassword_PasswordNotSame);
        ((Control) this.textBoxNewPassword).Focus();
        e.Cancel = true;
      }
      if (!string.IsNullOrEmpty(((Control) this.textBoxNewPassword).Text) && !((Control) this.textBoxNewPassword).Text.All<char>(new Func<char, bool>(char.IsLetterOrDigit)))
      {
        stringBuilder.Append(Resource.ChangeDataPagesPassword_PasswordMustContains);
        ((Control) this.textBoxNewPassword).Text = string.Empty;
        ((Control) this.textBoxConfirmNewPassword).Text = string.Empty;
        ((Control) this.textBoxNewPassword).Focus();
        e.Cancel = true;
      }
      if (stringBuilder.Length > 0)
      {
        int num = (int) ControlHelpers.ShowMessageBox((Control) this, stringBuilder.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
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
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (PasswordFleetDataPagesChangePasswordDialog));
    this.okButton = new Button();
    this.cancelButton = new Button();
    this.textBoxOldPassword = new PasswordBox();
    this.textBoxNewPassword = new PasswordBox();
    this.textBoxConfirmNewPassword = new PasswordBox();
    this.labelOldPassword = new Label();
    this.lblNewPassword = new Label();
    this.lblConfirmPassword = new Label();
    this.SuspendLayout();
    this.okButton.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.okButton, "okButton");
    this.okButton.Name = "okButton";
    this.okButton.UseVisualStyleBackColor = true;
    this.cancelButton.DialogResult = DialogResult.Cancel;
    componentResourceManager.ApplyResources((object) this.cancelButton, "cancelButton");
    this.cancelButton.Name = "cancelButton";
    this.cancelButton.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.textBoxOldPassword, "textBoxOldPassword");
    ((Control) this.textBoxOldPassword).Name = "textBoxOldPassword";
    componentResourceManager.ApplyResources((object) this.textBoxNewPassword, "textBoxNewPassword");
    ((Control) this.textBoxNewPassword).Name = "textBoxNewPassword";
    ((Control) this.textBoxNewPassword).KeyPress += new KeyPressEventHandler(this.textBoxNewPassword_KeyPress);
    componentResourceManager.ApplyResources((object) this.textBoxConfirmNewPassword, "textBoxConfirmNewPassword");
    ((Control) this.textBoxConfirmNewPassword).Name = "textBoxConfirmNewPassword";
    ((Control) this.textBoxConfirmNewPassword).KeyPress += new KeyPressEventHandler(this.textBoxConfirmNewPassword_KeyPress);
    componentResourceManager.ApplyResources((object) this.labelOldPassword, "labelOldPassword");
    this.labelOldPassword.Name = "labelOldPassword";
    componentResourceManager.ApplyResources((object) this.lblNewPassword, "lblNewPassword");
    this.lblNewPassword.Name = "lblNewPassword";
    componentResourceManager.ApplyResources((object) this.lblConfirmPassword, "lblConfirmPassword");
    this.lblConfirmPassword.Name = "lblConfirmPassword";
    this.AcceptButton = (IButtonControl) this.okButton;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.CancelButton = (IButtonControl) this.cancelButton;
    this.Controls.Add((Control) this.lblConfirmPassword);
    this.Controls.Add((Control) this.lblNewPassword);
    this.Controls.Add((Control) this.labelOldPassword);
    this.Controls.Add((Control) this.textBoxConfirmNewPassword);
    this.Controls.Add((Control) this.textBoxNewPassword);
    this.Controls.Add((Control) this.okButton);
    this.Controls.Add((Control) this.cancelButton);
    this.Controls.Add((Control) this.textBoxOldPassword);
    this.FormBorderStyle = FormBorderStyle.FixedDialog;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (PasswordFleetDataPagesChangePasswordDialog);
    this.ShowIcon = false;
    this.ShowInTaskbar = false;
    this.ResumeLayout(false);
    this.PerformLayout();
  }
}
