using System;

namespace Softing.Dts;

public class LinkQueueClearedArgs : EventArgs
{
	private MCDLogicalLink m_link;

	private MCDLogicalLinkState m_linkstate;

	public MCDLogicalLink Link => m_link;

	public MCDLogicalLinkState Linkstate => m_linkstate;

	public LinkQueueClearedArgs(MCDLogicalLink link, MCDLogicalLinkState linkstate)
	{
		m_link = link;
		m_linkstate = linkstate;
	}
}
