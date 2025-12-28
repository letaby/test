using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;

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
		InitializeComponent();
		base.HeaderImage = new Bitmap(GetType(), "option_server.png");
	}

	private void OnServerChanged(object sender, EventArgs e)
	{
		if (textBoxServer.Text != NetworkSettings.GlobalInstance.Server)
		{
			MarkDirty();
		}
	}

	private void OnPortChanged(object sender, EventArgs e)
	{
		if (textBoxPort.Text != NetworkSettings.GlobalInstance.Port.ToString(CultureInfo.CurrentCulture))
		{
			MarkDirty();
		}
	}

	private void OnLogonLocationChanged(object sender, EventArgs e)
	{
		if (NetworkSettings.GlobalInstance.LogOnLocation == null || textBoxLogonLocation.Text != NetworkSettings.GlobalInstance.LogOnLocation.ToString(CultureInfo.CurrentCulture))
		{
			MarkDirty();
		}
	}

	private void textBoxTechlaneLocation_TextChanged(object sender, EventArgs e)
	{
		if (textBoxTechlaneLocation.Text != NetworkSettings.GlobalInstance.TechlaneLocation)
		{
			MarkDirty();
		}
	}

	private void textBoxEdexLocation_TextChanged(object sender, EventArgs e)
	{
		if (textBoxEdexLocation.Text != NetworkSettings.GlobalInstance.EdexServiceLocation)
		{
			MarkDirty();
		}
	}

	private void textBoxEdexFileStoreLocation_TextChanged(object sender, EventArgs e)
	{
		if (textBoxEdexFileStoreLocation.Text != NetworkSettings.GlobalInstance.EdexFileStoreLocation)
		{
			MarkDirty();
		}
	}

	private void UpdateControlsUserInterface()
	{
		if (base.IsDirty)
		{
			UpdateServerDetails();
		}
	}

	private void UpdateServerDetails()
	{
		checkBoxUseOidc.Checked = NetworkSettings.GlobalInstance.UseOidc;
		checkBoxUseOidc.Visible = false;
		textBoxServer.Text = NetworkSettings.GlobalInstance.DLBrokerServer;
		textBoxPort.Text = NetworkSettings.GlobalInstance.DLBrokerPort.ToString(CultureInfo.CurrentCulture);
		textBoxLogonLocation.Text = NetworkSettings.GlobalInstance.LogOnLocation;
		textBoxTechlaneLocation.Text = NetworkSettings.GlobalInstance.TechlaneLocation;
		textBoxEdexLocation.Text = NetworkSettings.GlobalInstance.EdexServiceLocation;
		textBoxEdexFileStoreLocation.Text = NetworkSettings.GlobalInstance.EdexFileStoreLocation;
		Label label = labelEdexFileStore;
		bool visible = (textBoxEdexFileStoreLocation.Visible = false);
		label.Visible = visible;
		buttonDeauthorize.Enabled = true;
		checkBoxSaveUploadContent.Visible = ApplicationInformation.CanOverrideSaveUploadContent;
		checkBoxSaveUploadContent.Checked = NetworkSettings.GlobalInstance.SaveUploadContent;
		checkBoxSaveRawServerFiles.Visible = false;
		checkBoxSaveRawServerFiles.Checked = NetworkSettings.GlobalInstance.SaveRawServerFiles;
		checkBoxUseManualUnlockDialog.Checked = NetworkSettings.GlobalInstance.UseManualUnlockDialog;
		checkBoxAutomaticTroubleshootingMaterialDownload.Checked = NetworkSettings.GlobalInstance.AutomaticallyDownloadTroubleshootingMaterial;
	}

	protected override void OnLoad(EventArgs e)
	{
		UpdateServerDetails();
		UpdateControlsUserInterface();
		base.OnLoad(e);
	}

	public override bool ApplySettings()
	{
		bool flag = true;
		ushort dLBrokerPort = 0;
		if (base.IsDirty)
		{
			try
			{
				dLBrokerPort = Convert.ToUInt16(textBoxPort.Text, CultureInfo.CurrentCulture);
			}
			catch (OverflowException)
			{
				MessageBox.Show(Resources.MessageInvalidPort, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, ControlHelpers.GetRightToLeftOptions((Control)this));
				flag = false;
			}
			catch (FormatException)
			{
				MessageBox.Show(Resources.MessageInvalidPort, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, ControlHelpers.GetRightToLeftOptions((Control)this));
				flag = false;
			}
			NetworkSettings.GlobalInstance.DLBrokerServer = textBoxServer.Text.Trim();
			NetworkSettings.GlobalInstance.DLBrokerPort = dLBrokerPort;
			NetworkSettings.GlobalInstance.LogOnLocation = textBoxLogonLocation.Text.Trim();
			NetworkSettings.GlobalInstance.TechlaneLocation = textBoxTechlaneLocation.Text.Trim();
			NetworkSettings.GlobalInstance.EdexServiceLocation = textBoxEdexLocation.Text.Trim();
			NetworkSettings.GlobalInstance.EdexFileStoreLocation = textBoxEdexFileStoreLocation.Text.Trim();
			NetworkSettings.GlobalInstance.SaveUploadContent = checkBoxSaveUploadContent.Checked;
			NetworkSettings.GlobalInstance.SaveRawServerFiles = checkBoxSaveRawServerFiles.Checked;
			NetworkSettings.GlobalInstance.UseManualUnlockDialog = checkBoxUseManualUnlockDialog.Checked;
			NetworkSettings.GlobalInstance.AutomaticallyDownloadTroubleshootingMaterial = checkBoxAutomaticTroubleshootingMaterialDownload.Checked;
			ServerClient.GlobalInstance.ClearPassword();
		}
		if (flag)
		{
			flag = base.ApplySettings();
		}
		return flag;
	}

	private void buttonDeauthorize_Click(object sender, EventArgs e)
	{
		if (!ServerClient.GlobalInstance.InUse && ControlHelpers.ShowMessageBox(Resources.ServerOptionsPanel_Deauthorize, MessageBoxButtons.OKCancel, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1) == DialogResult.OK)
		{
			DeauthorizeClient();
		}
	}

	internal static void DeauthorizeClient()
	{
		File.Delete(FileEncryptionProvider.EncryptFileName(Directories.DrumrollToolLicenseFile));
		SapiManager.GlobalInstance.RegistrationKey = "deregister";
		ServerClient.GlobalInstance.Go((Collection<UnitInformation>)null, (Collection<UnitInformation>)null);
	}

	private void checkBoxUploadData_CheckedChanged(object sender, EventArgs e)
	{
		if (checkBoxSaveUploadContent.Checked != NetworkSettings.GlobalInstance.SaveUploadContent)
		{
			MarkDirty();
		}
	}

	private void checkBoxSaveRawServerFiles_CheckedChanged(object sender, EventArgs e)
	{
		if (checkBoxSaveRawServerFiles.Checked != NetworkSettings.GlobalInstance.SaveRawServerFiles)
		{
			MarkDirty();
		}
	}

	private void checkBoxUseManualUnlockDialog_CheckedChanged(object sender, EventArgs e)
	{
		if (checkBoxUseManualUnlockDialog.Checked != NetworkSettings.GlobalInstance.UseManualUnlockDialog)
		{
			MarkDirty();
		}
	}

	private void checkBoxAutomaticTroubleshootingMaterialDownload_CheckedChanged(object sender, EventArgs e)
	{
		if (checkBoxAutomaticTroubleshootingMaterialDownload.Checked != NetworkSettings.GlobalInstance.AutomaticallyDownloadTroubleshootingMaterial)
		{
			MarkDirty();
		}
	}

	private void buttonTest_Click(object sender, EventArgs e)
	{
		new NetworkDebugger().ShowDialog(this);
	}

	private void checkBoxUseOidc_CheckedChanged(object sender, EventArgs e)
	{
		if (checkBoxUseOidc.Checked != NetworkSettings.GlobalInstance.UseOidc)
		{
			MarkDirty();
			NetworkSettings.GlobalInstance.UseOidc = checkBoxUseOidc.Checked;
			UpdateServerDetails();
		}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Container.ServerOptionsPanel));
		this.labelPortNumber = new System.Windows.Forms.Label();
		this.textBoxServer = new System.Windows.Forms.TextBox();
		this.textBoxPort = new System.Windows.Forms.TextBox();
		this.labelServer = new System.Windows.Forms.Label();
		this.buttonDeauthorize = new System.Windows.Forms.Button();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.textBoxEdexFileStoreLocation = new System.Windows.Forms.TextBox();
		this.labelEdexFileStore = new System.Windows.Forms.Label();
		this.textBoxEdexLocation = new System.Windows.Forms.TextBox();
		this.label3 = new System.Windows.Forms.Label();
		this.buttonTest = new System.Windows.Forms.Button();
		this.textBoxTechlaneLocation = new System.Windows.Forms.TextBox();
		this.label2 = new System.Windows.Forms.Label();
		this.textBoxLogonLocation = new System.Windows.Forms.TextBox();
		this.label1 = new System.Windows.Forms.Label();
		this.checkBoxAutomaticTroubleshootingMaterialDownload = new System.Windows.Forms.CheckBox();
		this.checkBoxSaveRawServerFiles = new System.Windows.Forms.CheckBox();
		this.checkBoxSaveUploadContent = new System.Windows.Forms.CheckBox();
		this.checkBoxUseManualUnlockDialog = new System.Windows.Forms.CheckBox();
		this.checkBoxUseOidc = new System.Windows.Forms.CheckBox();
		this.tableLayoutPanel1.SuspendLayout();
		base.SuspendLayout();
		resources.ApplyResources(this.labelPortNumber, "labelPortNumber");
		this.labelPortNumber.Name = "labelPortNumber";
		resources.ApplyResources(this.textBoxServer, "textBoxServer");
		this.textBoxServer.Name = "textBoxServer";
		this.textBoxServer.TextChanged += new System.EventHandler(OnServerChanged);
		resources.ApplyResources(this.textBoxPort, "textBoxPort");
		this.textBoxPort.Name = "textBoxPort";
		this.textBoxPort.TextChanged += new System.EventHandler(OnPortChanged);
		resources.ApplyResources(this.labelServer, "labelServer");
		this.labelServer.Name = "labelServer";
		resources.ApplyResources(this.buttonDeauthorize, "buttonDeauthorize");
		this.buttonDeauthorize.Name = "buttonDeauthorize";
		this.buttonDeauthorize.UseVisualStyleBackColor = true;
		this.buttonDeauthorize.Click += new System.EventHandler(buttonDeauthorize_Click);
		resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
		this.tableLayoutPanel1.Controls.Add(this.textBoxEdexFileStoreLocation, 1, 4);
		this.tableLayoutPanel1.Controls.Add(this.labelEdexFileStore, 0, 4);
		this.tableLayoutPanel1.Controls.Add(this.textBoxEdexLocation, 1, 3);
		this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
		this.tableLayoutPanel1.Controls.Add(this.buttonTest, 1, 6);
		this.tableLayoutPanel1.Controls.Add(this.textBoxTechlaneLocation, 1, 5);
		this.tableLayoutPanel1.Controls.Add(this.label2, 0, 5);
		this.tableLayoutPanel1.Controls.Add(this.textBoxLogonLocation, 1, 2);
		this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
		this.tableLayoutPanel1.Controls.Add(this.labelServer, 0, 0);
		this.tableLayoutPanel1.Controls.Add(this.textBoxServer, 1, 0);
		this.tableLayoutPanel1.Controls.Add(this.textBoxPort, 1, 1);
		this.tableLayoutPanel1.Controls.Add(this.labelPortNumber, 0, 1);
		this.tableLayoutPanel1.Controls.Add(this.checkBoxAutomaticTroubleshootingMaterialDownload, 0, 10);
		this.tableLayoutPanel1.Controls.Add(this.checkBoxSaveRawServerFiles, 0, 9);
		this.tableLayoutPanel1.Controls.Add(this.checkBoxSaveUploadContent, 0, 8);
		this.tableLayoutPanel1.Controls.Add(this.checkBoxUseManualUnlockDialog, 0, 7);
		this.tableLayoutPanel1.Controls.Add(this.checkBoxUseOidc, 2, 0);
		this.tableLayoutPanel1.Controls.Add(this.buttonDeauthorize, 0, 6);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.SetColumnSpan(this.textBoxEdexFileStoreLocation, 2);
		resources.ApplyResources(this.textBoxEdexFileStoreLocation, "textBoxEdexFileStoreLocation");
		this.textBoxEdexFileStoreLocation.Name = "textBoxEdexFileStoreLocation";
		this.textBoxEdexFileStoreLocation.TextChanged += new System.EventHandler(textBoxEdexFileStoreLocation_TextChanged);
		resources.ApplyResources(this.labelEdexFileStore, "labelEdexFileStore");
		this.labelEdexFileStore.Name = "labelEdexFileStore";
		this.tableLayoutPanel1.SetColumnSpan(this.textBoxEdexLocation, 2);
		resources.ApplyResources(this.textBoxEdexLocation, "textBoxEdexLocation");
		this.textBoxEdexLocation.Name = "textBoxEdexLocation";
		this.textBoxEdexLocation.TextChanged += new System.EventHandler(textBoxEdexLocation_TextChanged);
		resources.ApplyResources(this.label3, "label3");
		this.label3.Name = "label3";
		resources.ApplyResources(this.buttonTest, "buttonTest");
		this.buttonTest.Name = "buttonTest";
		this.buttonTest.UseVisualStyleBackColor = true;
		this.buttonTest.Click += new System.EventHandler(buttonTest_Click);
		this.tableLayoutPanel1.SetColumnSpan(this.textBoxTechlaneLocation, 2);
		resources.ApplyResources(this.textBoxTechlaneLocation, "textBoxTechlaneLocation");
		this.textBoxTechlaneLocation.Name = "textBoxTechlaneLocation";
		this.textBoxTechlaneLocation.TextChanged += new System.EventHandler(textBoxTechlaneLocation_TextChanged);
		resources.ApplyResources(this.label2, "label2");
		this.label2.Name = "label2";
		this.tableLayoutPanel1.SetColumnSpan(this.textBoxLogonLocation, 2);
		resources.ApplyResources(this.textBoxLogonLocation, "textBoxLogonLocation");
		this.textBoxLogonLocation.Name = "textBoxLogonLocation";
		this.textBoxLogonLocation.TextChanged += new System.EventHandler(OnLogonLocationChanged);
		resources.ApplyResources(this.label1, "label1");
		this.label1.Name = "label1";
		resources.ApplyResources(this.checkBoxAutomaticTroubleshootingMaterialDownload, "checkBoxAutomaticTroubleshootingMaterialDownload");
		this.tableLayoutPanel1.SetColumnSpan(this.checkBoxAutomaticTroubleshootingMaterialDownload, 3);
		this.checkBoxAutomaticTroubleshootingMaterialDownload.Name = "checkBoxAutomaticTroubleshootingMaterialDownload";
		this.checkBoxAutomaticTroubleshootingMaterialDownload.UseVisualStyleBackColor = true;
		this.checkBoxAutomaticTroubleshootingMaterialDownload.CheckedChanged += new System.EventHandler(checkBoxAutomaticTroubleshootingMaterialDownload_CheckedChanged);
		resources.ApplyResources(this.checkBoxSaveRawServerFiles, "checkBoxSaveRawServerFiles");
		this.tableLayoutPanel1.SetColumnSpan(this.checkBoxSaveRawServerFiles, 3);
		this.checkBoxSaveRawServerFiles.Name = "checkBoxSaveRawServerFiles";
		this.checkBoxSaveRawServerFiles.UseVisualStyleBackColor = true;
		this.checkBoxSaveRawServerFiles.CheckedChanged += new System.EventHandler(checkBoxSaveRawServerFiles_CheckedChanged);
		resources.ApplyResources(this.checkBoxSaveUploadContent, "checkBoxSaveUploadContent");
		this.tableLayoutPanel1.SetColumnSpan(this.checkBoxSaveUploadContent, 3);
		this.checkBoxSaveUploadContent.Name = "checkBoxSaveUploadContent";
		this.checkBoxSaveUploadContent.UseVisualStyleBackColor = true;
		this.checkBoxSaveUploadContent.CheckedChanged += new System.EventHandler(checkBoxUploadData_CheckedChanged);
		resources.ApplyResources(this.checkBoxUseManualUnlockDialog, "checkBoxUseManualUnlockDialog");
		this.tableLayoutPanel1.SetColumnSpan(this.checkBoxUseManualUnlockDialog, 3);
		this.checkBoxUseManualUnlockDialog.Name = "checkBoxUseManualUnlockDialog";
		this.checkBoxUseManualUnlockDialog.UseVisualStyleBackColor = true;
		this.checkBoxUseManualUnlockDialog.CheckedChanged += new System.EventHandler(checkBoxUseManualUnlockDialog_CheckedChanged);
		resources.ApplyResources(this.checkBoxUseOidc, "checkBoxUseOidc");
		this.checkBoxUseOidc.Name = "checkBoxUseOidc";
		this.checkBoxUseOidc.UseVisualStyleBackColor = true;
		this.checkBoxUseOidc.CheckedChanged += new System.EventHandler(checkBoxUseOidc_CheckedChanged);
		resources.ApplyResources(this, "$this");
		base.Controls.Add(this.tableLayoutPanel1);
		base.Name = "ServerOptionsPanel";
		base.Controls.SetChildIndex(this.tableLayoutPanel1, 0);
		this.tableLayoutPanel1.ResumeLayout(false);
		this.tableLayoutPanel1.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
