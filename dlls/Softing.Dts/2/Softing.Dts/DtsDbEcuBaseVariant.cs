using System;

namespace Softing.Dts;

public interface DtsDbEcuBaseVariant : MCDDbEcuBaseVariant, MCDDbEcu, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsDbEcu, DtsDbObject, DtsNamedObject, DtsObject
{
	MCDDbMatchingPatterns DbBaseVariantPatterns { get; }
}
