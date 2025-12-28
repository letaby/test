using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace SapiLayer1;

public sealed class ConnectionResourceCollection : ReadOnlyCollection<ConnectionResource>
{
	public ConnectionResource this[string identifier] => this.FirstOrDefault((ConnectionResource resource) => string.Equals(resource.Type, identifier, StringComparison.Ordinal) || resource.Equals(identifier));

	internal ConnectionResourceCollection()
		: base((IList<ConnectionResource>)new List<ConnectionResource>())
	{
	}

	internal void Add(ConnectionResource connectionResource)
	{
		base.Items.Add(connectionResource);
	}

	public ConnectionResource GetResource(string type, int portIndex)
	{
		return this.Where((ConnectionResource resource) => string.Equals(resource.Type, type, StringComparison.Ordinal) && resource.PortIndex == portIndex).FirstOrDefault();
	}

	public ConnectionResource GetEquivalent(ConnectionResource other)
	{
		if (other == null)
		{
			return null;
		}
		return this.FirstOrDefault((ConnectionResource resource) => resource.IsEquivalent(other));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("get_Equivalent is deprecated, please use GetEquivalent(ConnectionResource) instead.")]
	public ConnectionResource get_Equivalent(ConnectionResource other)
	{
		return GetEquivalent(other);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("get_Resource is deprecated, please use GetResource(string, int) instead.")]
	public ConnectionResource get_Resource(string type, int portIndex)
	{
		return GetResource(type, portIndex);
	}
}
