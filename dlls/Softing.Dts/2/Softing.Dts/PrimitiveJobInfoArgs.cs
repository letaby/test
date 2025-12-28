using System;

namespace Softing.Dts;

public class PrimitiveJobInfoArgs : EventArgs
{
	private MCDDiagComPrimitive m_primitive;

	private MCDLogicalLink m_link;

	private string m_info;

	public MCDDiagComPrimitive Primitive => m_primitive;

	public MCDLogicalLink Link => m_link;

	public string Info => m_info;

	public PrimitiveJobInfoArgs(MCDDiagComPrimitive primitive, MCDLogicalLink link, string info)
	{
		m_primitive = primitive;
		m_link = link;
		m_info = info;
	}
}
