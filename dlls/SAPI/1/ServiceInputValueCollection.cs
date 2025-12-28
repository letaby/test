// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ServiceInputValueCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class ServiceInputValueCollection : ReadOnlyCollection<ServiceInputValue>
{
  private Service service;

  internal ServiceInputValueCollection(Service service)
    : base((IList<ServiceInputValue>) new List<ServiceInputValue>())
  {
    this.service = service;
  }

  internal void Add(ServiceInputValue serviceInputValue) => this.Items.Add(serviceInputValue);

  internal Exception InternalParseValues(string data, Dictionary<string, string> variables = null)
  {
    Exception values = (Exception) null;
    if (data.Contains("(") && data.Contains(")"))
    {
      int startIndex = data.IndexOf('(') + 1;
      int length = data.IndexOf(')') - startIndex;
      if (length > 0)
      {
        string[] source = data.Substring(startIndex, length).Split(",".ToCharArray(), StringSplitOptions.None);
        if (((IEnumerable<string>) source).All<string>((Func<string, bool>) (i => i.Contains("="))))
        {
          for (int index = 0; index < source.Length && values == null; ++index)
          {
            string[] strArray = source[index].Split("=".ToCharArray(), StringSplitOptions.None);
            ServiceInputValue serviceInputValue = this[strArray[0]];
            if (serviceInputValue != null)
              values = serviceInputValue.InternalSetValue(strArray[1], variables);
          }
        }
        else if (source.Length <= this.Count)
        {
          for (int index = 0; index < source.Length && values == null; ++index)
          {
            ServiceInputValue serviceInputValue = this[index];
            if (!string.IsNullOrEmpty(source[index]))
              values = serviceInputValue.InternalSetValue(source[index], variables);
          }
        }
        else
          values = (Exception) new InvalidOperationException("Too many input arguments provided");
      }
    }
    return values;
  }

  public ServiceInputValue this[string qualifier]
  {
    get
    {
      ServiceInputValue serviceInputValue = this.FirstOrDefault<ServiceInputValue>((Func<ServiceInputValue, bool>) (item => item.Qualifier.CompareNoCase(qualifier) || item.ParameterQualifier.CompareNoCase(qualifier)));
      string alternateQualifier;
      if (serviceInputValue == null && this.service.Channel.Ecu.AlternateQualifiers.TryGetValue(qualifier, out alternateQualifier))
        serviceInputValue = this.FirstOrDefault<ServiceInputValue>((Func<ServiceInputValue, bool>) (item => item.Qualifier.CompareNoCase(alternateQualifier) || item.ParameterQualifier.CompareNoCase(alternateQualifier)));
      return serviceInputValue;
    }
  }

  public void ParseValues(string inputValues)
  {
    Exception values = this.InternalParseValues(inputValues);
    if (values != null)
      throw values;
  }
}
