using System;

namespace Softing.Dts;

public interface DtsDbFlashSession : MCDDbFlashSession, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsDbObject, DtsNamedObject, DtsObject
{
	string FlashJobName { get; }

	string[] AllVariantReferences { get; }

	string[] LayerReferences { get; }
}
