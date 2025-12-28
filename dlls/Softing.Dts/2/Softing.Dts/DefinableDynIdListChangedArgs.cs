using System;

namespace Softing.Dts;

public class DefinableDynIdListChangedArgs : EventArgs
{
	private MCDValues m_dynIdList;

	private MCDLogicalLink m_link;

	public MCDValues DynIdList => m_dynIdList;

	public MCDLogicalLink Link => m_link;

	public DefinableDynIdListChangedArgs(MCDValues dynIdList, MCDLogicalLink link)
	{
		m_dynIdList = dynIdList;
		m_link = link;
	}
}
