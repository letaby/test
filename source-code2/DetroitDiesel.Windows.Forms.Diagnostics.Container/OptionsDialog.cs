using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Settings;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public class OptionsDialog : Form
{
	private Button buttonApply;

	private Button buttonCancel;

	private Button buttonOK;

	private TabControl tabControlOptions;

	private Panel tabControlPanel;

	private List<OptionsPanel> options = new List<OptionsPanel>();

	public OptionsDialog()
	{
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		AddTab(new FleetInfoOptionsPanel());
		AddTab(new DisplayOptionsPanel());
		AddTab(new ServerOptionsPanel());
		AddTab(new ConnectionOptionsPanel());
		AddTab(new SupportOptionsPanel());
		AddTab(new WarningsOptionsPanel());
	}

	internal OptionsDialog(OptionsPanel panel)
	{
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		AddTab(panel);
	}

	private void AddTab(OptionsPanel panel)
	{
		if (tabControlOptions.TabPages.ContainsKey(panel.Name))
		{
			throw new InvalidOperationException("Adding a panel that is already added");
		}
		tabControlOptions.SuspendLayout();
		options.Add(panel);
		tabControlOptions.TabPages.Add(panel.Name, panel.Text);
		tabControlOptions.TabPages[panel.Name].Controls.Add(panel);
		panel.Dock = DockStyle.Fill;
		panel.Visible = true;
		panel.Dirty += OnPanelDirty;
		panel.Clean += OnPanelClean;
		panel.Enabled = LicenseManager.GlobalInstance.AccessLevel >= panel.MinAccessLevel;
		tabControlOptions.ResumeLayout(performLayout: false);
	}

	private void OnPanelClean(object sender, EventArgs e)
	{
		UpdateButtons();
	}

	private void OnPanelDirty(object sender, EventArgs e)
	{
		UpdateButtons();
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Container.OptionsDialog));
		this.buttonApply = new System.Windows.Forms.Button();
		this.buttonCancel = new System.Windows.Forms.Button();
		this.buttonOK = new System.Windows.Forms.Button();
		this.tabControlOptions = new System.Windows.Forms.TabControl();
		this.tabControlPanel = new System.Windows.Forms.Panel();
		this.tabControlPanel.SuspendLayout();
		base.SuspendLayout();
		resources.ApplyResources(this.buttonApply, "buttonApply");
		this.buttonApply.Name = "buttonApply";
		this.buttonApply.Click += new System.EventHandler(buttonApply_Click);
		resources.ApplyResources(this.buttonCancel, "buttonCancel");
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.buttonCancel.Name = "buttonCancel";
		resources.ApplyResources(this.buttonOK, "buttonOK");
		this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.buttonOK.Name = "buttonOK";
		resources.ApplyResources(this.tabControlOptions, "tabControlOptions");
		this.tabControlOptions.Name = "tabControlOptions";
		this.tabControlOptions.SelectedIndex = 0;
		this.tabControlOptions.SelectedIndexChanged += new System.EventHandler(OnSelectedTabIndexChanged);
		this.tabControlPanel.Controls.Add(this.tabControlOptions);
		resources.ApplyResources(this.tabControlPanel, "tabControlPanel");
		this.tabControlPanel.Name = "tabControlPanel";
		base.AcceptButton = this.buttonOK;
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.buttonCancel;
		base.Controls.Add(this.tabControlPanel);
		base.Controls.Add(this.buttonApply);
		base.Controls.Add(this.buttonCancel);
		base.Controls.Add(this.buttonOK);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.HelpButton = true;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "OptionsDialog";
		base.ShowInTaskbar = false;
		base.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(OnHelpButtonClicked);
		this.tabControlPanel.ResumeLayout(false);
		base.ResumeLayout(false);
	}

	protected override void OnLoad(EventArgs e)
	{
		UpdateButtons();
		base.OnLoad(e);
	}

	protected override void OnFormClosing(FormClosingEventArgs e)
	{
		if (base.DialogResult == DialogResult.OK)
		{
			e.Cancel = !Apply();
		}
		base.OnFormClosing(e);
	}

	private void buttonApply_Click(object sender, EventArgs e)
	{
		Apply();
	}

	private void OnSelectedTabIndexChanged(object sender, EventArgs e)
	{
		UpdateButtons();
	}

	private bool Apply()
	{
		bool flag = true;
		foreach (OptionsPanel option in options)
		{
			flag &= option.ApplySettings();
		}
		if (flag)
		{
			SapiManager.GlobalInstance.SaveSettings();
		}
		return flag;
	}

	private void UpdateButtons()
	{
		bool flag = false;
		foreach (OptionsPanel option in options)
		{
			flag |= option.IsDirty;
		}
		buttonApply.Enabled = flag;
	}

	private void OnHelpButtonClicked(object sender, CancelEventArgs e)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		e.Cancel = true;
		Link.ShowTarget(Link.AvailableLinks.GetLinkOrEmpty("Form_OptionsDialog"));
	}
}
