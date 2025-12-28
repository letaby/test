using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SapiLayer1;

public sealed class FlashDataBlockCollection : ReadOnlyCollection<FlashDataBlock>
{
	public FlashDataBlock this[string qualifier] => this.FirstOrDefault((FlashDataBlock item) => string.Equals(item.Qualifier, qualifier, StringComparison.Ordinal));

	internal FlashDataBlockCollection()
		: base((IList<FlashDataBlock>)new List<FlashDataBlock>())
	{
	}

	internal void Add(FlashDataBlock flashDataBlock)
	{
		base.Items.Add(flashDataBlock);
	}
}
