using System;

namespace Softing.Dts;

public class PrimitiveErrorArgs : EventArgs
{
	private MCDDiagComPrimitive m_primitive;

	private MCDLogicalLink m_link;

	private MCDError m_error;

	public MCDDiagComPrimitive Primitive => m_primitive;

	public MCDLogicalLink Link => m_link;

	public MCDError Error => m_error;

	public PrimitiveErrorArgs(MCDDiagComPrimitive primitive, MCDLogicalLink link, MCDError error)
	{
		m_primitive = primitive;
		m_link = link;
		m_error = error;
	}
}
