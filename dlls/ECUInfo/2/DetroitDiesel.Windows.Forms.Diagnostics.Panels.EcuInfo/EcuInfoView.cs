using System.ComponentModel;
using System.Windows.Forms;
using DetroitDiesel.Reflection;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.EcuInfo;

public class EcuInfoView : PanelBase
{
	public EcuInfoView()
		: base((PanelIdentifier)0)
	{
		InitializeComponent();
	}

	protected override void Dispose(bool disposing)
	{
		((PanelBase)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(EcuInfoView));
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Name = "EcuInfoView";
		((Control)this).ResumeLayout(performLayout: false);
	}
}
