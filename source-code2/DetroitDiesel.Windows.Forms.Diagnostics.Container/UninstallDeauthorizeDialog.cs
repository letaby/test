using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public class UninstallDeauthorizeDialog : Form
{
	private IContainer components;

	private Label label;

	private TableLayoutPanel tableLayoutPanel1;

	private Button buttonYes;

	private Button buttonNo;

	private PictureBox pictureBox1;

	public UninstallDeauthorizeDialog()
	{
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		ServerClient.GlobalInstance.Complete += ServerClient_Complete;
	}

	private void ServerClient_Complete(object sender, ClientConnectionCompleteEventArgs e)
	{
		Close();
	}

	private void buttonYes_Click(object sender, EventArgs e)
	{
		buttonNo.Visible = false;
		buttonYes.Visible = false;
		pictureBox1.Visible = false;
		label.Text = Resources.Message_Deauthorizing;
		label.Font = new Font(label.Font.FontFamily, label.Font.Size * 2f);
		ServerOptionsPanel.DeauthorizeClient();
	}

	private void buttonNo_Click(object sender, EventArgs e)
	{
		Close();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			ServerClient.GlobalInstance.Complete -= ServerClient_Complete;
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Container.UninstallDeauthorizeDialog));
		this.label = new System.Windows.Forms.Label();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.buttonYes = new System.Windows.Forms.Button();
		this.buttonNo = new System.Windows.Forms.Button();
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.tableLayoutPanel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		base.SuspendLayout();
		resources.ApplyResources(this.label, "label");
		this.label.BackColor = System.Drawing.SystemColors.Control;
		this.tableLayoutPanel1.SetColumnSpan(this.label, 3);
		this.label.Name = "label";
		resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
		this.tableLayoutPanel1.Controls.Add(this.label, 1, 0);
		this.tableLayoutPanel1.Controls.Add(this.buttonYes, 2, 1);
		this.tableLayoutPanel1.Controls.Add(this.buttonNo, 3, 1);
		this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		resources.ApplyResources(this.buttonYes, "buttonYes");
		this.buttonYes.Name = "buttonYes";
		this.buttonYes.UseVisualStyleBackColor = true;
		this.buttonYes.Click += new System.EventHandler(buttonYes_Click);
		resources.ApplyResources(this.buttonNo, "buttonNo");
		this.buttonNo.Name = "buttonNo";
		this.buttonNo.UseVisualStyleBackColor = true;
		this.buttonNo.Click += new System.EventHandler(buttonNo_Click);
		this.pictureBox1.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.caution;
		resources.ApplyResources(this.pictureBox1, "pictureBox1");
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.TabStop = false;
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ControlBox = false;
		base.Controls.Add(this.tableLayoutPanel1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.Name = "UninstallDeauthorizeDialog";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.TopMost = true;
		this.tableLayoutPanel1.ResumeLayout(false);
		this.tableLayoutPanel1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
