using System;

namespace Softing.Dts;

public class StaticInterfaceErrorArgs : EventArgs
{
	private MCDError m_error;

	public MCDError Error => m_error;

	public StaticInterfaceErrorArgs(MCDError error)
	{
		m_error = error;
	}
}
