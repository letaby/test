using System;

namespace Softing.Dts;

public class LinkUnlockedArgs : EventArgs
{
	private MCDLogicalLink m_link;

	public MCDLogicalLink Link => m_link;

	public LinkUnlockedArgs(MCDLogicalLink link)
	{
		m_link = link;
	}
}
