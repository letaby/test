// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.DataHub.PasswordFleetDataPagesDialog
// Assembly: DataHub, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 89346980-C6E7-48B1-88DD-CF29796E810E
// Assembly location: C:\Users\petra\Downloads\Архив (2)\DataHub.dll

using DetroitDiesel.Common;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.DataHub;

public class PasswordFleetDataPagesDialog : Form
{
  private IContainer components;
  private Label labelPassword;
  private PasswordBox textBoxPassword;
  private Button okButton;
  private Button cancelButton;

  public string Password => ((Control) this.textBoxPassword).Text;

  public PasswordFleetDataPagesDialog()
  {
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (PasswordFleetDataPagesDialog));
    this.labelPassword = new Label();
    this.textBoxPassword = new PasswordBox();
    this.okButton = new Button();
    this.cancelButton = new Button();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) this.labelPassword, "labelPassword");
    this.labelPassword.Name = "labelPassword";
    componentResourceManager.ApplyResources((object) this.textBoxPassword, "textBoxPassword");
    ((Control) this.textBoxPassword).Name = "textBoxPassword";
    this.okButton.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.okButton, "okButton");
    this.okButton.Name = "okButton";
    this.okButton.UseVisualStyleBackColor = true;
    this.cancelButton.DialogResult = DialogResult.Cancel;
    componentResourceManager.ApplyResources((object) this.cancelButton, "cancelButton");
    this.cancelButton.Name = "cancelButton";
    this.cancelButton.UseVisualStyleBackColor = true;
    this.AcceptButton = (IButtonControl) this.okButton;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.CancelButton = (IButtonControl) this.cancelButton;
    this.Controls.Add((Control) this.okButton);
    this.Controls.Add((Control) this.cancelButton);
    this.Controls.Add((Control) this.textBoxPassword);
    this.Controls.Add((Control) this.labelPassword);
    this.FormBorderStyle = FormBorderStyle.FixedDialog;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (PasswordFleetDataPagesDialog);
    this.ShowIcon = false;
    this.ShowInTaskbar = false;
    this.ResumeLayout(false);
    this.PerformLayout();
  }
}
