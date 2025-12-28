using System;
using System.Collections.Generic;
using System.Linq;
using Softing.Dts;

namespace McdAbstraction;

public class McdDBOptionItem
{
	private MCDDbOptionItem item;

	private IEnumerable<McdDBItemValue> itemValues;

	private Type type;

	private IEnumerable<McdTextTableElement> textTableElements;

	public IEnumerable<McdDBItemValue> DBItemValues
	{
		get
		{
			if (itemValues == null)
			{
				List<McdDBItemValue> list = new List<McdDBItemValue>();
				if (item.DataType != MCDDataType.eNO_TYPE)
				{
					list.AddRange(from oi in item.DbItemValues.OfType<MCDDbItemValue>()
						select new McdDBItemValue(oi));
				}
				itemValues = list;
			}
			return itemValues;
		}
	}

	public string Name => item.LongName;

	public string Qualifier => item.ShortName;

	public string Description => item.Description;

	public int BytePos => (int)item.BytePos;

	public int BitPos => item.BitPos;

	public int? BitLength => (DataType != null) ? new int?((int)item.BitLength) : ((int?)null);

	public Type DataType
	{
		get
		{
			if (type == null && item.DataType != MCDDataType.eNO_TYPE)
			{
				type = McdRoot.MapDataType(item.DataType);
			}
			return type;
		}
	}

	public IEnumerable<McdTextTableElement> TextTableElements
	{
		get
		{
			if (textTableElements == null && DataType == typeof(McdTextTableElement))
			{
				textTableElements = (from tt in item.TextTableElements.OfType<MCDTextTableElement>()
					select new McdTextTableElement(tt)).ToList();
			}
			return textTableElements;
		}
	}

	internal McdDBOptionItem(MCDDbOptionItem data)
	{
		item = data;
	}
}
