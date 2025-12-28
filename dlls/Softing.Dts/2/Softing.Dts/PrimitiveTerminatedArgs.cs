using System;

namespace Softing.Dts;

public class PrimitiveTerminatedArgs : EventArgs
{
	private MCDDiagComPrimitive m_primitive;

	private MCDLogicalLink m_link;

	private MCDResultState m_resultstate;

	public MCDDiagComPrimitive Primitive => m_primitive;

	public MCDLogicalLink Link => m_link;

	public MCDResultState Resultstate => m_resultstate;

	public PrimitiveTerminatedArgs(MCDDiagComPrimitive primitive, MCDLogicalLink link, MCDResultState resultstate)
	{
		m_primitive = primitive;
		m_link = link;
		m_resultstate = resultstate;
	}
}
