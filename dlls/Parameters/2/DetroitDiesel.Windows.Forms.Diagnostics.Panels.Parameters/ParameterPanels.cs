using System.Windows.Forms;
using DetroitDiesel.Help;
using DetroitDiesel.Reflection;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;

internal class ParameterPanels : PanelBase
{
	public ParameterPanels()
		: base((PanelIdentifier)6, Link.Empty)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		((Control)this).SuspendLayout();
		((Control)this).Name = "ParameterPanels";
		((Control)this).ResumeLayout(performLayout: false);
	}
}
