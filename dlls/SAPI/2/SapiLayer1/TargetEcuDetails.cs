namespace SapiLayer1;

public sealed class TargetEcuDetails
{
	public string Ecu { get; private set; }

	public string DiagnosisVariant { get; private set; }

	public string AssumedDiagnosisVariant { get; private set; }

	public int AssumedUnknownCount { get; private set; }

	internal TargetEcuDetails(string ecu, string diagnosisVariant, string assumedDiagnosisVariant, int assumedUnknownCount)
	{
		Ecu = ecu;
		DiagnosisVariant = diagnosisVariant;
		AssumedDiagnosisVariant = assumedDiagnosisVariant;
		AssumedUnknownCount = assumedUnknownCount;
	}
}
