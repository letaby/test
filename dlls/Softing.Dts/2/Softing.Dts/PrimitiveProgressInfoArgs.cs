using System;

namespace Softing.Dts;

public class PrimitiveProgressInfoArgs : EventArgs
{
	private MCDDiagComPrimitive m_primitive;

	private MCDLogicalLink m_link;

	private byte m_progress;

	public MCDDiagComPrimitive Primitive => m_primitive;

	public MCDLogicalLink Link => m_link;

	public byte Progress => m_progress;

	public PrimitiveProgressInfoArgs(MCDDiagComPrimitive primitive, MCDLogicalLink link, byte progress)
	{
		m_primitive = primitive;
		m_link = link;
		m_progress = progress;
	}
}
