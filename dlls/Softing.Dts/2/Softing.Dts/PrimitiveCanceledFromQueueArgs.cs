using System;

namespace Softing.Dts;

public class PrimitiveCanceledFromQueueArgs : EventArgs
{
	private MCDDiagComPrimitive m_primitive;

	private MCDLogicalLink m_link;

	public MCDDiagComPrimitive Primitive => m_primitive;

	public MCDLogicalLink Link => m_link;

	public PrimitiveCanceledFromQueueArgs(MCDDiagComPrimitive primitive, MCDLogicalLink link)
	{
		m_primitive = primitive;
		m_link = link;
	}
}
