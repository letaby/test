using System;

namespace Softing.Dts;

public class PrimitiveCanceledDuringExecutionArgs : EventArgs
{
	private MCDDiagComPrimitive m_primitive;

	private MCDLogicalLink m_link;

	public MCDDiagComPrimitive Primitive => m_primitive;

	public MCDLogicalLink Link => m_link;

	public PrimitiveCanceledDuringExecutionArgs(MCDDiagComPrimitive primitive, MCDLogicalLink link)
	{
		m_primitive = primitive;
		m_link = link;
	}
}
