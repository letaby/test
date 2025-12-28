using System;

namespace Softing.Dts;

public class LinkVariantIdentifiedArgs : EventArgs
{
	private MCDLogicalLink m_link;

	private MCDLogicalLinkState m_linkstate;

	public MCDLogicalLink Link => m_link;

	public MCDLogicalLinkState Linkstate => m_linkstate;

	public LinkVariantIdentifiedArgs(MCDLogicalLink link, MCDLogicalLinkState linkstate)
	{
		m_link = link;
		m_linkstate = linkstate;
	}
}
