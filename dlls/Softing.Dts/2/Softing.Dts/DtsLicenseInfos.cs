using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsLicenseInfos : DtsCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsObject
{
	DtsLicenseInfo GetItemByIndex(uint index);
}
