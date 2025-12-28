using Softing.Dts;

namespace McdAbstraction;

public class McdTextTableElement
{
	public string Name { get; private set; }

	public McdValue LowerLimit { get; private set; }

	public McdValue UpperLimit { get; private set; }

	internal McdTextTableElement(MCDTextTableElement element)
	{
		Name = element.LongName;
		MCDInterval interval = element.Interval;
		LowerLimit = new McdValue(interval.LowerLimit);
		UpperLimit = new McdValue(interval.UpperLimit);
	}
}
