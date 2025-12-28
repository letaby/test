using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace SapiLayer1;

internal static class DataLinkMonitorImportExtensionMethods
{
	internal static T GetNamedPropertyValue<T>(this IDictionary<string, string> elements, string name, T defaultIfNotSet)
	{
		if (elements.ContainsKey(name))
		{
			Type underlyingType = Nullable.GetUnderlyingType(typeof(T));
			return (T)Convert.ChangeType(elements[name], (underlyingType != null) ? underlyingType : typeof(T), CultureInfo.InvariantCulture);
		}
		return defaultIfNotSet;
	}

	internal static string GetAttribute(this XElement element, string name)
	{
		return element.Attribute(name)?.Value;
	}
}
