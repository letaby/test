using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public class ChargeDialog : Form
{
	private IContainer components;

	private Button buttonCancel;

	private Button buttonOK;

	private Label label;

	private TextBox textBox;

	public string ReferenceNumber
	{
		get
		{
			return textBox.Text;
		}
		set
		{
			textBox.Text = value;
		}
	}

	public ChargeDialog()
	{
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
	}

	private void buttonOK_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void buttonCancel_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.ChargeDialog));
		this.buttonCancel = new System.Windows.Forms.Button();
		this.buttonOK = new System.Windows.Forms.Button();
		this.label = new System.Windows.Forms.Label();
		this.textBox = new System.Windows.Forms.TextBox();
		base.SuspendLayout();
		resources.ApplyResources(this.buttonCancel, "buttonCancel");
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.UseVisualStyleBackColor = true;
		this.buttonCancel.Click += new System.EventHandler(buttonCancel_Click);
		resources.ApplyResources(this.buttonOK, "buttonOK");
		this.buttonOK.Name = "buttonOK";
		this.buttonOK.UseVisualStyleBackColor = true;
		this.buttonOK.Click += new System.EventHandler(buttonOK_Click);
		resources.ApplyResources(this.label, "label");
		this.label.Name = "label";
		resources.ApplyResources(this.textBox, "textBox");
		this.textBox.Name = "textBox";
		base.AcceptButton = this.buttonOK;
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.buttonCancel;
		base.Controls.Add(this.textBox);
		base.Controls.Add(this.label);
		base.Controls.Add(this.buttonOK);
		base.Controls.Add(this.buttonCancel);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "ChargeDialog";
		base.ShowIcon = false;
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
