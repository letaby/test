using System;

namespace Softing.Dts;

public class InterfaceDetectedArgs : EventArgs
{
	private MCDInterface m_interface_;

	public MCDInterface Interface_ => m_interface_;

	public InterfaceDetectedArgs(MCDInterface interface_)
	{
		m_interface_ = interface_;
	}
}
