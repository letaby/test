using System;
using Softing.Dts;

namespace McdAbstraction;

public class McdOptionItem : IDisposable
{
	private MCDOptionItem optionItem;

	private McdDBOptionItem dbOptionItem;

	private bool disposedValue = false;

	public McdDBOptionItem DBOptionItem => dbOptionItem;

	internal McdOptionItem(MCDOptionItem optionItem, McdDBOptionItem dbOptionItem)
	{
		this.optionItem = optionItem;
		this.dbOptionItem = dbOptionItem;
	}

	public void SetItemValueByDBObject(McdDBItemValue value)
	{
		try
		{
			optionItem.ItemValueByDbObject = value.Handle;
		}
		catch (MCDException ex)
		{
			throw new McdException(ex, "ItemValueByDBObject");
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing && optionItem != null)
			{
				optionItem.Dispose();
				optionItem = null;
			}
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
