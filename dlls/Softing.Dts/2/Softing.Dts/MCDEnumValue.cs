using System;

namespace Softing.Dts;

public interface MCDEnumValue : MCDObject, IDisposable
{
	int GetEnumFromString(string enumString);

	string GetStringFromEnum(int enumValue);
}
