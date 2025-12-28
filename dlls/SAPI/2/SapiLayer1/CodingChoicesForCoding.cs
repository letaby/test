using System.Collections.Generic;

namespace SapiLayer1;

public class CodingChoicesForCoding
{
	public IEnumerable<CodingChoice> CodingChoices { get; private set; }

	public Dump Coding { get; private set; }

	internal CodingChoicesForCoding(IEnumerable<CodingChoice> choices, Dump coding)
	{
		CodingChoices = choices;
		Coding = coding;
	}
}
