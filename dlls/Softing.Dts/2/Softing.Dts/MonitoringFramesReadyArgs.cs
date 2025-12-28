using System;

namespace Softing.Dts;

public class MonitoringFramesReadyArgs : EventArgs
{
	private MCDMonitoringLink m_monLink;

	public MCDMonitoringLink MonLink => m_monLink;

	public MonitoringFramesReadyArgs(MCDMonitoringLink monLink)
	{
		m_monLink = monLink;
	}
}
