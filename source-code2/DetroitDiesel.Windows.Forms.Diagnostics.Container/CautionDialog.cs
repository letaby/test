using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;

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
		InitializeComponent();
		Text = ApplicationInformation.ProductName;
		BackColor = ApplicationInformation.Branding.LogoBackColor;
		ForeColor = ApplicationInformation.Branding.LogoForeColor;
		pictureLogo.Image = ApplicationInformation.Branding.Logo;
		webBrowserCaution.DocumentText = string.Format(CultureInfo.InvariantCulture, Resources.CautionHTML, Resources.MessageDialog_CautionDialog_Title, Resources.MessageDialog_CautionDialog_Header, Resources.MessageDialog_CautionDialog_Bullet0Line0, Resources.MessageDialog_CautionDialog_Bullet0Line1, Resources.MessageDialog_CautionDialog_Bullet1Line0, Resources.MessageDialog_CautionDialog_Bullet2Line0, Resources.MessageDialog_CautionDialog_Footer);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Container.CautionDialog));
		this.pictureLogo = new System.Windows.Forms.PictureBox();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.webBrowserCaution = new System.Windows.Forms.WebBrowser();
		this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
		System.Windows.Forms.Button button = new System.Windows.Forms.Button();
		System.Windows.Forms.PictureBox pictureBox = new System.Windows.Forms.PictureBox();
		System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel();
		System.Windows.Forms.PictureBox pictureBox2 = new System.Windows.Forms.PictureBox();
		((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
		panel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureLogo).BeginInit();
		((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
		this.tableLayoutPanel1.SuspendLayout();
		this.tableLayoutPanel2.SuspendLayout();
		base.SuspendLayout();
		resources.ApplyResources(button, "buttonOK");
		button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		button.Name = "buttonOK";
		resources.ApplyResources(pictureBox, "pictureBoxCautionRight");
		pictureBox.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.caution;
		pictureBox.Name = "pictureBoxCautionRight";
		pictureBox.TabStop = false;
		panel.Controls.Add(this.pictureLogo);
		panel.Controls.Add(pictureBox2);
		panel.Controls.Add(pictureBox);
		resources.ApplyResources(panel, "panelHeader");
		panel.Name = "panelHeader";
		resources.ApplyResources(this.pictureLogo, "pictureLogo");
		this.pictureLogo.Name = "pictureLogo";
		this.pictureLogo.TabStop = false;
		resources.ApplyResources(pictureBox2, "pictureBoxCautionLeft");
		pictureBox2.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.caution;
		pictureBox2.Name = "pictureBoxCautionLeft";
		pictureBox2.TabStop = false;
		resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
		this.tableLayoutPanel2.SetColumnSpan(this.tableLayoutPanel1, 2);
		this.tableLayoutPanel1.Controls.Add(this.webBrowserCaution, 0, 1);
		this.tableLayoutPanel1.Controls.Add(panel, 0, 0);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.webBrowserCaution.AllowNavigation = false;
		this.webBrowserCaution.AllowWebBrowserDrop = false;
		this.webBrowserCaution.IsWebBrowserContextMenuEnabled = false;
		resources.ApplyResources(this.webBrowserCaution, "webBrowserCaution");
		this.webBrowserCaution.Name = "webBrowserCaution";
		this.webBrowserCaution.ScriptErrorsSuppressed = true;
		this.webBrowserCaution.ScrollBarsEnabled = false;
		this.webBrowserCaution.TabStop = false;
		this.webBrowserCaution.WebBrowserShortcutsEnabled = false;
		this.webBrowserCaution.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(webBrowserCaution_DocumentCompleted);
		resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
		this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 0);
		this.tableLayoutPanel2.Controls.Add(button, 1, 1);
		this.tableLayoutPanel2.Name = "tableLayoutPanel2";
		base.AcceptButton = button;
		resources.ApplyResources(this, "$this");
		base.CancelButton = button;
		base.Controls.Add(this.tableLayoutPanel2);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "CautionDialog";
		base.ShowInTaskbar = false;
		((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
		panel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pictureLogo).EndInit();
		((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
		this.tableLayoutPanel1.ResumeLayout(false);
		this.tableLayoutPanel2.ResumeLayout(false);
		this.tableLayoutPanel2.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	private void webBrowserCaution_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
	{
		using Graphics graphics = CreateGraphics();
		webBrowserCaution.Height = (int)((double)((float)webBrowserCaution.Document.Body.ScrollRectangle.Height * graphics.DpiY) / 96.0);
	}
}
