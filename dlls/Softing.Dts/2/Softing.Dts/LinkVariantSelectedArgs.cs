using System;

namespace Softing.Dts;

public class LinkVariantSelectedArgs : EventArgs
{
	private MCDLogicalLink m_link;

	private MCDLogicalLinkState m_linkstate;

	public MCDLogicalLink Link => m_link;

	public MCDLogicalLinkState Linkstate => m_linkstate;

	public LinkVariantSelectedArgs(MCDLogicalLink link, MCDLogicalLinkState linkstate)
	{
		m_link = link;
		m_linkstate = linkstate;
	}
}
