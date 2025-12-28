using System;

namespace Softing.Dts;

public class SystemErrorArgs : EventArgs
{
	private MCDError m_error;

	public MCDError Error => m_error;

	public SystemErrorArgs(MCDError error)
	{
		m_error = error;
	}
}
