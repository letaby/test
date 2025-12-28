using System;

namespace Softing.Dts;

public class LinkStateCreatedArgs : EventArgs
{
	private MCDLogicalLink m_link;

	public MCDLogicalLink Link => m_link;

	public LinkStateCreatedArgs(MCDLogicalLink link)
	{
		m_link = link;
	}
}
