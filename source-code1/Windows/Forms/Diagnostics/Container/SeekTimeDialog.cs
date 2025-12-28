// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.SeekTimeDialog
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal class SeekTimeDialog : Form
{
  private LogFile logFile;
  private IContainer components;
  private TableLayoutPanel tableLayoutPanel1;
  private DateTimePicker seekDateTime;
  private Button buttonOK;
  private SeekTimeListView seekTimeListView;

  public DateTime SelectedTime => this.seekDateTime.Value;

  public SeekTimeDialog(LogFile logFile)
  {
    this.logFile = logFile != null ? logFile : throw new ArgumentNullException(nameof (logFile), "Must provide a valid log file.");
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    this.seekDateTime.CustomFormat = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}", (object) CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, (object) CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern);
  }

  protected override void OnLoad(EventArgs e)
  {
    this.seekDateTime.Value = this.logFile.CurrentTime;
    base.OnLoad(e);
  }

  private void seekTimeListView_SelectedTimeChanged(object sender, EventArgs e)
  {
    if (!this.seekTimeListView.SelectedTime.HasValue)
      return;
    DateTime dateTime = this.seekTimeListView.SelectedTime.Value;
    if (!(this.seekDateTime.Value != dateTime))
      return;
    this.seekDateTime.Value = dateTime;
  }

  private void OnSeekTimeChanged(object sender, EventArgs e)
  {
    DateTime dateTime = this.seekDateTime.Value;
    if (dateTime < this.logFile.StartTime)
    {
      this.seekDateTime.Value = this.logFile.StartTime;
    }
    else
    {
      if (!(dateTime > this.logFile.EndTime))
        return;
      this.seekDateTime.Value = this.logFile.EndTime;
    }
  }

  private void seekTimeListView_TimeActivate(object sender, EventArgs e)
  {
    this.buttonOK.PerformClick();
  }

  private void OnHelpButtonClicked(object sender, CancelEventArgs e)
  {
    e.Cancel = true;
    Link.ShowTarget(Link.AvailableLinks.GetLinkOrEmpty("Form_SeekTimeDialog"));
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (SeekTimeDialog));
    this.buttonOK = new Button();
    this.seekDateTime = new DateTimePicker();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.seekTimeListView = new SeekTimeListView();
    Label label1 = new Label();
    Button button = new Button();
    Label label2 = new Label();
    this.tableLayoutPanel1.SuspendLayout();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) label1, "labelHeader");
    this.tableLayoutPanel1.SetColumnSpan((Control) label1, 2);
    label1.Name = "labelHeader";
    componentResourceManager.ApplyResources((object) button, "buttonCancel");
    button.DialogResult = DialogResult.Cancel;
    button.Name = "buttonCancel";
    componentResourceManager.ApplyResources((object) label2, "labelTime");
    label2.Name = "labelTime";
    componentResourceManager.ApplyResources((object) this.buttonOK, "buttonOK");
    this.buttonOK.DialogResult = DialogResult.OK;
    this.buttonOK.Name = "buttonOK";
    this.buttonOK.Click += new EventHandler(this.OnSeekTimeChanged);
    this.seekDateTime.Format = DateTimePickerFormat.Custom;
    componentResourceManager.ApplyResources((object) this.seekDateTime, "seekDateTime");
    this.seekDateTime.Name = "seekDateTime";
    this.seekDateTime.ShowUpDown = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    this.tableLayoutPanel1.Controls.Add((Control) label1, 0, 0);
    this.tableLayoutPanel1.Controls.Add((Control) this.seekDateTime, 1, 2);
    this.tableLayoutPanel1.Controls.Add((Control) label2, 0, 2);
    this.tableLayoutPanel1.Controls.Add((Control) this.seekTimeListView, 0, 1);
    this.tableLayoutPanel1.Name = "tableLayoutPanel1";
    this.tableLayoutPanel1.SetColumnSpan((Control) this.seekTimeListView, 2);
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.SelectedTimeChanged += new EventHandler(this.seekTimeListView_SelectedTimeChanged);
    this.seekTimeListView.TimeActivate += new EventHandler(this.seekTimeListView_TimeActivate);
    this.AcceptButton = (IButtonControl) this.buttonOK;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.CancelButton = (IButtonControl) button;
    this.Controls.Add((Control) this.tableLayoutPanel1);
    this.Controls.Add((Control) this.buttonOK);
    this.Controls.Add((Control) button);
    this.HelpButton = true;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (SeekTimeDialog);
    this.ShowIcon = false;
    this.ShowInTaskbar = false;
    this.HelpButtonClicked += new CancelEventHandler(this.OnHelpButtonClicked);
    this.tableLayoutPanel1.ResumeLayout(false);
    this.tableLayoutPanel1.PerformLayout();
    this.ResumeLayout(false);
  }
}
