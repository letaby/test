// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.ConnectionState
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
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public class ConnectionState : UserControl
{
  private const string J1939EnhancedName = "J1939\nCAN";
  private const string LongestProtocolName = "J1939";
  private const int minimumImageSize = 8;
  private Size defaultImageSize;
  private readonly Image grayCircle;
  private readonly Image greenCircle;
  private readonly Image redCircle;
  private readonly Image yellowCircle;
  private IRollCall rollCallManager;
  private bool blinkOn;
  private bool stacked;
  private bool textInside;
  private Protocol protocol;
  private bool previouslyMonitoring;
  private IContainer components;
  private ToolTip overflowTooltip;

  [DefaultValue(false)]
  [Description("Show the icon stacked vertically above the text")]
  [Category("Appearance")]
  public bool StackedView
  {
    get => this.stacked;
    set
    {
      if (this.stacked == value)
        return;
      this.stacked = value;
      if (this.AutoSize)
        this.OnSizeChanged(new EventArgs());
      this.Invalidate();
    }
  }

  [DefaultValue(false)]
  [Description("Show the text inside the graphical images")]
  [Category("Appearance")]
  public bool TextInside
  {
    get => this.textInside;
    set
    {
      if (this.textInside == value)
        return;
      this.textInside = value;
      if (this.AutoSize)
        this.OnSizeChanged(new EventArgs());
      this.Invalidate();
    }
  }

  [DefaultValue(typeof (Protocol), "None")]
  [Description("The protocol type that this control will show information for.")]
  [Category("Behavior")]
  public Protocol Protocol
  {
    get => this.protocol;
    set
    {
      if (this.protocol == value)
        return;
      if (this.rollCallManager != null)
      {
        this.rollCallManager.StateChangedEvent -= new EventHandler<StateChangedEventArgs>(this.rollCall_StateChangedEvent);
        this.rollCallManager.LoadChangedEvent -= new EventHandler<EventArgs>(this.rollCall_LoadChangedEvent);
      }
      this.protocol = value;
      if (this.DesignMode)
        return;
      this.rollCallManager = ChannelCollection.GetManager(this.protocol);
      if (this.rollCallManager == null)
        return;
      this.rollCallManager.StateChangedEvent += new EventHandler<StateChangedEventArgs>(this.rollCall_StateChangedEvent);
      this.rollCallManager.LoadChangedEvent += new EventHandler<EventArgs>(this.rollCall_LoadChangedEvent);
      if (!this.AutoSize)
        return;
      this.OnSizeChanged(new EventArgs());
    }
  }

  public ConnectionState()
  {
    this.SetStyle(ControlStyles.ResizeRedraw, true);
    this.SetStyle(ControlStyles.DoubleBuffer, true);
    this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
    this.SetStyle(ControlStyles.UserPaint, true);
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    this.grayCircle = (Image) Resources.Gray1;
    this.redCircle = (Image) Resources.Red2;
    this.yellowCircle = (Image) Resources.Yellow2;
    this.greenCircle = (Image) Resources.Green2;
  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    if (this.DesignMode)
      return;
    ThemeProvider.GlobalInstance.ActiveTheme.BlinkEvent += new EventHandler<BlinkEventArgs>(this.ActiveTheme_BlinkEvent);
  }

  private bool ShowLoad
  {
    get
    {
      return (!this.stacked || this.textInside) && !this.DesignMode && ApplicationInformation.CanDisplayBusload && this.rollCallManager != null && this.rollCallManager.Load.HasValue;
    }
  }

  public override Size GetPreferredSize(Size proposedSize)
  {
    if (this.rollCallManager != null && !this.IsMonitoring)
      return Size.Empty;
    Size size = TextRenderer.MeasureText(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}{1}{2}", (object) "J1939", (object) Environment.NewLine, (object) "100%"), this.Font);
    Size preferredSize = this.defaultImageSize;
    if (this.textInside)
      preferredSize = new Size(size.Width * 3 / 2, size.Width * 3 / 2);
    else if (this.stacked)
      preferredSize.Height += size.Height;
    else
      preferredSize.Width += size.Width;
    return preferredSize;
  }

  private void ActiveTheme_BlinkEvent(object sender, BlinkEventArgs e)
  {
    if (this.rollCallManager == null || this.rollCallManager.State != 4 && this.rollCallManager.State != 6)
      return;
    this.blinkOn = e.Blink;
    this.Invalidate();
  }

  private void rollCall_LoadChangedEvent(object sender, EventArgs e) => this.Invalidate();

  private void rollCall_StateChangedEvent(object sender, StateChangedEventArgs e)
  {
    if (this.AutoSize && this.previouslyMonitoring != this.IsMonitoring)
    {
      this.OnSizeChanged(new EventArgs());
      this.previouslyMonitoring = this.IsMonitoring;
    }
    this.Invalidate();
  }

  private bool IsMonitoring
  {
    get
    {
      return this.rollCallManager != null && this.rollCallManager.State != 1 && this.rollCallManager.State > 0;
    }
  }

  private Image GetImage()
  {
    if (this.rollCallManager != null)
    {
      switch (this.rollCallManager.State - 2)
      {
        case 0:
        case 5:
          return this.redCircle;
        case 1:
          if (this.rollCallManager.Protocol == 71993 && this.rollCallManager.Load.HasValue && (double) this.rollCallManager.Load.Value > 0.0)
          {
            foreach (Channel channel in (ChannelBaseCollection) Sapi.GetSapi().Channels)
            {
              if (channel.Online && channel.Ecu.IsUds)
                return this.greenCircle;
            }
          }
          return this.yellowCircle;
        case 2:
          return !this.blinkOn ? this.yellowCircle : this.greenCircle;
        case 3:
          return this.greenCircle;
        case 4:
          return !this.blinkOn ? this.redCircle : this.yellowCircle;
      }
    }
    return this.grayCircle;
  }

  public override string Text
  {
    get
    {
      StringBuilder stringBuilder = new StringBuilder(this.ProtocolName);
      if (this.ShowLoad && !this.DesignMode && this.rollCallManager.Load.HasValue)
      {
        string str = this.rollCallManager.Load.Value.ToString("F0", (IFormatProvider) CultureInfo.CurrentCulture) + "%";
        stringBuilder.Append(Environment.NewLine).Append(str);
      }
      return stringBuilder.ToString();
    }
    set
    {
      throw new NotSupportedException("Setting the Text property on this control is not supported");
    }
  }

  protected override void OnPaint(PaintEventArgs e)
  {
    Size suggestedTextSize = TextRenderer.MeasureText("J1939", this.Font);
    Rectangle rectangle = this.ImagePaintLocation(suggestedTextSize);
    Rectangle bounds = this.TextPaintLocation(rectangle, suggestedTextSize);
    this.SetTooltipIfClipped(bounds.Size);
    e.Graphics.DrawImage(this.GetImage(), rectangle);
    TextRenderer.DrawText((IDeviceContext) e.Graphics, this.Text, this.Font, bounds, SystemColors.WindowText, ConnectionState.GetTextFormat(true));
  }

  private void SetTooltipIfClipped(Size textTargetSize)
  {
    Size size = TextRenderer.MeasureText(this.Text, this.Font, textTargetSize, ConnectionState.GetTextFormat(false));
    if (textTargetSize.Width < size.Width || textTargetSize.Height < size.Height)
      this.overflowTooltip.SetToolTip((Control) this, this.Text);
    else
      this.overflowTooltip.RemoveAll();
  }

  private static TextFormatFlags GetTextFormat(bool useEllipsis)
  {
    return (TextFormatFlags) (21 | (useEllipsis ? 262144 /*0x040000*/ : 0));
  }

  private Rectangle ImagePaintLocation(Size suggestedTextSize)
  {
    if (this.textInside)
      return this.ClientRectangle;
    Point empty = Point.Empty;
    int num1;
    if (this.stacked)
    {
      int height = this.ClientRectangle.Height;
      Rectangle clientRectangle = this.ClientRectangle;
      int val2 = Math.Max(8, clientRectangle.Height - suggestedTextSize.Height);
      int val1 = Math.Min(height, val2);
      clientRectangle = this.ClientRectangle;
      int width = clientRectangle.Width;
      num1 = Math.Min(val1, width);
      ref Point local1 = ref empty;
      clientRectangle = this.ClientRectangle;
      int x = clientRectangle.X;
      clientRectangle = this.ClientRectangle;
      int num2 = (clientRectangle.Width - num1) / 2;
      int num3 = x + num2;
      local1.X = num3;
      ref Point local2 = ref empty;
      clientRectangle = this.ClientRectangle;
      int y = clientRectangle.Y;
      local2.Y = y;
    }
    else
    {
      int width = this.ClientRectangle.Width;
      Rectangle clientRectangle = this.ClientRectangle;
      int val2 = Math.Max(8, clientRectangle.Width - suggestedTextSize.Width);
      int val1 = Math.Min(width, val2);
      clientRectangle = this.ClientRectangle;
      int height = clientRectangle.Height;
      num1 = Math.Min(val1, height);
      ref Point local3 = ref empty;
      clientRectangle = this.ClientRectangle;
      int x = clientRectangle.X;
      local3.X = x;
      ref Point local4 = ref empty;
      clientRectangle = this.ClientRectangle;
      int y = clientRectangle.Y;
      clientRectangle = this.ClientRectangle;
      int num4 = (clientRectangle.Height - num1) / 2;
      int num5 = y + num4;
      local4.Y = num5;
    }
    return new Rectangle(empty, new Size(num1, num1));
  }

  private Rectangle TextPaintLocation(Rectangle imageLocation, Size suggestedTextSize)
  {
    if (this.textInside)
      return this.ClientRectangle;
    Point empty1 = Point.Empty;
    Size empty2 = Size.Empty;
    if (this.stacked)
    {
      empty1.X = this.ClientRectangle.X;
      empty1.Y = this.ClientRectangle.Y + this.ClientRectangle.Height - suggestedTextSize.Height;
      empty2.Width = this.ClientRectangle.Width;
      empty2.Height = suggestedTextSize.Height;
    }
    else
    {
      empty1.X = this.ClientRectangle.X + imageLocation.Width;
      empty1.Y = this.ClientRectangle.Y;
      empty2.Width = suggestedTextSize.Width;
      empty2.Height = this.ClientRectangle.Height;
    }
    return new Rectangle(empty1, empty2);
  }

  private string ProtocolName
  {
    get
    {
      return this.protocol != 71993 || ApplicationInformation.CanDisplayBusload ? this.protocol.ToString() : "J1939\nCAN";
    }
  }

  protected override void OnCreateControl()
  {
    base.OnCreateControl();
    using (Graphics graphics = this.CreateGraphics())
      this.defaultImageSize = new Size((int) ((double) this.greenCircle.Width * (double) graphics.DpiX / 96.0) / 4, (int) ((double) this.greenCircle.Height * (double) graphics.DpiX / 96.0) / 4);
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    if (this.rollCallManager != null)
    {
      this.rollCallManager.StateChangedEvent -= new EventHandler<StateChangedEventArgs>(this.rollCall_StateChangedEvent);
      this.rollCallManager.LoadChangedEvent -= new EventHandler<EventArgs>(this.rollCall_LoadChangedEvent);
    }
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new System.ComponentModel.Container();
    this.overflowTooltip = new ToolTip(this.components);
    this.SuspendLayout();
    this.AutoScaleDimensions = new SizeF(6f, 13f);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.Name = nameof (ConnectionState);
    this.Size = new Size(211, 59);
    this.ResumeLayout(false);
  }
}
