// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.PasswordConfigureDialog
// Assembly: Parameters, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 266306EF-5E5A-4E97-A95E-0BCBE6FD3F76
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Parameters.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Security;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

#nullable disable
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

  public PasswordConfigureDialog(PasswordManager passwordManager)
  {
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    this.passwordManager = passwordManager;
    for (int index = 0; index < 7; ++index)
    {
      RadioButton control = this.GetControl<RadioButton>(index);
      if (index < this.passwordManager.ProtectionListCount)
      {
        control.Visible = true;
        string key = $"ProtectionList{(index + 1).ToString((IFormatProvider) CultureInfo.InvariantCulture)}Text";
        if (this.passwordManager.Channel.Ecu.Properties.ContainsKey(key))
          control.Text = this.passwordManager.Channel.Ecu.Properties[key];
      }
      else
        control.Visible = false;
    }
    this.protectionList1Label.Checked = true;
  }

  private bool Online => this.passwordManager.Channel.Online;

  private T GetControl<T>(int index) where T : Control
  {
    int num = 0;
    foreach (Control control in (ArrangedElementCollection) this.entryPanel.Controls)
    {
      if (control.GetType() == typeof (T))
      {
        if (index == num)
          return control as T;
        ++num;
      }
    }
    return default (T);
  }

  private int GetIndex(Control desiredControl)
  {
    int index = 0;
    foreach (Control control in (ArrangedElementCollection) this.entryPanel.Controls)
    {
      if (control.GetType() == desiredControl.GetType())
      {
        if (control == desiredControl)
          return index;
        ++index;
      }
    }
    return -1;
  }

  private void sendButton_Click(object sender, EventArgs e)
  {
    if (!this.Online)
    {
      this.Close();
    }
    else
    {
      string str = string.Empty;
      if (!((Control) this.oldPasswordTextBox).Enabled || ((Control) this.oldPasswordTextBox).Text.Length > 0)
      {
        if (((Control) this.oldPasswordTextBox).Enabled)
        {
          try
          {
            this.passwordManager.UnlockList(this.selectedProtectionList, ((Control) this.oldPasswordTextBox).Text);
          }
          catch (CaesarException ex)
          {
            str = ex.Message;
            if (ex.NegativeResponseCode == 84 || ex.NegativeResponseCode == 148)
              this.DialogResult = DialogResult.Abort;
            ((Control) this.oldPasswordTextBox).Focus();
          }
          catch (FormatException ex)
          {
            str = ex.Message;
            ((Control) this.oldPasswordTextBox).Focus();
          }
        }
        if (str.Length == 0)
        {
          try
          {
            if (((Control) this.newPasswordTextBox).Text.Length > 0)
              this.passwordManager.SetPasswordForList(this.selectedProtectionList, ((Control) this.newPasswordTextBox).Text);
            else
              this.passwordManager.ClearPasswordForList(this.selectedProtectionList);
            int num = (int) ControlHelpers.ShowMessageBox((Control) this, string.Format((IFormatProvider) CultureInfo.CurrentCulture, ((Control) this.newPasswordTextBox).Text.Length > 0 ? Resources.MessageFormatPasswordForListSuccessfullyChanged : Resources.MessageFormatPasswordForListSuccessfullyCleared, (object) (this.selectedProtectionList + 1)), MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
            this.UpdateTextBoxes();
          }
          catch (CaesarException ex)
          {
            str = ex.Message;
            ((Control) this.newPasswordTextBox).Focus();
          }
          catch (FormatException ex)
          {
            str = ex.Message;
            ((Control) this.newPasswordTextBox).Focus();
          }
          if (str.Length > 0)
            str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormatWhileConfiguringNewPassword, (object) str);
        }
        else
          str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormatWhileSubmittingOldPassword, (object) str);
      }
      else
        str = Resources.MessageMustProvidePreviousPassword;
      if (str.Length > 0)
      {
        int num1 = (int) ControlHelpers.ShowMessageBox((Control) this, this.DialogResult != DialogResult.Abort ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatMessageOtherError, (object) str) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatMessageAborted, (object) str), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
      }
      if (this.DialogResult != DialogResult.Abort)
        return;
      this.Close();
    }
  }

  protected override void OnFormClosing(FormClosingEventArgs e)
  {
    if (this.Online)
    {
      this.passwordManager.Commit();
    }
    else
    {
      int num = (int) ControlHelpers.ShowMessageBox((Control) this, Resources.MessageChangesMayNotHaveTakenEffectAsTheTargetDeviceIsOffline, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
    }
    base.OnFormClosing(e);
  }

  private void protectionListLabel_CheckedChanged(object sender, EventArgs e)
  {
    this.selectedProtectionList = this.GetIndex(sender as Control);
    this.UpdateTextBoxes();
    this.UpdateSendButton();
  }

  private void UpdateTextBoxes()
  {
    if (!this.Online)
    {
      this.Close();
    }
    else
    {
      bool flag = false;
      try
      {
        if (this.passwordManager.ListStatus(this.selectedProtectionList) == 1)
          flag = true;
      }
      catch (CaesarException ex)
      {
      }
      ((Control) this.oldPasswordTextBox).Enabled = flag;
      ((Control) this.oldPasswordTextBox).Text = string.Empty;
      ((Control) this.newPasswordTextBox).Text = string.Empty;
    }
  }

  private void backdoorButton_Click(object sender, EventArgs e)
  {
    if (!this.Online)
    {
      this.Close();
    }
    else
    {
      try
      {
        int num = (int) ((Form) new PasswordBackdoorDialog(this.passwordManager, this.passwordManager.ReadBackdoorSeed(), (Action) (() => Link.ShowTarget(Link.AvailableLinks.GetLinkOrEmpty("Form_PasswordBackdoorDialog"))), true)).ShowDialog();
        this.UpdateTextBoxes();
        this.UpdateSendButton();
      }
      catch (CaesarException ex)
      {
        int num = (int) ControlHelpers.ShowMessageBox((Control) this, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormatErrorsOccurredDuringRetrieval, (object) ex.Message), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
      }
    }
  }

  private void oldPasswordTextBox_TextChanged(object sender, EventArgs e)
  {
    this.UpdateSendButton();
  }

  private void UpdateSendButton()
  {
    this.sendButton.Enabled = !((Control) this.oldPasswordTextBox).Enabled || ((Control) this.oldPasswordTextBox).Text.Length > 0;
  }

  private void OnHelpButtonClicked(object sender, CancelEventArgs e)
  {
    e.Cancel = true;
    Link.ShowTarget(Link.AvailableLinks.GetLinkOrEmpty("Form_PasswordConfigureDialog"));
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (PasswordConfigureDialog));
    this.closeButton = new Button();
    this.groupBox1 = new GroupBox();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.label3 = new System.Windows.Forms.Label();
    this.sendButton = new Button();
    this.entryPanel = new Panel();
    this.protectionList1Label = new RadioButton();
    this.protectionList2Label = new RadioButton();
    this.protectionList3Label = new RadioButton();
    this.protectionList4Label = new RadioButton();
    this.protectionList5Label = new RadioButton();
    this.protectionList6Label = new RadioButton();
    this.protectionList7Label = new RadioButton();
    this.newPasswordTextBox = new PasswordBox();
    this.oldPasswordTextBox = new PasswordBox();
    this.label2 = new System.Windows.Forms.Label();
    this.label1 = new System.Windows.Forms.Label();
    this.backdoorButton = new Button();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.groupBox1.SuspendLayout();
    this.tableLayoutPanel1.SuspendLayout();
    this.entryPanel.SuspendLayout();
    this.tableLayoutPanel2.SuspendLayout();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) this.closeButton, "closeButton");
    this.closeButton.DialogResult = DialogResult.OK;
    this.closeButton.Name = "closeButton";
    this.closeButton.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.groupBox1, "groupBox1");
    this.groupBox1.Controls.Add((Control) this.tableLayoutPanel1);
    this.groupBox1.Name = "groupBox1";
    this.groupBox1.TabStop = false;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    this.tableLayoutPanel1.Controls.Add((Control) this.label3, 0, 0);
    this.tableLayoutPanel1.Controls.Add((Control) this.sendButton, 4, 2);
    this.tableLayoutPanel1.Controls.Add((Control) this.entryPanel, 0, 1);
    this.tableLayoutPanel1.Controls.Add((Control) this.newPasswordTextBox, 3, 2);
    this.tableLayoutPanel1.Controls.Add((Control) this.oldPasswordTextBox, 1, 2);
    this.tableLayoutPanel1.Controls.Add((Control) this.label2, 2, 2);
    this.tableLayoutPanel1.Controls.Add((Control) this.label1, 0, 2);
    this.tableLayoutPanel1.Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.label3, "label3");
    this.tableLayoutPanel1.SetColumnSpan((Control) this.label3, 3);
    this.label3.Name = "label3";
    componentResourceManager.ApplyResources((object) this.sendButton, "sendButton");
    this.sendButton.Name = "sendButton";
    this.sendButton.UseVisualStyleBackColor = true;
    this.sendButton.Click += new EventHandler(this.sendButton_Click);
    this.tableLayoutPanel1.SetColumnSpan((Control) this.entryPanel, 5);
    this.entryPanel.Controls.Add((Control) this.protectionList1Label);
    this.entryPanel.Controls.Add((Control) this.protectionList2Label);
    this.entryPanel.Controls.Add((Control) this.protectionList3Label);
    this.entryPanel.Controls.Add((Control) this.protectionList4Label);
    this.entryPanel.Controls.Add((Control) this.protectionList5Label);
    this.entryPanel.Controls.Add((Control) this.protectionList6Label);
    this.entryPanel.Controls.Add((Control) this.protectionList7Label);
    componentResourceManager.ApplyResources((object) this.entryPanel, "entryPanel");
    this.entryPanel.Name = "entryPanel";
    componentResourceManager.ApplyResources((object) this.protectionList1Label, "protectionList1Label");
    this.protectionList1Label.Name = "protectionList1Label";
    this.protectionList1Label.CheckedChanged += new EventHandler(this.protectionListLabel_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.protectionList2Label, "protectionList2Label");
    this.protectionList2Label.Name = "protectionList2Label";
    this.protectionList2Label.CheckedChanged += new EventHandler(this.protectionListLabel_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.protectionList3Label, "protectionList3Label");
    this.protectionList3Label.Name = "protectionList3Label";
    this.protectionList3Label.CheckedChanged += new EventHandler(this.protectionListLabel_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.protectionList4Label, "protectionList4Label");
    this.protectionList4Label.Name = "protectionList4Label";
    this.protectionList4Label.CheckedChanged += new EventHandler(this.protectionListLabel_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.protectionList5Label, "protectionList5Label");
    this.protectionList5Label.Name = "protectionList5Label";
    this.protectionList5Label.CheckedChanged += new EventHandler(this.protectionListLabel_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.protectionList6Label, "protectionList6Label");
    this.protectionList6Label.Name = "protectionList6Label";
    this.protectionList6Label.CheckedChanged += new EventHandler(this.protectionListLabel_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.protectionList7Label, "protectionList7Label");
    this.protectionList7Label.Name = "protectionList7Label";
    this.protectionList7Label.CheckedChanged += new EventHandler(this.protectionListLabel_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.newPasswordTextBox, "newPasswordTextBox");
    ((Control) this.newPasswordTextBox).Name = "newPasswordTextBox";
    componentResourceManager.ApplyResources((object) this.oldPasswordTextBox, "oldPasswordTextBox");
    ((Control) this.oldPasswordTextBox).Name = "oldPasswordTextBox";
    ((Control) this.oldPasswordTextBox).TextChanged += new EventHandler(this.oldPasswordTextBox_TextChanged);
    componentResourceManager.ApplyResources((object) this.label2, "label2");
    this.label2.Name = "label2";
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    componentResourceManager.ApplyResources((object) this.backdoorButton, "backdoorButton");
    this.backdoorButton.Name = "backdoorButton";
    this.backdoorButton.UseVisualStyleBackColor = true;
    this.backdoorButton.Click += new EventHandler(this.backdoorButton_Click);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    this.tableLayoutPanel2.Controls.Add((Control) this.backdoorButton, 1, 0);
    this.tableLayoutPanel2.Controls.Add((Control) this.closeButton, 2, 0);
    this.tableLayoutPanel2.Name = "tableLayoutPanel2";
    this.AcceptButton = (IButtonControl) this.closeButton;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.Controls.Add((Control) this.groupBox1);
    this.Controls.Add((Control) this.tableLayoutPanel2);
    this.FormBorderStyle = FormBorderStyle.FixedDialog;
    this.HelpButton = true;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (PasswordConfigureDialog);
    this.ShowInTaskbar = false;
    this.HelpButtonClicked += new CancelEventHandler(this.OnHelpButtonClicked);
    this.groupBox1.ResumeLayout(false);
    this.tableLayoutPanel1.ResumeLayout(false);
    this.tableLayoutPanel1.PerformLayout();
    this.entryPanel.ResumeLayout(false);
    this.entryPanel.PerformLayout();
    this.tableLayoutPanel2.ResumeLayout(false);
    this.tableLayoutPanel2.PerformLayout();
    this.ResumeLayout(false);
    this.PerformLayout();
  }
}
