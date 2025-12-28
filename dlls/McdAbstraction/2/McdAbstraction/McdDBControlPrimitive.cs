using Softing.Dts;

namespace McdAbstraction;

public class McdDBControlPrimitive : McdDBDiagComPrimitive
{
	private string internalShortName;

	public string InternalShortName => internalShortName;

	internal McdDBControlPrimitive(MCDDbControlPrimitive controlPrimitive)
		: base(controlPrimitive)
	{
		if (controlPrimitive is DtsDbStartCommunication dtsDbStartCommunication)
		{
			internalShortName = dtsDbStartCommunication.InternalShortName;
		}
		else if (controlPrimitive is DtsDbStopCommunication dtsDbStopCommunication)
		{
			internalShortName = dtsDbStopCommunication.InternalShortName;
		}
	}
}
