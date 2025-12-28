// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.CautionDialog
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public class CautionDialog : Form
{
  private TableLayoutPanel tableLayoutPanel1;
  private PictureBox pictureLogo;
  private WebBrowser webBrowserCaution;
  private TableLayoutPanel tableLayoutPanel2;
  private System.ComponentModel.Container components;

  public CautionDialog()
  {
    this.InitializeComponent();
    this.Text = ApplicationInformation.ProductName;
    this.BackColor = ApplicationInformation.Branding.LogoBackColor;
    this.ForeColor = ApplicationInformation.Branding.LogoForeColor;
    this.pictureLogo.Image = ApplicationInformation.Branding.Logo;
    this.webBrowserCaution.DocumentText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.CautionHTML, (object) Resources.MessageDialog_CautionDialog_Title, (object) Resources.MessageDialog_CautionDialog_Header, (object) Resources.MessageDialog_CautionDialog_Bullet0Line0, (object) Resources.MessageDialog_CautionDialog_Bullet0Line1, (object) Resources.MessageDialog_CautionDialog_Bullet1Line0, (object) Resources.MessageDialog_CautionDialog_Bullet2Line0, (object) Resources.MessageDialog_CautionDialog_Footer);
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (CautionDialog));
    this.pictureLogo = new PictureBox();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.webBrowserCaution = new WebBrowser();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    Button button = new Button();
    PictureBox pictureBox1 = new PictureBox();
    Panel panel = new Panel();
    PictureBox pictureBox2 = new PictureBox();
    ((ISupportInitialize) pictureBox1).BeginInit();
    panel.SuspendLayout();
    ((ISupportInitialize) this.pictureLogo).BeginInit();
    ((ISupportInitialize) pictureBox2).BeginInit();
    this.tableLayoutPanel1.SuspendLayout();
    this.tableLayoutPanel2.SuspendLayout();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) button, "buttonOK");
    button.DialogResult = DialogResult.Cancel;
    button.Name = "buttonOK";
    componentResourceManager.ApplyResources((object) pictureBox1, "pictureBoxCautionRight");
    pictureBox1.Image = (Image) Resources.caution;
    pictureBox1.Name = "pictureBoxCautionRight";
    pictureBox1.TabStop = false;
    panel.Controls.Add((Control) this.pictureLogo);
    panel.Controls.Add((Control) pictureBox2);
    panel.Controls.Add((Control) pictureBox1);
    componentResourceManager.ApplyResources((object) panel, "panelHeader");
    panel.Name = "panelHeader";
    componentResourceManager.ApplyResources((object) this.pictureLogo, "pictureLogo");
    this.pictureLogo.Name = "pictureLogo";
    this.pictureLogo.TabStop = false;
    componentResourceManager.ApplyResources((object) pictureBox2, "pictureBoxCautionLeft");
    pictureBox2.Image = (Image) Resources.caution;
    pictureBox2.Name = "pictureBoxCautionLeft";
    pictureBox2.TabStop = false;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    this.tableLayoutPanel2.SetColumnSpan((Control) this.tableLayoutPanel1, 2);
    this.tableLayoutPanel1.Controls.Add((Control) this.webBrowserCaution, 0, 1);
    this.tableLayoutPanel1.Controls.Add((Control) panel, 0, 0);
    this.tableLayoutPanel1.Name = "tableLayoutPanel1";
    this.webBrowserCaution.AllowNavigation = false;
    this.webBrowserCaution.AllowWebBrowserDrop = false;
    this.webBrowserCaution.IsWebBrowserContextMenuEnabled = false;
    componentResourceManager.ApplyResources((object) this.webBrowserCaution, "webBrowserCaution");
    this.webBrowserCaution.Name = "webBrowserCaution";
    this.webBrowserCaution.ScriptErrorsSuppressed = true;
    this.webBrowserCaution.ScrollBarsEnabled = false;
    this.webBrowserCaution.TabStop = false;
    this.webBrowserCaution.WebBrowserShortcutsEnabled = false;
    this.webBrowserCaution.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.webBrowserCaution_DocumentCompleted);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    this.tableLayoutPanel2.Controls.Add((Control) this.tableLayoutPanel1, 0, 0);
    this.tableLayoutPanel2.Controls.Add((Control) button, 1, 1);
    this.tableLayoutPanel2.Name = "tableLayoutPanel2";
    this.AcceptButton = (IButtonControl) button;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.CancelButton = (IButtonControl) button;
    this.Controls.Add((Control) this.tableLayoutPanel2);
    this.FormBorderStyle = FormBorderStyle.FixedDialog;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (CautionDialog);
    this.ShowInTaskbar = false;
    ((ISupportInitialize) pictureBox1).EndInit();
    panel.ResumeLayout(false);
    ((ISupportInitialize) this.pictureLogo).EndInit();
    ((ISupportInitialize) pictureBox2).EndInit();
    this.tableLayoutPanel1.ResumeLayout(false);
    this.tableLayoutPanel2.ResumeLayout(false);
    this.tableLayoutPanel2.PerformLayout();
    this.ResumeLayout(false);
    this.PerformLayout();
  }

  private void webBrowserCaution_DocumentCompleted(
    object sender,
    WebBrowserDocumentCompletedEventArgs e)
  {
    using (Graphics graphics = this.CreateGraphics())
      this.webBrowserCaution.Height = (int) ((double) this.webBrowserCaution.Document.Body.ScrollRectangle.Height * (double) graphics.DpiY / 96.0);
  }
}
