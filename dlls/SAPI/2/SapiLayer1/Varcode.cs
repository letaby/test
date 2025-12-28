using System;

namespace SapiLayer1;

internal abstract class Varcode : IDisposable
{
	protected bool disposedValue;

	internal bool IsErrorSet => Exception != null;

	internal CaesarException Exception { get; set; }

	internal abstract void DoCoding();

	internal abstract bool AllowSetDefaultString(string groupQualifier);

	internal abstract void EnableReadCodingStringFromEcu(bool enableReadCodingStringFromEcu);

	internal abstract byte[] GetCurrentCodingString(string groupQualifier);

	internal abstract void SetCurrentCodingString(string groupQualifier, byte[] content);

	internal abstract void SetDefaultStringByPartNumber(string partNumber);

	internal abstract void SetDefaultStringByPartNumberAndPartVersion(string partNumber, uint partVersion);

	internal abstract void SetFragmentMeaningByPartNumber(string partNumber);

	internal abstract void SetFragmentMeaningByPartNumberAndPartVersion(string partNumber, uint partVersion);

	internal abstract void SetFragmentValue(Parameter parameter, object value);

	internal abstract object GetFragmentValue(Parameter parameter);

	protected abstract void Dispose(bool disposing);

	public void Dispose()
	{
		Dispose(disposing: true);
	}
}
