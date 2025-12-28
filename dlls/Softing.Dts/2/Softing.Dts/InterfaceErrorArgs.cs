using System;

namespace Softing.Dts;

public class InterfaceErrorArgs : EventArgs
{
	private MCDInterface m_interface_;

	private MCDError m_error;

	public MCDInterface Interface_ => m_interface_;

	public MCDError Error => m_error;

	public InterfaceErrorArgs(MCDInterface interface_, MCDError error)
	{
		m_interface_ = interface_;
		m_error = error;
	}
}
