namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.XCPCCPActivation.panel;

internal class VedocReadInputDataQualifiers
{
	public string ChallengeCodeQualifier;

	public string IdCodeQualifier;

	public string NumberOfCodesQualifier;

	public string TransponderCodeQualifier;

	private VedocReadInputDataQualifiers(string challengeCodeQualifier, string idCodeQualifier, string numberOfCodesQualifier, string transponderCodeQualifier)
	{
		ChallengeCodeQualifier = challengeCodeQualifier;
		IdCodeQualifier = idCodeQualifier;
		NumberOfCodesQualifier = numberOfCodesQualifier;
		TransponderCodeQualifier = transponderCodeQualifier;
	}

	public static VedocReadInputDataQualifiers Create(string challengeCodeQualifier, string idCodeQualifier, string numberOfCodesQualifier, string transponderCodeQualifier)
	{
		return new VedocReadInputDataQualifiers(challengeCodeQualifier, idCodeQualifier, numberOfCodesQualifier, transponderCodeQualifier);
	}
}
