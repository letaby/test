using System;

namespace Softing.Dts;

public class LinkStateOnlineArgs : EventArgs
{
	private MCDLogicalLink m_link;

	public MCDLogicalLink Link => m_link;

	public LinkStateOnlineArgs(MCDLogicalLink link)
	{
		m_link = link;
	}
}
