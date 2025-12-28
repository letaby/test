using System;

namespace Softing.Dts;

public class LinkStateCommunicationArgs : EventArgs
{
	private MCDLogicalLink m_link;

	public MCDLogicalLink Link => m_link;

	public LinkStateCommunicationArgs(MCDLogicalLink link)
	{
		m_link = link;
	}
}
