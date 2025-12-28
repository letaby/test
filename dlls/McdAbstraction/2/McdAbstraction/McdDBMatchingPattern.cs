using System.Collections.Generic;
using System.Linq;
using Softing.Dts;

namespace McdAbstraction;

public class McdDBMatchingPattern
{
	private MCDDbMatchingPattern matchingPattern;

	private IEnumerable<McdDBMatchingParameter> matchingParameters;

	public IEnumerable<McdDBMatchingParameter> DBMatchingParameters
	{
		get
		{
			if (matchingParameters == null)
			{
				matchingParameters = (from mp in matchingPattern.DbMatchingParameters.OfType<MCDDbMatchingParameter>()
					select new McdDBMatchingParameter(mp)).ToList();
			}
			return matchingParameters;
		}
	}

	internal McdDBMatchingPattern(MCDDbMatchingPattern pattern)
	{
		matchingPattern = pattern;
	}
}
