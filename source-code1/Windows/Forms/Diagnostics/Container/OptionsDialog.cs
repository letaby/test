// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.OptionsDialog
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
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
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    this.AddTab((OptionsPanel) new FleetInfoOptionsPanel());
    this.AddTab((OptionsPanel) new DisplayOptionsPanel());
    this.AddTab((OptionsPanel) new ServerOptionsPanel());
    this.AddTab((OptionsPanel) new ConnectionOptionsPanel());
    this.AddTab((OptionsPanel) new SupportOptionsPanel());
    this.AddTab((OptionsPanel) new WarningsOptionsPanel());
  }

  internal OptionsDialog(OptionsPanel panel)
  {
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    this.AddTab(panel);
  }

  private void AddTab(OptionsPanel panel)
  {
    if (this.tabControlOptions.TabPages.ContainsKey(panel.Name))
      throw new InvalidOperationException("Adding a panel that is already added");
    this.tabControlOptions.SuspendLayout();
    this.options.Add(panel);
    this.tabControlOptions.TabPages.Add(panel.Name, panel.Text);
    this.tabControlOptions.TabPages[panel.Name].Controls.Add((Control) panel);
    panel.Dock = DockStyle.Fill;
    panel.Visible = true;
    panel.Dirty += new EventHandler(this.OnPanelDirty);
    panel.Clean += new EventHandler(this.OnPanelClean);
    panel.Enabled = LicenseManager.GlobalInstance.AccessLevel >= panel.MinAccessLevel;
    this.tabControlOptions.ResumeLayout(false);
  }

  private void OnPanelClean(object sender, EventArgs e) => this.UpdateButtons();

  private void OnPanelDirty(object sender, EventArgs e) => this.UpdateButtons();

  protected override void Dispose(bool disposing)
  {
    int num = disposing ? 1 : 0;
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (OptionsDialog));
    this.buttonApply = new Button();
    this.buttonCancel = new Button();
    this.buttonOK = new Button();
    this.tabControlOptions = new TabControl();
    this.tabControlPanel = new Panel();
    this.tabControlPanel.SuspendLayout();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) this.buttonApply, "buttonApply");
    this.buttonApply.Name = "buttonApply";
    this.buttonApply.Click += new EventHandler(this.buttonApply_Click);
    componentResourceManager.ApplyResources((object) this.buttonCancel, "buttonCancel");
    this.buttonCancel.DialogResult = DialogResult.Cancel;
    this.buttonCancel.Name = "buttonCancel";
    componentResourceManager.ApplyResources((object) this.buttonOK, "buttonOK");
    this.buttonOK.DialogResult = DialogResult.OK;
    this.buttonOK.Name = "buttonOK";
    componentResourceManager.ApplyResources((object) this.tabControlOptions, "tabControlOptions");
    this.tabControlOptions.Name = "tabControlOptions";
    this.tabControlOptions.SelectedIndex = 0;
    this.tabControlOptions.SelectedIndexChanged += new EventHandler(this.OnSelectedTabIndexChanged);
    this.tabControlPanel.Controls.Add((Control) this.tabControlOptions);
    componentResourceManager.ApplyResources((object) this.tabControlPanel, "tabControlPanel");
    this.tabControlPanel.Name = "tabControlPanel";
    this.AcceptButton = (IButtonControl) this.buttonOK;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.CancelButton = (IButtonControl) this.buttonCancel;
    this.Controls.Add((Control) this.tabControlPanel);
    this.Controls.Add((Control) this.buttonApply);
    this.Controls.Add((Control) this.buttonCancel);
    this.Controls.Add((Control) this.buttonOK);
    this.FormBorderStyle = FormBorderStyle.FixedDialog;
    this.HelpButton = true;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (OptionsDialog);
    this.ShowInTaskbar = false;
    this.HelpButtonClicked += new CancelEventHandler(this.OnHelpButtonClicked);
    this.tabControlPanel.ResumeLayout(false);
    this.ResumeLayout(false);
  }

  protected override void OnLoad(EventArgs e)
  {
    this.UpdateButtons();
    base.OnLoad(e);
  }

  protected override void OnFormClosing(FormClosingEventArgs e)
  {
    if (this.DialogResult == DialogResult.OK)
      e.Cancel = !this.Apply();
    base.OnFormClosing(e);
  }

  private void buttonApply_Click(object sender, EventArgs e) => this.Apply();

  private void OnSelectedTabIndexChanged(object sender, EventArgs e) => this.UpdateButtons();

  private bool Apply()
  {
    bool flag = true;
    foreach (OptionsPanel option in this.options)
      flag &= option.ApplySettings();
    if (flag)
      SapiManager.GlobalInstance.SaveSettings();
    return flag;
  }

  private void UpdateButtons()
  {
    bool flag = false;
    foreach (OptionsPanel option in this.options)
      flag |= option.IsDirty;
    this.buttonApply.Enabled = flag;
  }

  private void OnHelpButtonClicked(object sender, CancelEventArgs e)
  {
    e.Cancel = true;
    Link.ShowTarget(Link.AvailableLinks.GetLinkOrEmpty("Form_OptionsDialog"));
  }
}
