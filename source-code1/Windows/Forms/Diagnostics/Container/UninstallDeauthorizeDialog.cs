// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.UninstallDeauthorizeDialog
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Interfaces;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
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
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    ServerClient.GlobalInstance.Complete += new EventHandler<ClientConnectionCompleteEventArgs>(this.ServerClient_Complete);
  }

  private void ServerClient_Complete(object sender, ClientConnectionCompleteEventArgs e)
  {
    this.Close();
  }

  private void buttonYes_Click(object sender, EventArgs e)
  {
    this.buttonNo.Visible = false;
    this.buttonYes.Visible = false;
    this.pictureBox1.Visible = false;
    this.label.Text = Resources.Message_Deauthorizing;
    this.label.Font = new Font(this.label.Font.FontFamily, this.label.Font.Size * 2f);
    ServerOptionsPanel.DeauthorizeClient();
  }

  private void buttonNo_Click(object sender, EventArgs e) => this.Close();

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
    {
      ServerClient.GlobalInstance.Complete -= new EventHandler<ClientConnectionCompleteEventArgs>(this.ServerClient_Complete);
      this.components.Dispose();
    }
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UninstallDeauthorizeDialog));
    this.label = new Label();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.buttonYes = new Button();
    this.buttonNo = new Button();
    this.pictureBox1 = new PictureBox();
    this.tableLayoutPanel1.SuspendLayout();
    ((ISupportInitialize) this.pictureBox1).BeginInit();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) this.label, "label");
    this.label.BackColor = SystemColors.Control;
    this.tableLayoutPanel1.SetColumnSpan((Control) this.label, 3);
    this.label.Name = "label";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    this.tableLayoutPanel1.Controls.Add((Control) this.label, 1, 0);
    this.tableLayoutPanel1.Controls.Add((Control) this.buttonYes, 2, 1);
    this.tableLayoutPanel1.Controls.Add((Control) this.buttonNo, 3, 1);
    this.tableLayoutPanel1.Controls.Add((Control) this.pictureBox1, 0, 0);
    this.tableLayoutPanel1.Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.buttonYes, "buttonYes");
    this.buttonYes.Name = "buttonYes";
    this.buttonYes.UseVisualStyleBackColor = true;
    this.buttonYes.Click += new EventHandler(this.buttonYes_Click);
    componentResourceManager.ApplyResources((object) this.buttonNo, "buttonNo");
    this.buttonNo.Name = "buttonNo";
    this.buttonNo.UseVisualStyleBackColor = true;
    this.buttonNo.Click += new EventHandler(this.buttonNo_Click);
    this.pictureBox1.Image = (Image) Resources.caution;
    componentResourceManager.ApplyResources((object) this.pictureBox1, "pictureBox1");
    this.pictureBox1.Name = "pictureBox1";
    this.pictureBox1.TabStop = false;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.ControlBox = false;
    this.Controls.Add((Control) this.tableLayoutPanel1);
    this.FormBorderStyle = FormBorderStyle.FixedDialog;
    this.Name = nameof (UninstallDeauthorizeDialog);
    this.ShowIcon = false;
    this.ShowInTaskbar = false;
    this.TopMost = true;
    this.tableLayoutPanel1.ResumeLayout(false);
    this.tableLayoutPanel1.PerformLayout();
    ((ISupportInitialize) this.pictureBox1).EndInit();
    this.ResumeLayout(false);
    this.PerformLayout();
  }
}
