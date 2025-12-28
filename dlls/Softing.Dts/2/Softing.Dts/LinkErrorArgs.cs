using System;

namespace Softing.Dts;

public class LinkErrorArgs : EventArgs
{
	private MCDLogicalLink m_link;

	private MCDError m_error;

	public MCDLogicalLink Link => m_link;

	public MCDError Error => m_error;

	public LinkErrorArgs(MCDLogicalLink link, MCDError error)
	{
		m_link = link;
		m_error = error;
	}
}
