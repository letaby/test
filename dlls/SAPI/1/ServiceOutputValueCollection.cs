// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ServiceOutputValueCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace SapiLayer1;

public sealed class ServiceOutputValueCollection : ReadOnlyCollection<ServiceOutputValue>
{
  internal ServiceOutputValueCollection()
    : base((IList<ServiceOutputValue>) new List<ServiceOutputValue>())
  {
  }

  internal void Add(ServiceOutputValue serviceOutputValue)
  {
    if (serviceOutputValue.Service.CombinedService != (Service) null && serviceOutputValue.BytePosition.HasValue && serviceOutputValue.BitPosition.HasValue)
    {
      int num1 = serviceOutputValue.BytePosition.Value * 8 + serviceOutputValue.BitPosition.Value;
      foreach (ServiceOutputValue serviceOutputValue1 in (IEnumerable<ServiceOutputValue>) this.Items)
      {
        int num2 = serviceOutputValue1.BytePosition.Value * 8 + serviceOutputValue1.BitPosition.Value;
        if (num1 < num2)
        {
          this.Items.Insert(this.Items.IndexOf(serviceOutputValue1), serviceOutputValue);
          return;
        }
      }
    }
    this.Items.Add(serviceOutputValue);
  }

  public ServiceOutputValue this[string qualifier]
  {
    get
    {
      foreach (ServiceOutputValue serviceOutputValue in (ReadOnlyCollection<ServiceOutputValue>) this)
      {
        if (string.Equals(serviceOutputValue.Name, qualifier, StringComparison.Ordinal) || serviceOutputValue.ParameterQualifier != null && string.Equals(serviceOutputValue.ParameterQualifier, qualifier, StringComparison.Ordinal))
          return serviceOutputValue;
      }
      return (ServiceOutputValue) null;
    }
  }
}
