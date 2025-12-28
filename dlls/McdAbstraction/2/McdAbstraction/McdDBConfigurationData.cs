using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Softing.Dts;

namespace McdAbstraction;

public class McdDBConfigurationData
{
	private MCDDbConfigurationData configurationData;

	private IEnumerable<McdDBConfigurationRecord> records;

	private string databaseFile;

	private string qualifier;

	private string version;

	private IEnumerable<string> ecus;

	private IEnumerable<string> variants;

	public IEnumerable<McdDBConfigurationRecord> DBConfigurationRecords
	{
		get
		{
			if (records == null)
			{
				records = from cr in configurationData.DbConfigurationRecords.OfType<MCDDbConfigurationRecord>()
					select new McdDBConfigurationRecord(cr);
			}
			return records;
		}
	}

	public string Name => configurationData.LongName;

	public string Qualifier => qualifier;

	public string Description => configurationData.Description;

	public string Version => version;

	public string DatabaseFile => databaseFile;

	public string Preamble => (!string.IsNullOrEmpty(DatabaseFile)) ? McdRoot.GetPreamble(DatabaseFile) : null;

	public IEnumerable<string> EcuNames
	{
		get
		{
			if (ecus == null)
			{
				ecus = configurationData.DbEcuBaseVariants.Names;
			}
			return ecus;
		}
	}

	public IEnumerable<string> EcuVariantNames
	{
		get
		{
			if (variants == null)
			{
				variants = configurationData.DbEcuVariants.Names;
			}
			return variants;
		}
	}

	internal McdDBConfigurationData(MCDDbConfigurationData data)
	{
		configurationData = data;
		qualifier = data.ShortName;
		version = string.Format(CultureInfo.InvariantCulture, "{0:00}.{1:00}.{2:00}", data.Version.Major, data.Version.Minor, data.Version.Revision);
		databaseFile = McdRoot.DatabaseFileList.FirstOrDefault((Tuple<string, string, string> fl) => fl.Item1.EndsWith("smr-e", StringComparison.OrdinalIgnoreCase) && fl.Item2 == qualifier && fl.Item3 == version)?.Item1;
	}
}
