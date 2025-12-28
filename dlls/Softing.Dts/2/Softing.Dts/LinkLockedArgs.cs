using System;

namespace Softing.Dts;

public class LinkLockedArgs : EventArgs
{
	private MCDLogicalLink m_link;

	public MCDLogicalLink Link => m_link;

	public LinkLockedArgs(MCDLogicalLink link)
	{
		m_link = link;
	}
}
