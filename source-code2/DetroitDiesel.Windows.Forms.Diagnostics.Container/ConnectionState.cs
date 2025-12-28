using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using DetroitDiesel.Windows.Forms.Themed;
using SapiLayer1;

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
		get
		{
			return stacked;
		}
		set
		{
			if (stacked != value)
			{
				stacked = value;
				if (AutoSize)
				{
					OnSizeChanged(new EventArgs());
				}
				Invalidate();
			}
		}
	}

	[DefaultValue(false)]
	[Description("Show the text inside the graphical images")]
	[Category("Appearance")]
	public bool TextInside
	{
		get
		{
			return textInside;
		}
		set
		{
			if (textInside != value)
			{
				textInside = value;
				if (AutoSize)
				{
					OnSizeChanged(new EventArgs());
				}
				Invalidate();
			}
		}
	}

	[DefaultValue(typeof(Protocol), "None")]
	[Description("The protocol type that this control will show information for.")]
	[Category("Behavior")]
	public Protocol Protocol
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return protocol;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			if (protocol == value)
			{
				return;
			}
			if (rollCallManager != null)
			{
				rollCallManager.StateChangedEvent -= rollCall_StateChangedEvent;
				rollCallManager.LoadChangedEvent -= rollCall_LoadChangedEvent;
			}
			protocol = value;
			if (base.DesignMode)
			{
				return;
			}
			rollCallManager = ChannelCollection.GetManager(protocol);
			if (rollCallManager != null)
			{
				rollCallManager.StateChangedEvent += rollCall_StateChangedEvent;
				rollCallManager.LoadChangedEvent += rollCall_LoadChangedEvent;
				if (AutoSize)
				{
					OnSizeChanged(new EventArgs());
				}
			}
		}
	}

	private bool ShowLoad
	{
		get
		{
			if (stacked && !textInside)
			{
				return false;
			}
			if (!base.DesignMode && ApplicationInformation.CanDisplayBusload && rollCallManager != null)
			{
				return rollCallManager.Load.HasValue;
			}
			return false;
		}
	}

	private bool IsMonitoring
	{
		get
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Invalid comparison between Unknown and I4
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Invalid comparison between Unknown and I4
			if (rollCallManager != null)
			{
				if ((int)rollCallManager.State != 1)
				{
					return (int)rollCallManager.State > 0;
				}
				return false;
			}
			return false;
		}
	}

	public override string Text
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(ProtocolName);
			if (ShowLoad && !base.DesignMode && rollCallManager.Load.HasValue)
			{
				string value = rollCallManager.Load.Value.ToString("F0", CultureInfo.CurrentCulture) + "%";
				stringBuilder.Append(Environment.NewLine).Append(value);
			}
			return stringBuilder.ToString();
		}
		set
		{
			throw new NotSupportedException("Setting the Text property on this control is not supported");
		}
	}

	private string ProtocolName
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Invalid comparison between Unknown and I4
			if ((int)protocol != 71993 || ApplicationInformation.CanDisplayBusload)
			{
				return ((object)System.Runtime.CompilerServices.Unsafe.As<Protocol, Protocol>(ref protocol)/*cast due to .constrained prefix*/).ToString();
			}
			return "J1939\nCAN";
		}
	}

	public ConnectionState()
	{
		SetStyle(ControlStyles.ResizeRedraw, value: true);
		SetStyle(ControlStyles.DoubleBuffer, value: true);
		SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
		SetStyle(ControlStyles.UserPaint, value: true);
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		grayCircle = Resources.Gray1;
		redCircle = Resources.Red2;
		yellowCircle = Resources.Yellow2;
		greenCircle = Resources.Green2;
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!base.DesignMode)
		{
			ThemeProvider.GlobalInstance.ActiveTheme.BlinkEvent += ActiveTheme_BlinkEvent;
		}
	}

	public override Size GetPreferredSize(Size proposedSize)
	{
		if (rollCallManager != null && !IsMonitoring)
		{
			return Size.Empty;
		}
		string text = string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}", "J1939", Environment.NewLine, "100%");
		Size size = TextRenderer.MeasureText(text, Font);
		Size result = defaultImageSize;
		if (textInside)
		{
			result = new Size(size.Width * 3 / 2, size.Width * 3 / 2);
		}
		else if (stacked)
		{
			result.Height += size.Height;
		}
		else
		{
			result.Width += size.Width;
		}
		return result;
	}

	private void ActiveTheme_BlinkEvent(object sender, BlinkEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		if (rollCallManager != null && ((int)rollCallManager.State == 4 || (int)rollCallManager.State == 6))
		{
			blinkOn = e.Blink;
			Invalidate();
		}
	}

	private void rollCall_LoadChangedEvent(object sender, EventArgs e)
	{
		Invalidate();
	}

	private void rollCall_StateChangedEvent(object sender, StateChangedEventArgs e)
	{
		if (AutoSize && previouslyMonitoring != IsMonitoring)
		{
			OnSizeChanged(new EventArgs());
			previouslyMonitoring = IsMonitoring;
		}
		Invalidate();
	}

	private Image GetImage()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected I4, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Invalid comparison between Unknown and I4
		if (rollCallManager != null)
		{
			ConnectionState val = rollCallManager.State;
			switch (val - 2)
			{
			case 0:
			case 5:
				return redCircle;
			case 1:
				if ((int)rollCallManager.Protocol == 71993 && rollCallManager.Load.HasValue && rollCallManager.Load.Value > 0f)
				{
					foreach (Channel item in (ChannelBaseCollection)Sapi.GetSapi().Channels)
					{
						if (item.Online && item.Ecu.IsUds)
						{
							return greenCircle;
						}
					}
				}
				return yellowCircle;
			case 4:
				if (!blinkOn)
				{
					return redCircle;
				}
				return yellowCircle;
			case 2:
				if (!blinkOn)
				{
					return yellowCircle;
				}
				return greenCircle;
			case 3:
				return greenCircle;
			}
		}
		return grayCircle;
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		Size suggestedTextSize = TextRenderer.MeasureText("J1939", Font);
		Rectangle rectangle = ImagePaintLocation(suggestedTextSize);
		Rectangle bounds = TextPaintLocation(rectangle, suggestedTextSize);
		SetTooltipIfClipped(bounds.Size);
		e.Graphics.DrawImage(GetImage(), rectangle);
		TextRenderer.DrawText(e.Graphics, Text, Font, bounds, SystemColors.WindowText, GetTextFormat(useEllipsis: true));
	}

	private void SetTooltipIfClipped(Size textTargetSize)
	{
		Size size = TextRenderer.MeasureText(Text, Font, textTargetSize, GetTextFormat(useEllipsis: false));
		if (textTargetSize.Width < size.Width || textTargetSize.Height < size.Height)
		{
			overflowTooltip.SetToolTip(this, Text);
		}
		else
		{
			overflowTooltip.RemoveAll();
		}
	}

	private static TextFormatFlags GetTextFormat(bool useEllipsis)
	{
		return (TextFormatFlags)(0x15 | (useEllipsis ? 262144 : 0));
	}

	private Rectangle ImagePaintLocation(Size suggestedTextSize)
	{
		if (textInside)
		{
			return base.ClientRectangle;
		}
		int num = 0;
		Point empty = Point.Empty;
		if (stacked)
		{
			num = Math.Min(base.ClientRectangle.Height, Math.Max(8, base.ClientRectangle.Height - suggestedTextSize.Height));
			num = Math.Min(num, base.ClientRectangle.Width);
			empty.X = base.ClientRectangle.X + (base.ClientRectangle.Width - num) / 2;
			empty.Y = base.ClientRectangle.Y;
		}
		else
		{
			num = Math.Min(base.ClientRectangle.Width, Math.Max(8, base.ClientRectangle.Width - suggestedTextSize.Width));
			num = Math.Min(num, base.ClientRectangle.Height);
			empty.X = base.ClientRectangle.X;
			empty.Y = base.ClientRectangle.Y + (base.ClientRectangle.Height - num) / 2;
		}
		return new Rectangle(empty, new Size(num, num));
	}

	private Rectangle TextPaintLocation(Rectangle imageLocation, Size suggestedTextSize)
	{
		if (textInside)
		{
			return base.ClientRectangle;
		}
		Point empty = Point.Empty;
		Size empty2 = Size.Empty;
		if (stacked)
		{
			empty.X = base.ClientRectangle.X;
			empty.Y = base.ClientRectangle.Y + base.ClientRectangle.Height - suggestedTextSize.Height;
			empty2.Width = base.ClientRectangle.Width;
			empty2.Height = suggestedTextSize.Height;
		}
		else
		{
			empty.X = base.ClientRectangle.X + imageLocation.Width;
			empty.Y = base.ClientRectangle.Y;
			empty2.Width = suggestedTextSize.Width;
			empty2.Height = base.ClientRectangle.Height;
		}
		return new Rectangle(empty, empty2);
	}

	protected override void OnCreateControl()
	{
		base.OnCreateControl();
		using Graphics graphics = CreateGraphics();
		defaultImageSize = new Size((int)((float)greenCircle.Width * graphics.DpiX / 96f) / 4, (int)((float)greenCircle.Height * graphics.DpiX / 96f) / 4);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		if (rollCallManager != null)
		{
			rollCallManager.StateChangedEvent -= rollCall_StateChangedEvent;
			rollCallManager.LoadChangedEvent -= rollCall_LoadChangedEvent;
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.overflowTooltip = new System.Windows.Forms.ToolTip(this.components);
		base.SuspendLayout();
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Name = "ConnectionState";
		base.Size = new System.Drawing.Size(211, 59);
		base.ResumeLayout(false);
	}
}
