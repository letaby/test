// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.ServerOptionsPanel
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal class ServerOptionsPanel : OptionsPanel
{
  private const string DeregisterKey = "deregister";
  private IContainer components;
  private Label labelPortNumber;
  private TextBox textBoxServer;
  private TextBox textBoxPort;
  private Label labelServer;
  private Button buttonDeauthorize;
  private TableLayoutPanel tableLayoutPanel1;
  private CheckBox checkBoxSaveUploadContent;
  private CheckBox checkBoxSaveRawServerFiles;
  private CheckBox checkBoxUseManualUnlockDialog;
  private CheckBox checkBoxAutomaticTroubleshootingMaterialDownload;
  private TextBox textBoxLogonLocation;
  private Label label1;
  private TextBox textBoxTechlaneLocation;
  private Label label2;
  private Button buttonTest;
  private TextBox textBoxEdexLocation;
  private Label label3;
  private TextBox textBoxEdexFileStoreLocation;
  private Label labelEdexFileStore;
  private CheckBox checkBoxUseOidc;

  public ServerOptionsPanel()
  {
    this.InitializeComponent();
    this.HeaderImage = (Image) new Bitmap(this.GetType(), "option_server.png");
  }

  private void OnServerChanged(object sender, EventArgs e)
  {
    if (!(this.textBoxServer.Text != NetworkSettings.GlobalInstance.Server))
      return;
    this.MarkDirty();
  }

  private void OnPortChanged(object sender, EventArgs e)
  {
    if (!(this.textBoxPort.Text != NetworkSettings.GlobalInstance.Port.ToString((IFormatProvider) CultureInfo.CurrentCulture)))
      return;
    this.MarkDirty();
  }

  private void OnLogonLocationChanged(object sender, EventArgs e)
  {
    if (NetworkSettings.GlobalInstance.LogOnLocation != null && !(this.textBoxLogonLocation.Text != NetworkSettings.GlobalInstance.LogOnLocation.ToString((IFormatProvider) CultureInfo.CurrentCulture)))
      return;
    this.MarkDirty();
  }

  private void textBoxTechlaneLocation_TextChanged(object sender, EventArgs e)
  {
    if (!(this.textBoxTechlaneLocation.Text != NetworkSettings.GlobalInstance.TechlaneLocation))
      return;
    this.MarkDirty();
  }

  private void textBoxEdexLocation_TextChanged(object sender, EventArgs e)
  {
    if (!(this.textBoxEdexLocation.Text != NetworkSettings.GlobalInstance.EdexServiceLocation))
      return;
    this.MarkDirty();
  }

  private void textBoxEdexFileStoreLocation_TextChanged(object sender, EventArgs e)
  {
    if (!(this.textBoxEdexFileStoreLocation.Text != NetworkSettings.GlobalInstance.EdexFileStoreLocation))
      return;
    this.MarkDirty();
  }

  private void UpdateControlsUserInterface()
  {
    if (!this.IsDirty)
      return;
    this.UpdateServerDetails();
  }

  private void UpdateServerDetails()
  {
    this.checkBoxUseOidc.Checked = NetworkSettings.GlobalInstance.UseOidc;
    this.checkBoxUseOidc.Visible = false;
    this.textBoxServer.Text = NetworkSettings.GlobalInstance.DLBrokerServer;
    this.textBoxPort.Text = NetworkSettings.GlobalInstance.DLBrokerPort.ToString((IFormatProvider) CultureInfo.CurrentCulture);
    this.textBoxLogonLocation.Text = NetworkSettings.GlobalInstance.LogOnLocation;
    this.textBoxTechlaneLocation.Text = NetworkSettings.GlobalInstance.TechlaneLocation;
    this.textBoxEdexLocation.Text = NetworkSettings.GlobalInstance.EdexServiceLocation;
    this.textBoxEdexFileStoreLocation.Text = NetworkSettings.GlobalInstance.EdexFileStoreLocation;
    this.labelEdexFileStore.Visible = this.textBoxEdexFileStoreLocation.Visible = false;
    this.buttonDeauthorize.Enabled = true;
    this.checkBoxSaveUploadContent.Visible = ApplicationInformation.CanOverrideSaveUploadContent;
    this.checkBoxSaveUploadContent.Checked = NetworkSettings.GlobalInstance.SaveUploadContent;
    this.checkBoxSaveRawServerFiles.Visible = false;
    this.checkBoxSaveRawServerFiles.Checked = NetworkSettings.GlobalInstance.SaveRawServerFiles;
    this.checkBoxUseManualUnlockDialog.Checked = NetworkSettings.GlobalInstance.UseManualUnlockDialog;
    this.checkBoxAutomaticTroubleshootingMaterialDownload.Checked = NetworkSettings.GlobalInstance.AutomaticallyDownloadTroubleshootingMaterial;
  }

  protected override void OnLoad(EventArgs e)
  {
    this.UpdateServerDetails();
    this.UpdateControlsUserInterface();
    base.OnLoad(e);
  }

  public override bool ApplySettings()
  {
    bool flag = true;
    ushort num1 = 0;
    if (this.IsDirty)
    {
      try
      {
        num1 = Convert.ToUInt16(this.textBoxPort.Text, (IFormatProvider) CultureInfo.CurrentCulture);
      }
      catch (OverflowException ex)
      {
        int num2 = (int) MessageBox.Show(Resources.MessageInvalidPort, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, ControlHelpers.GetRightToLeftOptions((Control) this));
        flag = false;
      }
      catch (FormatException ex)
      {
        int num3 = (int) MessageBox.Show(Resources.MessageInvalidPort, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, ControlHelpers.GetRightToLeftOptions((Control) this));
        flag = false;
      }
      NetworkSettings.GlobalInstance.DLBrokerServer = this.textBoxServer.Text.Trim();
      NetworkSettings.GlobalInstance.DLBrokerPort = num1;
      NetworkSettings.GlobalInstance.LogOnLocation = this.textBoxLogonLocation.Text.Trim();
      NetworkSettings.GlobalInstance.TechlaneLocation = this.textBoxTechlaneLocation.Text.Trim();
      NetworkSettings.GlobalInstance.EdexServiceLocation = this.textBoxEdexLocation.Text.Trim();
      NetworkSettings.GlobalInstance.EdexFileStoreLocation = this.textBoxEdexFileStoreLocation.Text.Trim();
      NetworkSettings.GlobalInstance.SaveUploadContent = this.checkBoxSaveUploadContent.Checked;
      NetworkSettings.GlobalInstance.SaveRawServerFiles = this.checkBoxSaveRawServerFiles.Checked;
      NetworkSettings.GlobalInstance.UseManualUnlockDialog = this.checkBoxUseManualUnlockDialog.Checked;
      NetworkSettings.GlobalInstance.AutomaticallyDownloadTroubleshootingMaterial = this.checkBoxAutomaticTroubleshootingMaterialDownload.Checked;
      ServerClient.GlobalInstance.ClearPassword();
    }
    if (flag)
      flag = base.ApplySettings();
    return flag;
  }

  private void buttonDeauthorize_Click(object sender, EventArgs e)
  {
    if (ServerClient.GlobalInstance.InUse || ControlHelpers.ShowMessageBox(Resources.ServerOptionsPanel_Deauthorize, MessageBoxButtons.OKCancel, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1) != DialogResult.OK)
      return;
    ServerOptionsPanel.DeauthorizeClient();
  }

  internal static void DeauthorizeClient()
  {
    File.Delete(FileEncryptionProvider.EncryptFileName(Directories.DrumrollToolLicenseFile));
    SapiManager.GlobalInstance.RegistrationKey = "deregister";
    ServerClient.GlobalInstance.Go((Collection<UnitInformation>) null, (Collection<UnitInformation>) null);
  }

  private void checkBoxUploadData_CheckedChanged(object sender, EventArgs e)
  {
    if (this.checkBoxSaveUploadContent.Checked == NetworkSettings.GlobalInstance.SaveUploadContent)
      return;
    this.MarkDirty();
  }

  private void checkBoxSaveRawServerFiles_CheckedChanged(object sender, EventArgs e)
  {
    if (this.checkBoxSaveRawServerFiles.Checked == NetworkSettings.GlobalInstance.SaveRawServerFiles)
      return;
    this.MarkDirty();
  }

  private void checkBoxUseManualUnlockDialog_CheckedChanged(object sender, EventArgs e)
  {
    if (this.checkBoxUseManualUnlockDialog.Checked == NetworkSettings.GlobalInstance.UseManualUnlockDialog)
      return;
    this.MarkDirty();
  }

  private void checkBoxAutomaticTroubleshootingMaterialDownload_CheckedChanged(
    object sender,
    EventArgs e)
  {
    if (this.checkBoxAutomaticTroubleshootingMaterialDownload.Checked == NetworkSettings.GlobalInstance.AutomaticallyDownloadTroubleshootingMaterial)
      return;
    this.MarkDirty();
  }

  private void buttonTest_Click(object sender, EventArgs e)
  {
    int num = (int) new NetworkDebugger().ShowDialog((IWin32Window) this);
  }

  private void checkBoxUseOidc_CheckedChanged(object sender, EventArgs e)
  {
    if (this.checkBoxUseOidc.Checked == NetworkSettings.GlobalInstance.UseOidc)
      return;
    this.MarkDirty();
    NetworkSettings.GlobalInstance.UseOidc = this.checkBoxUseOidc.Checked;
    this.UpdateServerDetails();
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ServerOptionsPanel));
    this.labelPortNumber = new Label();
    this.textBoxServer = new TextBox();
    this.textBoxPort = new TextBox();
    this.labelServer = new Label();
    this.buttonDeauthorize = new Button();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.textBoxEdexFileStoreLocation = new TextBox();
    this.labelEdexFileStore = new Label();
    this.textBoxEdexLocation = new TextBox();
    this.label3 = new Label();
    this.buttonTest = new Button();
    this.textBoxTechlaneLocation = new TextBox();
    this.label2 = new Label();
    this.textBoxLogonLocation = new TextBox();
    this.label1 = new Label();
    this.checkBoxAutomaticTroubleshootingMaterialDownload = new CheckBox();
    this.checkBoxSaveRawServerFiles = new CheckBox();
    this.checkBoxSaveUploadContent = new CheckBox();
    this.checkBoxUseManualUnlockDialog = new CheckBox();
    this.checkBoxUseOidc = new CheckBox();
    this.tableLayoutPanel1.SuspendLayout();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) this.labelPortNumber, "labelPortNumber");
    this.labelPortNumber.Name = "labelPortNumber";
    componentResourceManager.ApplyResources((object) this.textBoxServer, "textBoxServer");
    this.textBoxServer.Name = "textBoxServer";
    this.textBoxServer.TextChanged += new EventHandler(this.OnServerChanged);
    componentResourceManager.ApplyResources((object) this.textBoxPort, "textBoxPort");
    this.textBoxPort.Name = "textBoxPort";
    this.textBoxPort.TextChanged += new EventHandler(this.OnPortChanged);
    componentResourceManager.ApplyResources((object) this.labelServer, "labelServer");
    this.labelServer.Name = "labelServer";
    componentResourceManager.ApplyResources((object) this.buttonDeauthorize, "buttonDeauthorize");
    this.buttonDeauthorize.Name = "buttonDeauthorize";
    this.buttonDeauthorize.UseVisualStyleBackColor = true;
    this.buttonDeauthorize.Click += new EventHandler(this.buttonDeauthorize_Click);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    this.tableLayoutPanel1.Controls.Add((Control) this.textBoxEdexFileStoreLocation, 1, 4);
    this.tableLayoutPanel1.Controls.Add((Control) this.labelEdexFileStore, 0, 4);
    this.tableLayoutPanel1.Controls.Add((Control) this.textBoxEdexLocation, 1, 3);
    this.tableLayoutPanel1.Controls.Add((Control) this.label3, 0, 3);
    this.tableLayoutPanel1.Controls.Add((Control) this.buttonTest, 1, 6);
    this.tableLayoutPanel1.Controls.Add((Control) this.textBoxTechlaneLocation, 1, 5);
    this.tableLayoutPanel1.Controls.Add((Control) this.label2, 0, 5);
    this.tableLayoutPanel1.Controls.Add((Control) this.textBoxLogonLocation, 1, 2);
    this.tableLayoutPanel1.Controls.Add((Control) this.label1, 0, 2);
    this.tableLayoutPanel1.Controls.Add((Control) this.labelServer, 0, 0);
    this.tableLayoutPanel1.Controls.Add((Control) this.textBoxServer, 1, 0);
    this.tableLayoutPanel1.Controls.Add((Control) this.textBoxPort, 1, 1);
    this.tableLayoutPanel1.Controls.Add((Control) this.labelPortNumber, 0, 1);
    this.tableLayoutPanel1.Controls.Add((Control) this.checkBoxAutomaticTroubleshootingMaterialDownload, 0, 10);
    this.tableLayoutPanel1.Controls.Add((Control) this.checkBoxSaveRawServerFiles, 0, 9);
    this.tableLayoutPanel1.Controls.Add((Control) this.checkBoxSaveUploadContent, 0, 8);
    this.tableLayoutPanel1.Controls.Add((Control) this.checkBoxUseManualUnlockDialog, 0, 7);
    this.tableLayoutPanel1.Controls.Add((Control) this.checkBoxUseOidc, 2, 0);
    this.tableLayoutPanel1.Controls.Add((Control) this.buttonDeauthorize, 0, 6);
    this.tableLayoutPanel1.Name = "tableLayoutPanel1";
    this.tableLayoutPanel1.SetColumnSpan((Control) this.textBoxEdexFileStoreLocation, 2);
    componentResourceManager.ApplyResources((object) this.textBoxEdexFileStoreLocation, "textBoxEdexFileStoreLocation");
    this.textBoxEdexFileStoreLocation.Name = "textBoxEdexFileStoreLocation";
    this.textBoxEdexFileStoreLocation.TextChanged += new EventHandler(this.textBoxEdexFileStoreLocation_TextChanged);
    componentResourceManager.ApplyResources((object) this.labelEdexFileStore, "labelEdexFileStore");
    this.labelEdexFileStore.Name = "labelEdexFileStore";
    this.tableLayoutPanel1.SetColumnSpan((Control) this.textBoxEdexLocation, 2);
    componentResourceManager.ApplyResources((object) this.textBoxEdexLocation, "textBoxEdexLocation");
    this.textBoxEdexLocation.Name = "textBoxEdexLocation";
    this.textBoxEdexLocation.TextChanged += new EventHandler(this.textBoxEdexLocation_TextChanged);
    componentResourceManager.ApplyResources((object) this.label3, "label3");
    this.label3.Name = "label3";
    componentResourceManager.ApplyResources((object) this.buttonTest, "buttonTest");
    this.buttonTest.Name = "buttonTest";
    this.buttonTest.UseVisualStyleBackColor = true;
    this.buttonTest.Click += new EventHandler(this.buttonTest_Click);
    this.tableLayoutPanel1.SetColumnSpan((Control) this.textBoxTechlaneLocation, 2);
    componentResourceManager.ApplyResources((object) this.textBoxTechlaneLocation, "textBoxTechlaneLocation");
    this.textBoxTechlaneLocation.Name = "textBoxTechlaneLocation";
    this.textBoxTechlaneLocation.TextChanged += new EventHandler(this.textBoxTechlaneLocation_TextChanged);
    componentResourceManager.ApplyResources((object) this.label2, "label2");
    this.label2.Name = "label2";
    this.tableLayoutPanel1.SetColumnSpan((Control) this.textBoxLogonLocation, 2);
    componentResourceManager.ApplyResources((object) this.textBoxLogonLocation, "textBoxLogonLocation");
    this.textBoxLogonLocation.Name = "textBoxLogonLocation";
    this.textBoxLogonLocation.TextChanged += new EventHandler(this.OnLogonLocationChanged);
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    componentResourceManager.ApplyResources((object) this.checkBoxAutomaticTroubleshootingMaterialDownload, "checkBoxAutomaticTroubleshootingMaterialDownload");
    this.tableLayoutPanel1.SetColumnSpan((Control) this.checkBoxAutomaticTroubleshootingMaterialDownload, 3);
    this.checkBoxAutomaticTroubleshootingMaterialDownload.Name = "checkBoxAutomaticTroubleshootingMaterialDownload";
    this.checkBoxAutomaticTroubleshootingMaterialDownload.UseVisualStyleBackColor = true;
    this.checkBoxAutomaticTroubleshootingMaterialDownload.CheckedChanged += new EventHandler(this.checkBoxAutomaticTroubleshootingMaterialDownload_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.checkBoxSaveRawServerFiles, "checkBoxSaveRawServerFiles");
    this.tableLayoutPanel1.SetColumnSpan((Control) this.checkBoxSaveRawServerFiles, 3);
    this.checkBoxSaveRawServerFiles.Name = "checkBoxSaveRawServerFiles";
    this.checkBoxSaveRawServerFiles.UseVisualStyleBackColor = true;
    this.checkBoxSaveRawServerFiles.CheckedChanged += new EventHandler(this.checkBoxSaveRawServerFiles_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.checkBoxSaveUploadContent, "checkBoxSaveUploadContent");
    this.tableLayoutPanel1.SetColumnSpan((Control) this.checkBoxSaveUploadContent, 3);
    this.checkBoxSaveUploadContent.Name = "checkBoxSaveUploadContent";
    this.checkBoxSaveUploadContent.UseVisualStyleBackColor = true;
    this.checkBoxSaveUploadContent.CheckedChanged += new EventHandler(this.checkBoxUploadData_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.checkBoxUseManualUnlockDialog, "checkBoxUseManualUnlockDialog");
    this.tableLayoutPanel1.SetColumnSpan((Control) this.checkBoxUseManualUnlockDialog, 3);
    this.checkBoxUseManualUnlockDialog.Name = "checkBoxUseManualUnlockDialog";
    this.checkBoxUseManualUnlockDialog.UseVisualStyleBackColor = true;
    this.checkBoxUseManualUnlockDialog.CheckedChanged += new EventHandler(this.checkBoxUseManualUnlockDialog_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.checkBoxUseOidc, "checkBoxUseOidc");
    this.checkBoxUseOidc.Name = "checkBoxUseOidc";
    this.checkBoxUseOidc.UseVisualStyleBackColor = true;
    this.checkBoxUseOidc.CheckedChanged += new EventHandler(this.checkBoxUseOidc_CheckedChanged);
    componentResourceManager.ApplyResources((object) this, "$this");
    this.Controls.Add((Control) this.tableLayoutPanel1);
    this.Name = nameof (ServerOptionsPanel);
    this.Controls.SetChildIndex((Control) this.tableLayoutPanel1, 0);
    this.tableLayoutPanel1.ResumeLayout(false);
    this.tableLayoutPanel1.PerformLayout();
    this.ResumeLayout(false);
    this.PerformLayout();
  }
}
