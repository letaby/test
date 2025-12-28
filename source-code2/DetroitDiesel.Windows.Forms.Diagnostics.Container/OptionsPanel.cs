using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal class OptionsPanel : UserControl
{
	private bool dirty;

	private int minAccessLevel;

	private IContainer components;

	private PictureBox pictureBox;

	private Label labelHeader;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool IsDirty => dirty;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Description("The title of this options panel")]
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[Category("Appearance")]
	[Localizable(true)]
	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			base.Text = value;
		}
	}

	public int MinAccessLevel
	{
		get
		{
			return minAccessLevel;
		}
		protected set
		{
			if (minAccessLevel != 0)
			{
				throw new InvalidOperationException("MinAccessLevel can only be set once.");
			}
			minAccessLevel = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Description("The usage information for this panel")]
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[Category("Appearance")]
	[Localizable(true)]
	public string HeaderText
	{
		get
		{
			return labelHeader.Text;
		}
		set
		{
			labelHeader.Text = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Description("An image to represent the options on this panel")]
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[Category("Appearance")]
	[Localizable(true)]
	public Image HeaderImage
	{
		get
		{
			return pictureBox.Image;
		}
		set
		{
			pictureBox.Image = value;
		}
	}

	public event EventHandler Dirty;

	public event EventHandler Clean;

	protected OptionsPanel()
	{
		InitializeComponent();
	}

	protected void MarkDirty()
	{
		if (!dirty)
		{
			dirty = true;
			OnDirty(new EventArgs());
		}
	}

	protected void MarkClean()
	{
		if (dirty)
		{
			dirty = false;
			OnClean(new EventArgs());
		}
	}

	protected virtual void OnDirty(EventArgs e)
	{
		if (this.Dirty != null)
		{
			this.Dirty(this, e);
		}
	}

	protected virtual void OnClean(EventArgs e)
	{
		if (this.Clean != null)
		{
			this.Clean(this, e);
		}
	}

	public virtual bool ApplySettings()
	{
		MarkClean();
		return true;
	}

	protected override void OnLoad(EventArgs e)
	{
		MarkClean();
		base.OnLoad(e);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Container.OptionsPanel));
		this.pictureBox = new System.Windows.Forms.PictureBox();
		this.labelHeader = new System.Windows.Forms.Label();
		((System.ComponentModel.ISupportInitialize)this.pictureBox).BeginInit();
		base.SuspendLayout();
		resources.ApplyResources(this.pictureBox, "pictureBox");
		this.pictureBox.Name = "pictureBox";
		this.pictureBox.TabStop = false;
		resources.ApplyResources(this.labelHeader, "labelHeader");
		this.labelHeader.Name = "labelHeader";
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.pictureBox);
		base.Controls.Add(this.labelHeader);
		base.Name = "OptionsPanel";
		((System.ComponentModel.ISupportInitialize)this.pictureBox).EndInit();
		base.ResumeLayout(false);
	}
}
