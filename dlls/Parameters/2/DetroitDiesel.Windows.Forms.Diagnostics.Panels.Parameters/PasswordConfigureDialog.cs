using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Security;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;

internal class PasswordConfigureDialog : Form
{
	private PasswordManager passwordManager;

	private int selectedProtectionList;

	private const byte tooManyAttempts = 84;

	private const byte exceedNumberAttempts = 148;

	private IContainer components;

	private Button closeButton;

	private GroupBox groupBox1;

	private Panel entryPanel;

	private RadioButton protectionList2Label;

	private RadioButton protectionList3Label;

	private RadioButton protectionList4Label;

	private RadioButton protectionList5Label;

	private RadioButton protectionList6Label;

	private RadioButton protectionList7Label;

	private RadioButton protectionList1Label;

	private System.Windows.Forms.Label label1;

	private PasswordBox oldPasswordTextBox;

	private System.Windows.Forms.Label label2;

	private PasswordBox newPasswordTextBox;

	private Button backdoorButton;

	private Button sendButton;

	private System.Windows.Forms.Label label3;

	private TableLayoutPanel tableLayoutPanel1;

	private TableLayoutPanel tableLayoutPanel2;

	private bool Online => passwordManager.Channel.Online;

	public PasswordConfigureDialog(PasswordManager passwordManager)
	{
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		this.passwordManager = passwordManager;
		for (int i = 0; i < 7; i++)
		{
			RadioButton control = GetControl<RadioButton>(i);
			if (i < this.passwordManager.ProtectionListCount)
			{
				control.Visible = true;
				string key = "ProtectionList" + (i + 1).ToString(CultureInfo.InvariantCulture) + "Text";
				if (this.passwordManager.Channel.Ecu.Properties.ContainsKey(key))
				{
					control.Text = this.passwordManager.Channel.Ecu.Properties[key];
				}
			}
			else
			{
				control.Visible = false;
			}
		}
		protectionList1Label.Checked = true;
	}

	private T GetControl<T>(int index) where T : Control
	{
		int num = 0;
		foreach (Control control in entryPanel.Controls)
		{
			if (control.GetType() == typeof(T))
			{
				if (index == num)
				{
					return control as T;
				}
				num++;
			}
		}
		return null;
	}

	private int GetIndex(Control desiredControl)
	{
		int num = 0;
		foreach (Control control in entryPanel.Controls)
		{
			if (control.GetType() == desiredControl.GetType())
			{
				if (control == desiredControl)
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	private void sendButton_Click(object sender, EventArgs e)
	{
		if (!Online)
		{
			Close();
			return;
		}
		string text = string.Empty;
		if (!((Control)(object)oldPasswordTextBox).Enabled || ((Control)(object)oldPasswordTextBox).Text.Length > 0)
		{
			if (((Control)(object)oldPasswordTextBox).Enabled)
			{
				try
				{
					passwordManager.UnlockList(selectedProtectionList, ((Control)(object)oldPasswordTextBox).Text);
				}
				catch (CaesarException ex)
				{
					text = ex.Message;
					if (ex.NegativeResponseCode == 84 || ex.NegativeResponseCode == 148)
					{
						base.DialogResult = DialogResult.Abort;
					}
					((Control)(object)oldPasswordTextBox).Focus();
				}
				catch (FormatException ex2)
				{
					text = ex2.Message;
					((Control)(object)oldPasswordTextBox).Focus();
				}
			}
			if (text.Length == 0)
			{
				try
				{
					if (((Control)(object)newPasswordTextBox).Text.Length > 0)
					{
						passwordManager.SetPasswordForList(selectedProtectionList, ((Control)(object)newPasswordTextBox).Text);
					}
					else
					{
						passwordManager.ClearPasswordForList(selectedProtectionList);
					}
					string text2 = string.Format(CultureInfo.CurrentCulture, (((Control)(object)newPasswordTextBox).Text.Length > 0) ? Resources.MessageFormatPasswordForListSuccessfullyChanged : Resources.MessageFormatPasswordForListSuccessfullyCleared, selectedProtectionList + 1);
					ControlHelpers.ShowMessageBox((Control)this, text2, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
					UpdateTextBoxes();
				}
				catch (CaesarException ex3)
				{
					text = ex3.Message;
					((Control)(object)newPasswordTextBox).Focus();
				}
				catch (FormatException ex4)
				{
					text = ex4.Message;
					((Control)(object)newPasswordTextBox).Focus();
				}
				if (text.Length > 0)
				{
					text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormatWhileConfiguringNewPassword, text);
				}
			}
			else
			{
				text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormatWhileSubmittingOldPassword, text);
			}
		}
		else
		{
			text = Resources.MessageMustProvidePreviousPassword;
		}
		if (text.Length > 0)
		{
			string text3 = ((base.DialogResult != DialogResult.Abort) ? string.Format(CultureInfo.CurrentCulture, Resources.FormatMessageOtherError, text) : string.Format(CultureInfo.CurrentCulture, Resources.FormatMessageAborted, text));
			ControlHelpers.ShowMessageBox((Control)this, text3, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
		}
		if (base.DialogResult == DialogResult.Abort)
		{
			Close();
		}
	}

	protected override void OnFormClosing(FormClosingEventArgs e)
	{
		if (Online)
		{
			passwordManager.Commit();
		}
		else
		{
			ControlHelpers.ShowMessageBox((Control)this, Resources.MessageChangesMayNotHaveTakenEffectAsTheTargetDeviceIsOffline, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
		}
		base.OnFormClosing(e);
	}

	private void protectionListLabel_CheckedChanged(object sender, EventArgs e)
	{
		selectedProtectionList = GetIndex(sender as Control);
		UpdateTextBoxes();
		UpdateSendButton();
	}

	private void UpdateTextBoxes()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Invalid comparison between Unknown and I4
		if (!Online)
		{
			Close();
			return;
		}
		bool enabled = false;
		try
		{
			if ((int)passwordManager.ListStatus(selectedProtectionList) == 1)
			{
				enabled = true;
			}
		}
		catch (CaesarException)
		{
		}
		((Control)(object)oldPasswordTextBox).Enabled = enabled;
		((Control)(object)oldPasswordTextBox).Text = string.Empty;
		((Control)(object)newPasswordTextBox).Text = string.Empty;
	}

	private void backdoorButton_Click(object sender, EventArgs e)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		if (!Online)
		{
			Close();
			return;
		}
		try
		{
			PasswordBackdoorDialog val = new PasswordBackdoorDialog(passwordManager, passwordManager.ReadBackdoorSeed(), (Action)delegate
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				Link.ShowTarget(Link.AvailableLinks.GetLinkOrEmpty("Form_PasswordBackdoorDialog"));
			}, true);
			((Form)(object)val).ShowDialog();
			UpdateTextBoxes();
			UpdateSendButton();
		}
		catch (CaesarException ex)
		{
			ControlHelpers.ShowMessageBox((Control)this, string.Format(CultureInfo.CurrentCulture, Resources.MessageFormatErrorsOccurredDuringRetrieval, ex.Message), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
		}
	}

	private void oldPasswordTextBox_TextChanged(object sender, EventArgs e)
	{
		UpdateSendButton();
	}

	private void UpdateSendButton()
	{
		sendButton.Enabled = !((Control)(object)oldPasswordTextBox).Enabled || ((Control)(object)oldPasswordTextBox).Text.Length > 0;
	}

	private void OnHelpButtonClicked(object sender, CancelEventArgs e)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		e.Cancel = true;
		Link.ShowTarget(Link.AvailableLinks.GetLinkOrEmpty("Form_PasswordConfigureDialog"));
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
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.PasswordConfigureDialog));
		this.closeButton = new System.Windows.Forms.Button();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.label3 = new System.Windows.Forms.Label();
		this.sendButton = new System.Windows.Forms.Button();
		this.entryPanel = new System.Windows.Forms.Panel();
		this.protectionList1Label = new System.Windows.Forms.RadioButton();
		this.protectionList2Label = new System.Windows.Forms.RadioButton();
		this.protectionList3Label = new System.Windows.Forms.RadioButton();
		this.protectionList4Label = new System.Windows.Forms.RadioButton();
		this.protectionList5Label = new System.Windows.Forms.RadioButton();
		this.protectionList6Label = new System.Windows.Forms.RadioButton();
		this.protectionList7Label = new System.Windows.Forms.RadioButton();
		this.newPasswordTextBox = new PasswordBox();
		this.oldPasswordTextBox = new PasswordBox();
		this.label2 = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		this.backdoorButton = new System.Windows.Forms.Button();
		this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
		this.groupBox1.SuspendLayout();
		this.tableLayoutPanel1.SuspendLayout();
		this.entryPanel.SuspendLayout();
		this.tableLayoutPanel2.SuspendLayout();
		base.SuspendLayout();
		resources.ApplyResources(this.closeButton, "closeButton");
		this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.closeButton.Name = "closeButton";
		this.closeButton.UseVisualStyleBackColor = true;
		resources.ApplyResources(this.groupBox1, "groupBox1");
		this.groupBox1.Controls.Add(this.tableLayoutPanel1);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.TabStop = false;
		resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
		this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
		this.tableLayoutPanel1.Controls.Add(this.sendButton, 4, 2);
		this.tableLayoutPanel1.Controls.Add(this.entryPanel, 0, 1);
		this.tableLayoutPanel1.Controls.Add((System.Windows.Forms.Control)(object)this.newPasswordTextBox, 3, 2);
		this.tableLayoutPanel1.Controls.Add((System.Windows.Forms.Control)(object)this.oldPasswordTextBox, 1, 2);
		this.tableLayoutPanel1.Controls.Add(this.label2, 2, 2);
		this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		resources.ApplyResources(this.label3, "label3");
		this.tableLayoutPanel1.SetColumnSpan(this.label3, 3);
		this.label3.Name = "label3";
		resources.ApplyResources(this.sendButton, "sendButton");
		this.sendButton.Name = "sendButton";
		this.sendButton.UseVisualStyleBackColor = true;
		this.sendButton.Click += new System.EventHandler(sendButton_Click);
		this.tableLayoutPanel1.SetColumnSpan(this.entryPanel, 5);
		this.entryPanel.Controls.Add(this.protectionList1Label);
		this.entryPanel.Controls.Add(this.protectionList2Label);
		this.entryPanel.Controls.Add(this.protectionList3Label);
		this.entryPanel.Controls.Add(this.protectionList4Label);
		this.entryPanel.Controls.Add(this.protectionList5Label);
		this.entryPanel.Controls.Add(this.protectionList6Label);
		this.entryPanel.Controls.Add(this.protectionList7Label);
		resources.ApplyResources(this.entryPanel, "entryPanel");
		this.entryPanel.Name = "entryPanel";
		resources.ApplyResources(this.protectionList1Label, "protectionList1Label");
		this.protectionList1Label.Name = "protectionList1Label";
		this.protectionList1Label.CheckedChanged += new System.EventHandler(protectionListLabel_CheckedChanged);
		resources.ApplyResources(this.protectionList2Label, "protectionList2Label");
		this.protectionList2Label.Name = "protectionList2Label";
		this.protectionList2Label.CheckedChanged += new System.EventHandler(protectionListLabel_CheckedChanged);
		resources.ApplyResources(this.protectionList3Label, "protectionList3Label");
		this.protectionList3Label.Name = "protectionList3Label";
		this.protectionList3Label.CheckedChanged += new System.EventHandler(protectionListLabel_CheckedChanged);
		resources.ApplyResources(this.protectionList4Label, "protectionList4Label");
		this.protectionList4Label.Name = "protectionList4Label";
		this.protectionList4Label.CheckedChanged += new System.EventHandler(protectionListLabel_CheckedChanged);
		resources.ApplyResources(this.protectionList5Label, "protectionList5Label");
		this.protectionList5Label.Name = "protectionList5Label";
		this.protectionList5Label.CheckedChanged += new System.EventHandler(protectionListLabel_CheckedChanged);
		resources.ApplyResources(this.protectionList6Label, "protectionList6Label");
		this.protectionList6Label.Name = "protectionList6Label";
		this.protectionList6Label.CheckedChanged += new System.EventHandler(protectionListLabel_CheckedChanged);
		resources.ApplyResources(this.protectionList7Label, "protectionList7Label");
		this.protectionList7Label.Name = "protectionList7Label";
		this.protectionList7Label.CheckedChanged += new System.EventHandler(protectionListLabel_CheckedChanged);
		resources.ApplyResources(this.newPasswordTextBox, "newPasswordTextBox");
		((System.Windows.Forms.Control)(object)this.newPasswordTextBox).Name = "newPasswordTextBox";
		resources.ApplyResources(this.oldPasswordTextBox, "oldPasswordTextBox");
		((System.Windows.Forms.Control)(object)this.oldPasswordTextBox).Name = "oldPasswordTextBox";
		((System.Windows.Forms.Control)(object)this.oldPasswordTextBox).TextChanged += new System.EventHandler(oldPasswordTextBox_TextChanged);
		resources.ApplyResources(this.label2, "label2");
		this.label2.Name = "label2";
		resources.ApplyResources(this.label1, "label1");
		this.label1.Name = "label1";
		resources.ApplyResources(this.backdoorButton, "backdoorButton");
		this.backdoorButton.Name = "backdoorButton";
		this.backdoorButton.UseVisualStyleBackColor = true;
		this.backdoorButton.Click += new System.EventHandler(backdoorButton_Click);
		resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
		this.tableLayoutPanel2.Controls.Add(this.backdoorButton, 1, 0);
		this.tableLayoutPanel2.Controls.Add(this.closeButton, 2, 0);
		this.tableLayoutPanel2.Name = "tableLayoutPanel2";
		base.AcceptButton = this.closeButton;
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.groupBox1);
		base.Controls.Add(this.tableLayoutPanel2);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.HelpButton = true;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "PasswordConfigureDialog";
		base.ShowInTaskbar = false;
		base.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(OnHelpButtonClicked);
		this.groupBox1.ResumeLayout(false);
		this.tableLayoutPanel1.ResumeLayout(false);
		this.tableLayoutPanel1.PerformLayout();
		this.entryPanel.ResumeLayout(false);
		this.entryPanel.PerformLayout();
		this.tableLayoutPanel2.ResumeLayout(false);
		this.tableLayoutPanel2.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
