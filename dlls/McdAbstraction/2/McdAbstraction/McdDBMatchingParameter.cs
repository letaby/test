using Softing.Dts;

namespace McdAbstraction;

public class McdDBMatchingParameter
{
	private MCDDbMatchingParameter matchingParameter;

	private string primitiveQualifier;

	private string responseQualifier;

	private McdValue expectedValue;

	public string Primitive => primitiveQualifier;

	public string ResponseParameter => responseQualifier;

	public McdValue ExpectedValue => expectedValue;

	internal McdDBMatchingParameter(MCDDbMatchingParameter parameter)
	{
		matchingParameter = parameter;
		primitiveQualifier = matchingParameter.DbDiagComPrimitive.ShortName;
		responseQualifier = matchingParameter.DbResponseParameter.ShortName;
		expectedValue = new McdValue(matchingParameter.ExpectedValue);
	}

	public bool IsMatch(McdLogicalLink link)
	{
		McdValue variantIdentificationResult = link.GetVariantIdentificationResult(primitiveQualifier, responseQualifier);
		if (variantIdentificationResult != null && variantIdentificationResult.Value != null && expectedValue != null && expectedValue.Value != null)
		{
			return object.Equals(variantIdentificationResult.Value, expectedValue.Value);
		}
		return false;
	}
}
