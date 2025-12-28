// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.FeedbackDialog
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public class FeedbackDialog : Form
{
  private FleetInformation fleetInformation = FleetInformation.Load();
  private IContainer components;
  private TableLayoutPanel tableLayoutPanel;
  private Label labelComment;
  private Button buttonSubmit;
  private TableLayoutPanel tableLayoutPanel1;
  private Label labelCompany;
  private Label labelPhone;
  private TextBox textBoxEmail;
  private TextBox textBoxCompany;
  private Label labelEmail;
  private TextBox textBoxUserFeedbackForm;
  private Label labelComments;
  private MaskedTextBox maskedTextBoxTelephoneNumber;
  private Button buttonCancel;
  private Label warningLabel;
  private Label labelName;
  private TextBox textBoxName;

  public FeedbackDialog()
  {
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    if (!string.IsNullOrEmpty(this.fleetInformation.CompanyName))
      this.textBoxCompany.Text = this.fleetInformation.CompanyName;
    else
      this.textBoxCompany.Text = Environment.UserDomainName;
    if (!string.IsNullOrEmpty(this.fleetInformation.TelephoneNumber))
      this.maskedTextBoxTelephoneNumber.Text = this.fleetInformation.TelephoneNumber;
    this.textBoxName.Text = Environment.UserName;
  }

  private void OnSubmitClick(object sender, EventArgs e)
  {
    ServerDataManager.SaveUserFeedback(this.textBoxCompany.Text, this.textBoxName.Text, this.textBoxEmail.Text, this.maskedTextBoxTelephoneNumber.Text, this.textBoxUserFeedbackForm.Text);
    int num = (int) ControlHelpers.ShowMessageBox((Control) this, Resources.MessageFeedBackReportSubmitInformation, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
  }

  private void OnTextBoxUserFeedbackCommentsChanged(object sender, EventArgs e)
  {
    if (this.textBoxUserFeedbackForm.Text.Length > 0)
      this.buttonSubmit.Enabled = true;
    else
      this.buttonSubmit.Enabled = false;
  }

  private void OnHelpButtonClicked(object sender, CancelEventArgs e)
  {
    e.Cancel = true;
    Link.ShowTarget(Link.AvailableLinks.GetLinkOrEmpty("Form_FeedbackDialog"));
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (FeedbackDialog));
    this.tableLayoutPanel = new TableLayoutPanel();
    this.labelComment = new Label();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.labelCompany = new Label();
    this.textBoxCompany = new TextBox();
    this.textBoxUserFeedbackForm = new TextBox();
    this.labelComments = new Label();
    this.maskedTextBoxTelephoneNumber = new MaskedTextBox();
    this.labelPhone = new Label();
    this.textBoxEmail = new TextBox();
    this.labelEmail = new Label();
    this.labelName = new Label();
    this.textBoxName = new TextBox();
    this.warningLabel = new Label();
    this.buttonSubmit = new Button();
    this.buttonCancel = new Button();
    this.tableLayoutPanel.SuspendLayout();
    this.tableLayoutPanel1.SuspendLayout();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    this.tableLayoutPanel.Controls.Add((Control) this.labelComment, 0, 0);
    this.tableLayoutPanel.Controls.Add((Control) this.tableLayoutPanel1, 0, 2);
    this.tableLayoutPanel.Controls.Add((Control) this.warningLabel, 0, 1);
    this.tableLayoutPanel.Controls.Add((Control) this.buttonSubmit, 0, 3);
    this.tableLayoutPanel.Controls.Add((Control) this.buttonCancel, 1, 3);
    this.tableLayoutPanel.Name = "tableLayoutPanel";
    componentResourceManager.ApplyResources((object) this.labelComment, "labelComment");
    this.tableLayoutPanel.SetColumnSpan((Control) this.labelComment, 2);
    this.labelComment.Name = "labelComment";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    this.tableLayoutPanel.SetColumnSpan((Control) this.tableLayoutPanel1, 2);
    this.tableLayoutPanel1.Controls.Add((Control) this.labelCompany, 0, 0);
    this.tableLayoutPanel1.Controls.Add((Control) this.textBoxCompany, 1, 0);
    this.tableLayoutPanel1.Controls.Add((Control) this.textBoxUserFeedbackForm, 1, 4);
    this.tableLayoutPanel1.Controls.Add((Control) this.labelComments, 0, 4);
    this.tableLayoutPanel1.Controls.Add((Control) this.maskedTextBoxTelephoneNumber, 1, 3);
    this.tableLayoutPanel1.Controls.Add((Control) this.labelPhone, 0, 3);
    this.tableLayoutPanel1.Controls.Add((Control) this.textBoxEmail, 1, 2);
    this.tableLayoutPanel1.Controls.Add((Control) this.labelEmail, 0, 2);
    this.tableLayoutPanel1.Controls.Add((Control) this.labelName, 0, 1);
    this.tableLayoutPanel1.Controls.Add((Control) this.textBoxName, 1, 1);
    this.tableLayoutPanel1.Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.labelCompany, "labelCompany");
    this.labelCompany.Name = "labelCompany";
    this.tableLayoutPanel1.SetColumnSpan((Control) this.textBoxCompany, 2);
    componentResourceManager.ApplyResources((object) this.textBoxCompany, "textBoxCompany");
    this.textBoxCompany.Name = "textBoxCompany";
    this.textBoxUserFeedbackForm.AcceptsReturn = true;
    this.tableLayoutPanel1.SetColumnSpan((Control) this.textBoxUserFeedbackForm, 2);
    componentResourceManager.ApplyResources((object) this.textBoxUserFeedbackForm, "textBoxUserFeedbackForm");
    this.textBoxUserFeedbackForm.Name = "textBoxUserFeedbackForm";
    this.textBoxUserFeedbackForm.TextChanged += new EventHandler(this.OnTextBoxUserFeedbackCommentsChanged);
    componentResourceManager.ApplyResources((object) this.labelComments, "labelComments");
    this.labelComments.Name = "labelComments";
    this.maskedTextBoxTelephoneNumber.AllowPromptAsInput = false;
    this.tableLayoutPanel1.SetColumnSpan((Control) this.maskedTextBoxTelephoneNumber, 2);
    componentResourceManager.ApplyResources((object) this.maskedTextBoxTelephoneNumber, "maskedTextBoxTelephoneNumber");
    this.maskedTextBoxTelephoneNumber.HidePromptOnLeave = true;
    this.maskedTextBoxTelephoneNumber.Name = "maskedTextBoxTelephoneNumber";
    componentResourceManager.ApplyResources((object) this.labelPhone, "labelPhone");
    this.labelPhone.Name = "labelPhone";
    this.tableLayoutPanel1.SetColumnSpan((Control) this.textBoxEmail, 2);
    componentResourceManager.ApplyResources((object) this.textBoxEmail, "textBoxEmail");
    this.textBoxEmail.Name = "textBoxEmail";
    componentResourceManager.ApplyResources((object) this.labelEmail, "labelEmail");
    this.labelEmail.Name = "labelEmail";
    componentResourceManager.ApplyResources((object) this.labelName, "labelName");
    this.labelName.Name = "labelName";
    this.tableLayoutPanel1.SetColumnSpan((Control) this.textBoxName, 2);
    componentResourceManager.ApplyResources((object) this.textBoxName, "textBoxName");
    this.textBoxName.Name = "textBoxName";
    componentResourceManager.ApplyResources((object) this.warningLabel, "warningLabel");
    this.tableLayoutPanel.SetColumnSpan((Control) this.warningLabel, 2);
    this.warningLabel.Name = "warningLabel";
    componentResourceManager.ApplyResources((object) this.buttonSubmit, "buttonSubmit");
    this.buttonSubmit.DialogResult = DialogResult.OK;
    this.buttonSubmit.Name = "buttonSubmit";
    this.buttonSubmit.UseVisualStyleBackColor = true;
    this.buttonSubmit.Click += new EventHandler(this.OnSubmitClick);
    componentResourceManager.ApplyResources((object) this.buttonCancel, "buttonCancel");
    this.buttonCancel.DialogResult = DialogResult.Cancel;
    this.buttonCancel.Name = "buttonCancel";
    this.buttonCancel.UseVisualStyleBackColor = true;
    this.AcceptButton = (IButtonControl) this.buttonSubmit;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.CancelButton = (IButtonControl) this.buttonCancel;
    this.Controls.Add((Control) this.tableLayoutPanel);
    this.FormBorderStyle = FormBorderStyle.FixedDialog;
    this.HelpButton = true;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (FeedbackDialog);
    this.ShowIcon = false;
    this.ShowInTaskbar = false;
    this.HelpButtonClicked += new CancelEventHandler(this.OnHelpButtonClicked);
    this.tableLayoutPanel.ResumeLayout(false);
    this.tableLayoutPanel.PerformLayout();
    this.tableLayoutPanel1.ResumeLayout(false);
    this.tableLayoutPanel1.PerformLayout();
    this.ResumeLayout(false);
  }
}
