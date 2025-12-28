using System;

namespace Softing.Dts;

public class InterfaceStatusChangedArgs : EventArgs
{
	private MCDInterface m_interface_;

	private MCDInterfaceStatus m_status;

	public MCDInterface Interface_ => m_interface_;

	public MCDInterfaceStatus Status => m_status;

	public InterfaceStatusChangedArgs(MCDInterface interface_, MCDInterfaceStatus status)
	{
		m_interface_ = interface_;
		m_status = status;
	}
}
