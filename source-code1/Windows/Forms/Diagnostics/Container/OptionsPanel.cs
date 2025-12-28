// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.OptionsPanel
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal class OptionsPanel : UserControl
{
  private bool dirty;
  private int minAccessLevel;
  private IContainer components;
  private PictureBox pictureBox;
  private Label labelHeader;

  protected OptionsPanel() => this.InitializeComponent();

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public bool IsDirty => this.dirty;

  protected void MarkDirty()
  {
    if (this.dirty)
      return;
    this.dirty = true;
    this.OnDirty(new EventArgs());
  }

  protected void MarkClean()
  {
    if (!this.dirty)
      return;
    this.dirty = false;
    this.OnClean(new EventArgs());
  }

  public event EventHandler Dirty;

  public event EventHandler Clean;

  protected virtual void OnDirty(EventArgs e)
  {
    if (this.Dirty == null)
      return;
    this.Dirty((object) this, e);
  }

  protected virtual void OnClean(EventArgs e)
  {
    if (this.Clean == null)
      return;
    this.Clean((object) this, e);
  }

  public virtual bool ApplySettings()
  {
    this.MarkClean();
    return true;
  }

  [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
  [Description("The title of this options panel")]
  [Browsable(true)]
  [EditorBrowsable(EditorBrowsableState.Always)]
  [Category("Appearance")]
  [Localizable(true)]
  public override string Text
  {
    get => base.Text;
    set => base.Text = value;
  }

  public int MinAccessLevel
  {
    get => this.minAccessLevel;
    protected set
    {
      this.minAccessLevel = this.minAccessLevel == 0 ? value : throw new InvalidOperationException("MinAccessLevel can only be set once.");
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
    get => this.labelHeader.Text;
    set => this.labelHeader.Text = value;
  }

  [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
  [Description("An image to represent the options on this panel")]
  [Browsable(true)]
  [EditorBrowsable(EditorBrowsableState.Always)]
  [Category("Appearance")]
  [Localizable(true)]
  public Image HeaderImage
  {
    get => this.pictureBox.Image;
    set => this.pictureBox.Image = value;
  }

  protected override void OnLoad(EventArgs e)
  {
    this.MarkClean();
    base.OnLoad(e);
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (OptionsPanel));
    this.pictureBox = new PictureBox();
    this.labelHeader = new Label();
    ((ISupportInitialize) this.pictureBox).BeginInit();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) this.pictureBox, "pictureBox");
    this.pictureBox.Name = "pictureBox";
    this.pictureBox.TabStop = false;
    componentResourceManager.ApplyResources((object) this.labelHeader, "labelHeader");
    this.labelHeader.Name = "labelHeader";
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.Controls.Add((Control) this.pictureBox);
    this.Controls.Add((Control) this.labelHeader);
    this.Name = nameof (OptionsPanel);
    ((ISupportInitialize) this.pictureBox).EndInit();
    this.ResumeLayout(false);
  }
}
