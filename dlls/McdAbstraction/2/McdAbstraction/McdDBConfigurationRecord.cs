using System.Collections.Generic;
using System.Linq;
using Softing.Dts;

namespace McdAbstraction;

public class McdDBConfigurationRecord
{
	private MCDDbConfigurationRecord configurationRecord;

	private IEnumerable<McdDBDataRecord> dataRecords;

	private IEnumerable<McdDBOptionItem> optionItems;

	public IEnumerable<McdDBDataRecord> DBDataRecords
	{
		get
		{
			if (dataRecords == null)
			{
				dataRecords = (from cr in configurationRecord.DbDataRecords.OfType<MCDDbDataRecord>()
					select new McdDBDataRecord(cr)).ToList();
			}
			return dataRecords;
		}
	}

	public IEnumerable<McdDBOptionItem> DBOptionItems
	{
		get
		{
			if (optionItems == null)
			{
				try
				{
					optionItems = (from oi in configurationRecord.DbOptionItems.OfType<MCDDbOptionItem>()
						select new McdDBOptionItem(oi)).ToList();
				}
				catch (DtsSystemException ex)
				{
					throw new McdException(ex.Message, ex);
				}
				catch (DtsProgramViolationException ex2)
				{
					throw new McdException(ex2.Message, ex2);
				}
			}
			return optionItems;
		}
	}

	public string Name => configurationRecord.LongName;

	internal MCDDbConfigurationRecord Handle => configurationRecord;

	public string Qualifier => configurationRecord.ShortName;

	public string Description => configurationRecord.Description;

	public int ByteLength => (int)configurationRecord.ByteLength;

	internal McdDBConfigurationRecord(MCDDbConfigurationRecord data)
	{
		configurationRecord = data;
	}
}
