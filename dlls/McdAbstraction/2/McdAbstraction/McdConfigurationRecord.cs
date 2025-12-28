using System;
using System.Collections.Generic;
using System.Linq;
using Softing.Dts;

namespace McdAbstraction;

public class McdConfigurationRecord : IDisposable
{
	private McdLogicalLink link;

	private MCDConfigurationRecord record;

	private McdDBConfigurationRecord dbRecord;

	private string qualifier;

	private Dictionary<string, McdOptionItem> optionItems;

	private bool disposedValue = false;

	internal MCDConfigurationRecord Handle => record;

	public McdDBConfigurationRecord DBRecord => dbRecord;

	public string Qualifier => qualifier;

	public IEnumerable<byte> ConfigurationRecord => record.ConfigurationRecord;

	public IEnumerable<McdOptionItem> OptionItems => optionItems.Values.ToList();

	internal McdConfigurationRecord(McdLogicalLink link, MCDConfigurationRecord record, McdDBConfigurationRecord dbRecord)
	{
		this.link = link;
		this.record = record;
		this.dbRecord = dbRecord;
		qualifier = record.ShortName;
		optionItems = new Dictionary<string, McdOptionItem>();
		foreach (MCDOptionItem optionItem in record.OptionItems)
		{
			optionItems.Add(optionItem.ShortName, new McdOptionItem(optionItem, this.dbRecord.DBOptionItems.FirstOrDefault((McdDBOptionItem oi) => oi.Qualifier == optionItem.ShortName)));
		}
	}

	public void SetConfigurationRecordByDBObject(McdDBDataRecord value)
	{
		try
		{
			record.ConfigurationRecordByDbObject = value.Handle;
		}
		catch (MCDException ex)
		{
			if (!value.BinaryData.SequenceEqual(ConfigurationRecord))
			{
				throw new McdException(ex, "ConfigurationRecordByDBObject");
			}
		}
	}

	internal McdDBDataRecord GetDefaultString(string partNumber)
	{
		return dbRecord.DBDataRecords.FirstOrDefault((McdDBDataRecord dr) => dr.PartNumber == partNumber);
	}

	internal Tuple<McdOptionItem, McdDBItemValue> GetFragment(string partNumber)
	{
		foreach (McdOptionItem value in optionItems.Values)
		{
			McdDBItemValue mcdDBItemValue = value.DBOptionItem.DBItemValues.FirstOrDefault((McdDBItemValue iv) => iv.PartNumber == partNumber);
			if (mcdDBItemValue != null)
			{
				return new Tuple<McdOptionItem, McdDBItemValue>(value, mcdDBItemValue);
			}
		}
		return null;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposedValue)
		{
			return;
		}
		if (disposing)
		{
			if (link != null)
			{
				link.RemoveConfigRecord(this);
				link = null;
			}
			foreach (McdOptionItem value in optionItems.Values)
			{
				value.Dispose();
			}
			optionItems.Clear();
			if (record != null)
			{
				record.Dispose();
				record = null;
			}
		}
		disposedValue = true;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
