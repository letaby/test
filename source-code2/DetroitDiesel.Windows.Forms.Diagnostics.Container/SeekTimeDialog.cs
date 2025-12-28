using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal class SeekTimeDialog : Form
{
	private LogFile logFile;

	private IContainer components;

	private TableLayoutPanel tableLayoutPanel1;

	private DateTimePicker seekDateTime;

	private Button buttonOK;

	private SeekTimeListView seekTimeListView;

	public DateTime SelectedTime => seekDateTime.Value;

	public SeekTimeDialog(LogFile logFile)
	{
		if (logFile == null)
		{
			throw new ArgumentNullException("logFile", "Must provide a valid log file.");
		}
		this.logFile = logFile;
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		seekDateTime.CustomFormat = string.Format(CultureInfo.CurrentCulture, "{0} {1}", CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern);
	}

	protected override void OnLoad(EventArgs e)
	{
		seekDateTime.Value = logFile.CurrentTime;
		base.OnLoad(e);
	}

	private void seekTimeListView_SelectedTimeChanged(object sender, EventArgs e)
	{
		if (seekTimeListView.SelectedTime.HasValue)
		{
			DateTime value = seekTimeListView.SelectedTime.Value;
			if (seekDateTime.Value != value)
			{
				seekDateTime.Value = value;
			}
		}
	}

	private void OnSeekTimeChanged(object sender, EventArgs e)
	{
		DateTime value = seekDateTime.Value;
		if (value < logFile.StartTime)
		{
			seekDateTime.Value = logFile.StartTime;
		}
		else if (value > logFile.EndTime)
		{
			seekDateTime.Value = logFile.EndTime;
		}
	}

	private void seekTimeListView_TimeActivate(object sender, EventArgs e)
	{
		buttonOK.PerformClick();
	}

	private void OnHelpButtonClicked(object sender, CancelEventArgs e)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		e.Cancel = true;
		Link.ShowTarget(Link.AvailableLinks.GetLinkOrEmpty("Form_SeekTimeDialog"));
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
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Container.SeekTimeDialog));
		this.buttonOK = new System.Windows.Forms.Button();
		this.seekDateTime = new System.Windows.Forms.DateTimePicker();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.seekTimeListView = new SeekTimeListView();
		System.Windows.Forms.Label label = new System.Windows.Forms.Label();
		System.Windows.Forms.Button button = new System.Windows.Forms.Button();
		System.Windows.Forms.Label label2 = new System.Windows.Forms.Label();
		this.tableLayoutPanel1.SuspendLayout();
		base.SuspendLayout();
		resources.ApplyResources(label, "labelHeader");
		this.tableLayoutPanel1.SetColumnSpan(label, 2);
		label.Name = "labelHeader";
		resources.ApplyResources(button, "buttonCancel");
		button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		button.Name = "buttonCancel";
		resources.ApplyResources(label2, "labelTime");
		label2.Name = "labelTime";
		resources.ApplyResources(this.buttonOK, "buttonOK");
		this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.buttonOK.Name = "buttonOK";
		this.buttonOK.Click += new System.EventHandler(OnSeekTimeChanged);
		this.seekDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
		resources.ApplyResources(this.seekDateTime, "seekDateTime");
		this.seekDateTime.Name = "seekDateTime";
		this.seekDateTime.ShowUpDown = true;
		resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
		this.tableLayoutPanel1.Controls.Add(label, 0, 0);
		this.tableLayoutPanel1.Controls.Add(this.seekDateTime, 1, 2);
		this.tableLayoutPanel1.Controls.Add(label2, 0, 2);
		this.tableLayoutPanel1.Controls.Add((System.Windows.Forms.Control)(object)this.seekTimeListView, 0, 1);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.SetColumnSpan((System.Windows.Forms.Control)(object)this.seekTimeListView, 2);
		resources.ApplyResources(this.seekTimeListView, "seekTimeListView");
		((System.Windows.Forms.Control)(object)this.seekTimeListView).Name = "seekTimeListView";
		this.seekTimeListView.SelectedTimeChanged += new System.EventHandler(seekTimeListView_SelectedTimeChanged);
		this.seekTimeListView.TimeActivate += new System.EventHandler(seekTimeListView_TimeActivate);
		base.AcceptButton = this.buttonOK;
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = button;
		base.Controls.Add(this.tableLayoutPanel1);
		base.Controls.Add(this.buttonOK);
		base.Controls.Add(button);
		base.HelpButton = true;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "SeekTimeDialog";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(OnHelpButtonClicked);
		this.tableLayoutPanel1.ResumeLayout(false);
		this.tableLayoutPanel1.PerformLayout();
		base.ResumeLayout(false);
	}
}
