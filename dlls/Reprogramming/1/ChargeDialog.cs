// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.ChargeDialog
// Assembly: Reprogramming, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 6E09671B-250E-411A-80FC-C490A3A17075
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Reprogramming.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public class ChargeDialog : Form
{
  private IContainer components;
  private Button buttonCancel;
  private Button buttonOK;
  private Label label;
  private TextBox textBox;

  public ChargeDialog()
  {
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
  }

  public string ReferenceNumber
  {
    get => this.textBox.Text;
    set => this.textBox.Text = value;
  }

  private void buttonOK_Click(object sender, EventArgs e)
  {
    this.DialogResult = DialogResult.OK;
    this.Close();
  }

  private void buttonCancel_Click(object sender, EventArgs e)
  {
    this.DialogResult = DialogResult.Cancel;
    this.Close();
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ChargeDialog));
    this.buttonCancel = new Button();
    this.buttonOK = new Button();
    this.label = new Label();
    this.textBox = new TextBox();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) this.buttonCancel, "buttonCancel");
    this.buttonCancel.DialogResult = DialogResult.Cancel;
    this.buttonCancel.Name = "buttonCancel";
    this.buttonCancel.UseVisualStyleBackColor = true;
    this.buttonCancel.Click += new EventHandler(this.buttonCancel_Click);
    componentResourceManager.ApplyResources((object) this.buttonOK, "buttonOK");
    this.buttonOK.Name = "buttonOK";
    this.buttonOK.UseVisualStyleBackColor = true;
    this.buttonOK.Click += new EventHandler(this.buttonOK_Click);
    componentResourceManager.ApplyResources((object) this.label, "label");
    this.label.Name = "label";
    componentResourceManager.ApplyResources((object) this.textBox, "textBox");
    this.textBox.Name = "textBox";
    this.AcceptButton = (IButtonControl) this.buttonOK;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.CancelButton = (IButtonControl) this.buttonCancel;
    this.Controls.Add((Control) this.textBox);
    this.Controls.Add((Control) this.label);
    this.Controls.Add((Control) this.buttonOK);
    this.Controls.Add((Control) this.buttonCancel);
    this.FormBorderStyle = FormBorderStyle.FixedDialog;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (ChargeDialog);
    this.ShowIcon = false;
    this.ResumeLayout(false);
    this.PerformLayout();
  }
}
