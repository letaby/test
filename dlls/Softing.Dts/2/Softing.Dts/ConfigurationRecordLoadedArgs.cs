using System;

namespace Softing.Dts;

public class ConfigurationRecordLoadedArgs : EventArgs
{
	private MCDConfigurationRecord m_configurationRecord;

	public MCDConfigurationRecord ConfigurationRecord => m_configurationRecord;

	public ConfigurationRecordLoadedArgs(MCDConfigurationRecord configurationRecord)
	{
		m_configurationRecord = configurationRecord;
	}
}
