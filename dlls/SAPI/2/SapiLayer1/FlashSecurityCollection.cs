using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SapiLayer1;

public sealed class FlashSecurityCollection : ReadOnlyCollection<FlashSecurity>
{
	internal FlashSecurityCollection()
		: base((IList<FlashSecurity>)new List<FlashSecurity>())
	{
	}

	internal void Add(FlashSecurity flashSecurityBlock)
	{
		base.Items.Add(flashSecurityBlock);
	}
}
