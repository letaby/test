using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;

namespace DetroitDiesel.DataHub;

public class PasswordFleetDataPagesDialog : Form
{
	private IContainer components;

	private Label labelPassword;

	private PasswordBox textBoxPassword;

	private Button okButton;

	private Button cancelButton;

	public string Password => ((Control)(object)textBoxPassword).Text;

	public PasswordFleetDataPagesDialog()
	{
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
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
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.DataHub.PasswordFleetDataPagesDialog));
		this.labelPassword = new System.Windows.Forms.Label();
		this.textBoxPassword = new PasswordBox();
		this.okButton = new System.Windows.Forms.Button();
		this.cancelButton = new System.Windows.Forms.Button();
		base.SuspendLayout();
		componentResourceManager.ApplyResources(this.labelPassword, "labelPassword");
		this.labelPassword.Name = "labelPassword";
		componentResourceManager.ApplyResources(this.textBoxPassword, "textBoxPassword");
		((System.Windows.Forms.Control)(object)this.textBoxPassword).Name = "textBoxPassword";
		this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
		componentResourceManager.ApplyResources(this.okButton, "okButton");
		this.okButton.Name = "okButton";
		this.okButton.UseVisualStyleBackColor = true;
		this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		componentResourceManager.ApplyResources(this.cancelButton, "cancelButton");
		this.cancelButton.Name = "cancelButton";
		this.cancelButton.UseVisualStyleBackColor = true;
		base.AcceptButton = this.okButton;
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.cancelButton;
		base.Controls.Add(this.okButton);
		base.Controls.Add(this.cancelButton);
		base.Controls.Add((System.Windows.Forms.Control)(object)this.textBoxPassword);
		base.Controls.Add(this.labelPassword);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "PasswordFleetDataPagesDialog";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
