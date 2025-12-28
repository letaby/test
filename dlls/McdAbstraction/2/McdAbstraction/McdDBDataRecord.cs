using System.Collections.Generic;
using Softing.Dts;

namespace McdAbstraction;

public class McdDBDataRecord
{
	private MCDDbDataRecord item;

	private IEnumerable<byte> binaryData;

	internal MCDDbDataRecord Handle => item;

	public IEnumerable<byte> BinaryData
	{
		get
		{
			if (binaryData == null)
			{
				binaryData = item.BinaryData;
			}
			return binaryData;
		}
	}

	public string Name => item.LongName;

	public string Qualifier => item.ShortName;

	public string Description => item.Description;

	public string PartNumber => item.Key;

	internal McdDBDataRecord(MCDDbDataRecord data)
	{
		item = data;
	}
}
