// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.EcuStatusItem
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using DetroitDiesel.Windows.Forms.Themed;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public class EcuStatusItem : UserControl
{
  private const float NotifcationScale = 0.7f;
  private InPlaceToolTip toolTip = new InPlaceToolTip();
  private Font descriptionFont;
  private Size imageSize;
  private Image image;
  private int preferredHeight;
  private readonly string identifier;
  private Ecu ecu;
  private string descriptionText = "<no description>";
  private string statusText = "<no status>";
  private ConnectionIcon icon;
  private NotificationItem notification = new NotificationItem();
  private bool toolTipDisplayed;
  private IContainer components;

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public new string Text
  {
    get => base.Text;
    set => base.Text = value;
  }

  public string Identifier => this.identifier;

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public Ecu Ecu
  {
    get => this.ecu;
    set
    {
      if (this.ecu == value)
        return;
      this.ecu = value;
      this.descriptionText = !string.IsNullOrEmpty(this.ecu.ShortDescription) ? this.ecu.ShortDescription : this.ecu.Name;
      this.Invalidate();
    }
  }

  [Category("Appearance")]
  [Description("The description of the Ecu.")]
  [DefaultValue(typeof (string), "<no description>")]
  public string DescriptionText => this.descriptionText;

  [Category("Appearance")]
  [Description("The status of the Ecu.")]
  [DefaultValue(typeof (string), "<no status>")]
  public string StatusText
  {
    get => this.statusText;
    set
    {
      if (!(this.statusText != value))
        return;
      this.statusText = value;
      this.Invalidate();
    }
  }

  [Category("Appearance")]
  [Description("The connection icon to show.")]
  [DefaultValue(ConnectionIcon.Gray)]
  public ConnectionIcon Icon
  {
    get => this.icon;
    set
    {
      this.icon = value;
      switch (this.icon)
      {
        case ConnectionIcon.Red:
          this.image = (Image) Resources.Red2;
          break;
        case ConnectionIcon.Yellow:
          this.image = (Image) Resources.Yellow2;
          break;
        case ConnectionIcon.Green:
          this.image = (Image) Resources.Green2;
          break;
        case ConnectionIcon.Gray:
          this.image = (Image) Resources.Gray1;
          break;
      }
      this.Invalidate();
    }
  }

  public NotificationItem Notification => this.notification;

  public override Font Font
  {
    get => base.Font;
    set
    {
      if (value == base.Font)
        return;
      base.Font = value;
      this.descriptionFont = new Font(value, FontStyle.Bold);
      this.notification.Font = new Font(value.FontFamily, value.SizeInPoints * 0.7f);
      Size size = TextRenderer.MeasureText(Resources.EcuStatusView_Writing, this.descriptionFont);
      this.preferredHeight = size.Height * 2;
      this.imageSize = new Size(size.Height * 4 / 3, size.Height * 4 / 3);
    }
  }

  public EcuStatusItem(string identifier)
  {
    this.Font = SystemFonts.MessageBoxFont;
    this.identifier = identifier;
    this.SetStyle(ControlStyles.ResizeRedraw, true);
    this.SetStyle(ControlStyles.DoubleBuffer, true);
    this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
    this.SetStyle(ControlStyles.UserPaint, true);
    this.SetStyle(ControlStyles.Selectable, true);
    this.InitializeComponent();
    ((ToolTip) this.toolTip).AutomaticDelay = 0;
    ((ToolTip) this.toolTip).ShowAlways = true;
    this.notification.ContentChanged += new EventHandler(this.notificationItem_ContentChanged);
  }

  private void notificationItem_ContentChanged(object sender, EventArgs e) => this.Invalidate();

  protected override void OnMouseMove(MouseEventArgs e)
  {
    base.OnMouseMove(e);
    if (this.toolTipDisplayed)
      return;
    ((ToolTip) this.toolTip).Show(((!this.ecu.IsRollCall ? this.DescriptionText : string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.EcuStatusItem_DescriptionToolTipGenericFormat, (object) this.DescriptionText, this.ecu.Name != this.Identifier ? (object) (" " + this.Identifier) : (object) string.Empty)) + Environment.NewLine + this.StatusText).Trim(), (IWin32Window) this);
    this.toolTipDisplayed = true;
  }

  protected override void OnMouseLeave(EventArgs e)
  {
    base.OnMouseLeave(e);
    if (!this.toolTipDisplayed)
      return;
    ((ToolTip) this.toolTip).Hide((IWin32Window) this);
    this.toolTipDisplayed = false;
  }

  protected override void OnPaint(PaintEventArgs e)
  {
    Rectangle rect;
    ref Rectangle local1 = ref rect;
    Rectangle clientRectangle1 = this.ClientRectangle;
    int x1 = clientRectangle1.X;
    clientRectangle1 = this.ClientRectangle;
    int y1 = clientRectangle1.Y;
    clientRectangle1 = this.ClientRectangle;
    int num = (clientRectangle1.Height - this.imageSize.Height) / 2;
    int y2 = y1 + num;
    int width1 = this.imageSize.Width;
    int height1 = this.imageSize.Height;
    local1 = new Rectangle(x1, y2, width1, height1);
    e.Graphics.DrawImage(this.image, rect);
    Rectangle rectangle1;
    ref Rectangle local2 = ref rectangle1;
    Rectangle clientRectangle2 = this.ClientRectangle;
    int x2 = clientRectangle2.X + rect.Width;
    clientRectangle2 = this.ClientRectangle;
    int y3 = clientRectangle2.Y;
    clientRectangle2 = this.ClientRectangle;
    int width2 = clientRectangle2.Width - rect.Width;
    clientRectangle2 = this.ClientRectangle;
    int height2 = clientRectangle2.Height;
    local2 = new Rectangle(x2, y3, width2, height2);
    TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter;
    Rectangle bounds1 = rectangle1;
    bounds1.Height /= 2;
    Rectangle bounds2 = bounds1 with
    {
      Y = rectangle1.Y + bounds1.Height
    };
    TextRenderer.DrawText((IDeviceContext) e.Graphics, EcuStatusItem.GetFirstLine(this.descriptionText), this.descriptionFont, bounds1, this.ForeColor, flags);
    TextRenderer.DrawText((IDeviceContext) e.Graphics, EcuStatusItem.GetFirstLine(this.statusText), this.Font, bounds2, this.ForeColor, flags);
    if (this.notification.Count > 0)
    {
      NotificationItem notification = this.notification;
      Graphics graphics = e.Graphics;
      int right = rect.Right;
      Size size = this.notification.Size;
      int width3 = size.Width;
      int x3 = right - width3;
      int bottom = rect.Bottom;
      size = this.notification.Size;
      int height3 = size.Height;
      int y4 = bottom - height3;
      Rectangle rectangle2 = new Rectangle(new Point(x3, y4), this.notification.Size);
      notification.Draw(graphics, rectangle2);
    }
    if (this.ecu != null && this.ecu.IsRollCall)
      e.Graphics.FillRectangle((Brush) new SolidBrush(Color.FromArgb(96 /*0x60*/, SystemColors.Window)), this.ClientRectangle);
    if (!this.Focused)
      return;
    ControlPaint.DrawFocusRectangle(e.Graphics, this.ClientRectangle);
  }

  protected override void OnGotFocus(EventArgs e)
  {
    base.OnGotFocus(e);
    this.Invalidate();
  }

  protected override void OnLostFocus(EventArgs e)
  {
    base.OnLostFocus(e);
    this.Invalidate();
  }

  private static string GetFirstLine(string input)
  {
    return input != null && input.Contains(Environment.NewLine) ? input.Substring(0, input.IndexOf(Environment.NewLine, StringComparison.Ordinal)) : input;
  }

  public override Size GetPreferredSize(Size proposedSize)
  {
    return new Size(proposedSize.Width, this.preferredHeight);
  }

  public void Copy(ClipboardData data)
  {
    data.Text.Append(this.DescriptionText);
    data.Csv.Append(this.DescriptionText);
    data.SymbolicLink.AppendCellData(this.DescriptionText);
    data.Text.Append("\t");
    data.Csv.Append(",");
    data.SymbolicLink.AdvanceColumn();
    data.Text.Append(this.StatusText);
    data.Csv.Append(this.StatusText);
    data.SymbolicLink.AppendCellData(this.StatusText);
    data.Text.Append(Environment.NewLine);
    data.Csv.Append(Environment.NewLine);
    data.SymbolicLink.AdvanceRow();
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing)
    {
      if (this.components != null)
        this.components.Dispose();
      if (this.descriptionFont != null)
      {
        this.descriptionFont.Dispose();
        this.descriptionFont = (Font) null;
      }
    }
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    this.SuspendLayout();
    this.AutoScaleDimensions = new SizeF(6f, 13f);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.AutoSize = true;
    this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
    this.BackColor = Color.Transparent;
    this.Margin = new Padding(0);
    this.Name = nameof (EcuStatusItem);
    this.Size = new Size(0, 0);
    this.ResumeLayout(false);
  }
}
