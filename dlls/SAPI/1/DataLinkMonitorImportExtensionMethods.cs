// Decompiled with JetBrains decompiler
// Type: SapiLayer1.DataLinkMonitorImportExtensionMethods
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

#nullable disable
namespace SapiLayer1;

internal static class DataLinkMonitorImportExtensionMethods
{
  internal static T GetNamedPropertyValue<T>(
    this IDictionary<string, string> elements,
    string name,
    T defaultIfNotSet)
  {
    if (!elements.ContainsKey(name))
      return defaultIfNotSet;
    Type underlyingType = Nullable.GetUnderlyingType(typeof (T));
    return (T) Convert.ChangeType((object) elements[name], underlyingType != (Type) null ? underlyingType : typeof (T), (IFormatProvider) CultureInfo.InvariantCulture);
  }

  internal static string GetAttribute(this XElement element, string name)
  {
    return element.Attribute((XName) name)?.Value;
  }
}
