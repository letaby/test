using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using DetroitDiesel.Windows.Forms.Themed;
using SapiLayer1;

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
		get
		{
			return base.Text;
		}
		set
		{
			base.Text = value;
		}
	}

	public string Identifier => identifier;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Ecu Ecu
	{
		get
		{
			return ecu;
		}
		set
		{
			if (ecu != value)
			{
				ecu = value;
				descriptionText = ((!string.IsNullOrEmpty(ecu.ShortDescription)) ? ecu.ShortDescription : ecu.Name);
				Invalidate();
			}
		}
	}

	[Category("Appearance")]
	[Description("The description of the Ecu.")]
	[DefaultValue(typeof(string), "<no description>")]
	public string DescriptionText => descriptionText;

	[Category("Appearance")]
	[Description("The status of the Ecu.")]
	[DefaultValue(typeof(string), "<no status>")]
	public string StatusText
	{
		get
		{
			return statusText;
		}
		set
		{
			if (statusText != value)
			{
				statusText = value;
				Invalidate();
			}
		}
	}

	[Category("Appearance")]
	[Description("The connection icon to show.")]
	[DefaultValue(ConnectionIcon.Gray)]
	public ConnectionIcon Icon
	{
		get
		{
			return icon;
		}
		set
		{
			icon = value;
			switch (icon)
			{
			case ConnectionIcon.Gray:
				image = Resources.Gray1;
				break;
			case ConnectionIcon.Green:
				image = Resources.Green2;
				break;
			case ConnectionIcon.Red:
				image = Resources.Red2;
				break;
			case ConnectionIcon.Yellow:
				image = Resources.Yellow2;
				break;
			}
			Invalidate();
		}
	}

	public NotificationItem Notification => notification;

	public override Font Font
	{
		get
		{
			return base.Font;
		}
		set
		{
			if (value != base.Font)
			{
				base.Font = value;
				descriptionFont = new Font(value, FontStyle.Bold);
				notification.Font = new Font(value.FontFamily, value.SizeInPoints * 0.7f);
				Size size = TextRenderer.MeasureText(Resources.EcuStatusView_Writing, descriptionFont);
				preferredHeight = size.Height * 2;
				imageSize = new Size(size.Height * 4 / 3, size.Height * 4 / 3);
			}
		}
	}

	public EcuStatusItem(string identifier)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		Font = SystemFonts.MessageBoxFont;
		this.identifier = identifier;
		SetStyle(ControlStyles.ResizeRedraw, value: true);
		SetStyle(ControlStyles.DoubleBuffer, value: true);
		SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
		SetStyle(ControlStyles.UserPaint, value: true);
		SetStyle(ControlStyles.Selectable, value: true);
		InitializeComponent();
		((ToolTip)(object)toolTip).AutomaticDelay = 0;
		((ToolTip)(object)toolTip).ShowAlways = true;
		notification.ContentChanged += notificationItem_ContentChanged;
	}

	private void notificationItem_ContentChanged(object sender, EventArgs e)
	{
		Invalidate();
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		base.OnMouseMove(e);
		if (!toolTipDisplayed)
		{
			string text = ((!ecu.IsRollCall) ? DescriptionText : string.Format(CultureInfo.CurrentCulture, Resources.EcuStatusItem_DescriptionToolTipGenericFormat, DescriptionText, (ecu.Name != Identifier) ? (" " + Identifier) : string.Empty));
			((ToolTip)(object)toolTip).Show((text + Environment.NewLine + StatusText).Trim(), (IWin32Window)this);
			toolTipDisplayed = true;
		}
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		base.OnMouseLeave(e);
		if (toolTipDisplayed)
		{
			((ToolTip)(object)toolTip).Hide((IWin32Window)this);
			toolTipDisplayed = false;
		}
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		Rectangle rect = new Rectangle(base.ClientRectangle.X, base.ClientRectangle.Y + (base.ClientRectangle.Height - imageSize.Height) / 2, imageSize.Width, imageSize.Height);
		e.Graphics.DrawImage(image, rect);
		Rectangle rectangle = new Rectangle(base.ClientRectangle.X + rect.Width, base.ClientRectangle.Y, base.ClientRectangle.Width - rect.Width, base.ClientRectangle.Height);
		TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter;
		Rectangle rectangle2 = rectangle;
		rectangle2.Height /= 2;
		Rectangle bounds = rectangle2;
		bounds.Y = rectangle.Y + rectangle2.Height;
		TextRenderer.DrawText(e.Graphics, GetFirstLine(descriptionText), descriptionFont, rectangle2, ForeColor, flags);
		TextRenderer.DrawText(e.Graphics, GetFirstLine(statusText), Font, bounds, ForeColor, flags);
		if (notification.Count > 0)
		{
			notification.Draw(e.Graphics, new Rectangle(new Point(rect.Right - notification.Size.Width, rect.Bottom - notification.Size.Height), notification.Size));
		}
		if (ecu != null && ecu.IsRollCall)
		{
			e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(96, SystemColors.Window)), base.ClientRectangle);
		}
		if (Focused)
		{
			ControlPaint.DrawFocusRectangle(e.Graphics, base.ClientRectangle);
		}
	}

	protected override void OnGotFocus(EventArgs e)
	{
		base.OnGotFocus(e);
		Invalidate();
	}

	protected override void OnLostFocus(EventArgs e)
	{
		base.OnLostFocus(e);
		Invalidate();
	}

	private static string GetFirstLine(string input)
	{
		if (input != null && input.Contains(Environment.NewLine))
		{
			return input.Substring(0, input.IndexOf(Environment.NewLine, StringComparison.Ordinal));
		}
		return input;
	}

	public override Size GetPreferredSize(Size proposedSize)
	{
		return new Size(proposedSize.Width, preferredHeight);
	}

	public void Copy(ClipboardData data)
	{
		data.Text.Append(DescriptionText);
		data.Csv.Append(DescriptionText);
		data.SymbolicLink.AppendCellData(DescriptionText);
		data.Text.Append("\t");
		data.Csv.Append(",");
		data.SymbolicLink.AdvanceColumn();
		data.Text.Append(StatusText);
		data.Csv.Append(StatusText);
		data.SymbolicLink.AppendCellData(StatusText);
		data.Text.Append(Environment.NewLine);
		data.Csv.Append(Environment.NewLine);
		data.SymbolicLink.AdvanceRow();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (components != null)
			{
				components.Dispose();
			}
			if (descriptionFont != null)
			{
				descriptionFont.Dispose();
				descriptionFont = null;
			}
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		base.SuspendLayout();
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.AutoSize = true;
		base.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.BackColor = System.Drawing.Color.Transparent;
		base.Margin = new System.Windows.Forms.Padding(0);
		base.Name = "EcuStatusItem";
		base.Size = new System.Drawing.Size(0, 0);
		base.ResumeLayout(false);
	}
}
