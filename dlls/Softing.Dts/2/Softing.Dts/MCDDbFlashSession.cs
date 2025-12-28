using System;

namespace Softing.Dts;

public interface MCDDbFlashSession : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbFlashSessionClasses DbFlashSessionsClasses { get; }

	string FlashKey { get; }

	MCDDbFlashChecksums Checksums { get; }

	MCDDbFlashIdents DbExpectedIdents { get; }

	MCDDbFlashSecurities DbSecurities { get; }

	MCDDbFlashDataBlocks DbDataBlocks { get; }

	MCDDbFlashJob DbFlashJob { get; }

	uint Priority { get; }

	bool IsDownload { get; }

	MCDDbSpecialDataGroups DbSDGs { get; }

	MCDDbFlashJob GetDbFlashJobByLocation(MCDDbLocation pDtsDbLocation);
}
