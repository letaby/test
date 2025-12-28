using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SapiLayer1;

public sealed class ServiceOutputValueCollection : ReadOnlyCollection<ServiceOutputValue>
{
	public ServiceOutputValue this[string qualifier]
	{
		get
		{
			using (IEnumerator<ServiceOutputValue> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ServiceOutputValue current = enumerator.Current;
					if (string.Equals(current.Name, qualifier, StringComparison.Ordinal) || (current.ParameterQualifier != null && string.Equals(current.ParameterQualifier, qualifier, StringComparison.Ordinal)))
					{
						return current;
					}
				}
			}
			return null;
		}
	}

	internal ServiceOutputValueCollection()
		: base((IList<ServiceOutputValue>)new List<ServiceOutputValue>())
	{
	}

	internal void Add(ServiceOutputValue serviceOutputValue)
	{
		if (serviceOutputValue.Service.CombinedService != null && serviceOutputValue.BytePosition.HasValue && serviceOutputValue.BitPosition.HasValue)
		{
			int num = serviceOutputValue.BytePosition.Value * 8 + serviceOutputValue.BitPosition.Value;
			foreach (ServiceOutputValue item in base.Items)
			{
				int num2 = item.BytePosition.Value * 8 + item.BitPosition.Value;
				if (num < num2)
				{
					base.Items.Insert(base.Items.IndexOf(item), serviceOutputValue);
					return;
				}
			}
		}
		base.Items.Add(serviceOutputValue);
	}
}
