using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SapiLayer1;

public sealed class FlashMeaningCollection : ReadOnlyCollection<FlashMeaning>
{
	public FlashMeaning this[string flashKey] => this.FirstOrDefault((FlashMeaning item) => string.Equals(item.FlashKey, flashKey, StringComparison.Ordinal));

	internal FlashMeaningCollection()
		: base((IList<FlashMeaning>)new List<FlashMeaning>())
	{
	}

	internal void Add(FlashMeaning meaning)
	{
		base.Items.Add(meaning);
	}
}
