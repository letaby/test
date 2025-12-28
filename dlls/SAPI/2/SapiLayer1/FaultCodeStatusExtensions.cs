using System;
using System.Collections.Generic;

namespace SapiLayer1;

internal static class FaultCodeStatusExtensions
{
	private enum TriState
	{
		NotSet,
		Set,
		PreviouslySet
	}

	private static TriState GetTriState(bool setCondition, bool previouslySetCondition)
	{
		if (!setCondition)
		{
			if (!previouslySetCondition)
			{
				return TriState.NotSet;
			}
			return TriState.PreviouslySet;
		}
		return TriState.Set;
	}

	private static string ToDisplayString(this TriState status, bool confirmed, Ecu ecu)
	{
		switch (status)
		{
		case TriState.NotSet:
			if (!confirmed)
			{
				return Translate("not active", ecu);
			}
			return Translate("not confirmed", ecu);
		case TriState.Set:
			if (!confirmed)
			{
				return Translate("active", ecu);
			}
			return Translate("confirmed", ecu);
		case TriState.PreviouslySet:
			if (!confirmed)
			{
				return Translate("previously active", ecu);
			}
			return Translate("previously confirmed", ecu);
		default:
			throw new ArgumentException("Unknown status value.", "status");
		}
	}

	private static string Translate(string original, Ecu ecu)
	{
		return ecu.Translate(Sapi.MakeTranslationIdentifier(original.CreateQualifierFromName(), "FaultCodeStatus"), original);
	}

	public static string ToStatusString(this FaultCodeStatus status, bool obd, Ecu ecu)
	{
		if (status != FaultCodeStatus.None)
		{
			if (obd)
			{
				List<string> list = new List<string>();
				if ((status & FaultCodeStatus.Pending) != FaultCodeStatus.None)
				{
					list.Add(Translate("pending", ecu));
				}
				TriState triState = GetTriState((status & FaultCodeStatus.Mil) != 0, (status & FaultCodeStatus.Stored) != 0);
				if (triState != TriState.NotSet)
				{
					list.Add(triState.ToDisplayString(confirmed: true, ecu));
				}
				TriState triState2 = GetTriState((status & FaultCodeStatus.Active) != 0, (status & FaultCodeStatus.TestFailedSinceLastClear) != 0);
				if (triState2 != TriState.NotSet)
				{
					list.Add(triState2.ToDisplayString(confirmed: false, ecu));
				}
				if ((status & FaultCodeStatus.Permanent) != FaultCodeStatus.None)
				{
					list.Add(Translate("permanent", ecu));
				}
				if ((status & FaultCodeStatus.Immediate) != FaultCodeStatus.None)
				{
					list.Add(Translate("immediate", ecu));
				}
				return string.Join(", ", list);
			}
			return GetTriState((status & FaultCodeStatus.Active) != 0, (status & FaultCodeStatus.TestFailedSinceLastClear) != 0).ToDisplayString(confirmed: false, ecu);
		}
		return Translate("no fault", ecu);
	}
}
