using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SapiLayer1;

public sealed class EnvironmentDataCollection : ReadOnlyCollection<EnvironmentData>
{
	public EnvironmentData this[string qualifier] => this.FirstOrDefault((EnvironmentData item) => string.Equals(item.Qualifier, qualifier, StringComparison.Ordinal));

	internal EnvironmentDataCollection()
		: base((IList<EnvironmentData>)new List<EnvironmentData>())
	{
	}

	internal void Add(EnvironmentData environmentDataValue)
	{
		base.Items.Add(environmentDataValue);
	}
}
