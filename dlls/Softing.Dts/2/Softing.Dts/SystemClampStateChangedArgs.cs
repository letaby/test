using System;

namespace Softing.Dts;

public class SystemClampStateChangedArgs : EventArgs
{
	private string m_clamp;

	private MCDClampState m_clampState;

	public string Clamp => m_clamp;

	public MCDClampState ClampState => m_clampState;

	public SystemClampStateChangedArgs(string clamp, MCDClampState clampState)
	{
		m_clamp = clamp;
		m_clampState = clampState;
	}
}
