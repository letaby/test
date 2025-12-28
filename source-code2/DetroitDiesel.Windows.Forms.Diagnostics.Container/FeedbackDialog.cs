using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;

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
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		if (!string.IsNullOrEmpty(fleetInformation.CompanyName))
		{
			textBoxCompany.Text = fleetInformation.CompanyName;
		}
		else
		{
			textBoxCompany.Text = Environment.UserDomainName;
		}
		if (!string.IsNullOrEmpty(fleetInformation.TelephoneNumber))
		{
			maskedTextBoxTelephoneNumber.Text = fleetInformation.TelephoneNumber;
		}
		textBoxName.Text = Environment.UserName;
	}

	private void OnSubmitClick(object sender, EventArgs e)
	{
		ServerDataManager.SaveUserFeedback(textBoxCompany.Text, textBoxName.Text, textBoxEmail.Text, maskedTextBoxTelephoneNumber.Text, textBoxUserFeedbackForm.Text);
		ControlHelpers.ShowMessageBox((Control)this, Resources.MessageFeedBackReportSubmitInformation, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
	}

	private void OnTextBoxUserFeedbackCommentsChanged(object sender, EventArgs e)
	{
		if (textBoxUserFeedbackForm.Text.Length > 0)
		{
			buttonSubmit.Enabled = true;
		}
		else
		{
			buttonSubmit.Enabled = false;
		}
	}

	private void OnHelpButtonClicked(object sender, CancelEventArgs e)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		e.Cancel = true;
		Link.ShowTarget(Link.AvailableLinks.GetLinkOrEmpty("Form_FeedbackDialog"));
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Container.FeedbackDialog));
		this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
		this.labelComment = new System.Windows.Forms.Label();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.labelCompany = new System.Windows.Forms.Label();
		this.textBoxCompany = new System.Windows.Forms.TextBox();
		this.textBoxUserFeedbackForm = new System.Windows.Forms.TextBox();
		this.labelComments = new System.Windows.Forms.Label();
		this.maskedTextBoxTelephoneNumber = new System.Windows.Forms.MaskedTextBox();
		this.labelPhone = new System.Windows.Forms.Label();
		this.textBoxEmail = new System.Windows.Forms.TextBox();
		this.labelEmail = new System.Windows.Forms.Label();
		this.labelName = new System.Windows.Forms.Label();
		this.textBoxName = new System.Windows.Forms.TextBox();
		this.warningLabel = new System.Windows.Forms.Label();
		this.buttonSubmit = new System.Windows.Forms.Button();
		this.buttonCancel = new System.Windows.Forms.Button();
		this.tableLayoutPanel.SuspendLayout();
		this.tableLayoutPanel1.SuspendLayout();
		base.SuspendLayout();
		resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
		this.tableLayoutPanel.Controls.Add(this.labelComment, 0, 0);
		this.tableLayoutPanel.Controls.Add(this.tableLayoutPanel1, 0, 2);
		this.tableLayoutPanel.Controls.Add(this.warningLabel, 0, 1);
		this.tableLayoutPanel.Controls.Add(this.buttonSubmit, 0, 3);
		this.tableLayoutPanel.Controls.Add(this.buttonCancel, 1, 3);
		this.tableLayoutPanel.Name = "tableLayoutPanel";
		resources.ApplyResources(this.labelComment, "labelComment");
		this.tableLayoutPanel.SetColumnSpan(this.labelComment, 2);
		this.labelComment.Name = "labelComment";
		resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
		this.tableLayoutPanel.SetColumnSpan(this.tableLayoutPanel1, 2);
		this.tableLayoutPanel1.Controls.Add(this.labelCompany, 0, 0);
		this.tableLayoutPanel1.Controls.Add(this.textBoxCompany, 1, 0);
		this.tableLayoutPanel1.Controls.Add(this.textBoxUserFeedbackForm, 1, 4);
		this.tableLayoutPanel1.Controls.Add(this.labelComments, 0, 4);
		this.tableLayoutPanel1.Controls.Add(this.maskedTextBoxTelephoneNumber, 1, 3);
		this.tableLayoutPanel1.Controls.Add(this.labelPhone, 0, 3);
		this.tableLayoutPanel1.Controls.Add(this.textBoxEmail, 1, 2);
		this.tableLayoutPanel1.Controls.Add(this.labelEmail, 0, 2);
		this.tableLayoutPanel1.Controls.Add(this.labelName, 0, 1);
		this.tableLayoutPanel1.Controls.Add(this.textBoxName, 1, 1);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		resources.ApplyResources(this.labelCompany, "labelCompany");
		this.labelCompany.Name = "labelCompany";
		this.tableLayoutPanel1.SetColumnSpan(this.textBoxCompany, 2);
		resources.ApplyResources(this.textBoxCompany, "textBoxCompany");
		this.textBoxCompany.Name = "textBoxCompany";
		this.textBoxUserFeedbackForm.AcceptsReturn = true;
		this.tableLayoutPanel1.SetColumnSpan(this.textBoxUserFeedbackForm, 2);
		resources.ApplyResources(this.textBoxUserFeedbackForm, "textBoxUserFeedbackForm");
		this.textBoxUserFeedbackForm.Name = "textBoxUserFeedbackForm";
		this.textBoxUserFeedbackForm.TextChanged += new System.EventHandler(OnTextBoxUserFeedbackCommentsChanged);
		resources.ApplyResources(this.labelComments, "labelComments");
		this.labelComments.Name = "labelComments";
		this.maskedTextBoxTelephoneNumber.AllowPromptAsInput = false;
		this.tableLayoutPanel1.SetColumnSpan(this.maskedTextBoxTelephoneNumber, 2);
		resources.ApplyResources(this.maskedTextBoxTelephoneNumber, "maskedTextBoxTelephoneNumber");
		this.maskedTextBoxTelephoneNumber.HidePromptOnLeave = true;
		this.maskedTextBoxTelephoneNumber.Name = "maskedTextBoxTelephoneNumber";
		resources.ApplyResources(this.labelPhone, "labelPhone");
		this.labelPhone.Name = "labelPhone";
		this.tableLayoutPanel1.SetColumnSpan(this.textBoxEmail, 2);
		resources.ApplyResources(this.textBoxEmail, "textBoxEmail");
		this.textBoxEmail.Name = "textBoxEmail";
		resources.ApplyResources(this.labelEmail, "labelEmail");
		this.labelEmail.Name = "labelEmail";
		resources.ApplyResources(this.labelName, "labelName");
		this.labelName.Name = "labelName";
		this.tableLayoutPanel1.SetColumnSpan(this.textBoxName, 2);
		resources.ApplyResources(this.textBoxName, "textBoxName");
		this.textBoxName.Name = "textBoxName";
		resources.ApplyResources(this.warningLabel, "warningLabel");
		this.tableLayoutPanel.SetColumnSpan(this.warningLabel, 2);
		this.warningLabel.Name = "warningLabel";
		resources.ApplyResources(this.buttonSubmit, "buttonSubmit");
		this.buttonSubmit.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.buttonSubmit.Name = "buttonSubmit";
		this.buttonSubmit.UseVisualStyleBackColor = true;
		this.buttonSubmit.Click += new System.EventHandler(OnSubmitClick);
		resources.ApplyResources(this.buttonCancel, "buttonCancel");
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.UseVisualStyleBackColor = true;
		base.AcceptButton = this.buttonSubmit;
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.buttonCancel;
		base.Controls.Add(this.tableLayoutPanel);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.HelpButton = true;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "FeedbackDialog";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(OnHelpButtonClicked);
		this.tableLayoutPanel.ResumeLayout(false);
		this.tableLayoutPanel.PerformLayout();
		this.tableLayoutPanel1.ResumeLayout(false);
		this.tableLayoutPanel1.PerformLayout();
		base.ResumeLayout(false);
	}
}
