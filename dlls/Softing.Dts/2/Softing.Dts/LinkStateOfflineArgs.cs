using System;

namespace Softing.Dts;

public class LinkStateOfflineArgs : EventArgs
{
	private MCDLogicalLink m_link;

	public MCDLogicalLink Link => m_link;

	public LinkStateOfflineArgs(MCDLogicalLink link)
	{
		m_link = link;
	}
}
