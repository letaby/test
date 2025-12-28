using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SapiLayer1;

public sealed class ServiceInputValueCollection : ReadOnlyCollection<ServiceInputValue>
{
	private Service service;

	public ServiceInputValue this[string qualifier]
	{
		get
		{
			ServiceInputValue serviceInputValue = this.FirstOrDefault((ServiceInputValue item) => item.Qualifier.CompareNoCase(qualifier) || item.ParameterQualifier.CompareNoCase(qualifier));
			if (serviceInputValue == null && service.Channel.Ecu.AlternateQualifiers.TryGetValue(qualifier, out var alternateQualifier))
			{
				serviceInputValue = this.FirstOrDefault((ServiceInputValue item) => item.Qualifier.CompareNoCase(alternateQualifier) || item.ParameterQualifier.CompareNoCase(alternateQualifier));
			}
			return serviceInputValue;
		}
	}

	internal ServiceInputValueCollection(Service service)
		: base((IList<ServiceInputValue>)new List<ServiceInputValue>())
	{
		this.service = service;
	}

	internal void Add(ServiceInputValue serviceInputValue)
	{
		base.Items.Add(serviceInputValue);
	}

	internal Exception InternalParseValues(string data, Dictionary<string, string> variables = null)
	{
		Exception ex = null;
		if (data.Contains("(") && data.Contains(")"))
		{
			int num = data.IndexOf('(') + 1;
			int num2 = data.IndexOf(')') - num;
			if (num2 > 0)
			{
				string[] array = data.Substring(num, num2).Split(",".ToCharArray(), StringSplitOptions.None);
				if (array.All((string i) => i.Contains("=")))
				{
					for (int num3 = 0; num3 < array.Length; num3++)
					{
						if (ex != null)
						{
							break;
						}
						string[] array2 = array[num3].Split("=".ToCharArray(), StringSplitOptions.None);
						ServiceInputValue serviceInputValue = this[array2[0]];
						if (serviceInputValue != null)
						{
							ex = serviceInputValue.InternalSetValue(array2[1], variables);
						}
					}
				}
				else if (array.Length <= base.Count)
				{
					for (int num4 = 0; num4 < array.Length; num4++)
					{
						if (ex != null)
						{
							break;
						}
						ServiceInputValue serviceInputValue2 = base[num4];
						if (!string.IsNullOrEmpty(array[num4]))
						{
							ex = serviceInputValue2.InternalSetValue(array[num4], variables);
						}
					}
				}
				else
				{
					ex = new InvalidOperationException("Too many input arguments provided");
				}
			}
		}
		return ex;
	}

	public void ParseValues(string inputValues)
	{
		Exception ex = InternalParseValues(inputValues);
		if (ex != null)
		{
			throw ex;
		}
	}
}
