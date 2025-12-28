using System;

namespace Softing.Dts;

public interface DtsOfflineVariantCoding : DtsObject, MCDObject, IDisposable
{
	MCDValue GetCodingString(string domain);

	void SetCodingString(string domain, MCDValue codingString);

	MCDValue GetInternalMeaning(string domain, string fragment);

	void SetInternalMeaning(string domain, string fragment, MCDValue meaning);

	MCDValue GetExternalMeaning(string domain, string fragment);

	void SetExternalMeaning(string domain, string fragment, MCDValue meaning);
}
