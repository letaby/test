using Softing.Dts;

namespace McdAbstraction;

public class McdDBItemValue
{
	private MCDDbItemValue item;

	public string Meaning => item.Meaning;

	public string Description => item.Description;

	public string PartNumber => item.Key;

	public McdValue Value
	{
		get
		{
			try
			{
				return new McdValue(item.PhysicalConstantValue);
			}
			catch (MCDException)
			{
				return null;
			}
		}
	}

	internal MCDDbItemValue Handle => item;

	internal McdDBItemValue(MCDDbItemValue data)
	{
		item = data;
	}
}
