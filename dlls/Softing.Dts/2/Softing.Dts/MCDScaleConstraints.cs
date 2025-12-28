using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDScaleConstraints : MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDScaleConstraint> ToList();

	MCDScaleConstraint[] ToArray();

	MCDScaleConstraint GetItemByIndex(uint index);
}
