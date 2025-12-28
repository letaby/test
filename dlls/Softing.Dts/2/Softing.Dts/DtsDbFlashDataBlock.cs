using System;

namespace Softing.Dts;

public interface DtsDbFlashDataBlock : MCDDbFlashDataBlock, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsDbObject, DtsNamedObject, DtsObject
{
	MCDDbFlashSecurities DbSecuritiesAsSecurities { get; }

	void LoadSegments(string filename);
}
