// Decompiled with JetBrains decompiler
// Type: SapiLayer1.EcuInterfaceCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class EcuInterfaceCollection : ReadOnlyCollection<EcuInterface>
{
  private Ecu ecu;

  internal EcuInterfaceCollection(Ecu parent)
    : base((IList<EcuInterface>) new List<EcuInterface>())
  {
    this.ecu = parent;
  }

  internal void AcquireList()
  {
    if (this.ecu.IsMcd)
    {
      IEnumerable<McdDBLogicalLink> logicalLinksForEcu = McdRoot.GetDBLogicalLinksForEcu(this.ecu.Name);
      int num = 0;
      foreach (McdDBLogicalLink logicalLinkInfo in (IEnumerable<McdDBLogicalLink>) logicalLinksForEcu.OrderByDescending<McdDBLogicalLink, int>((Func<McdDBLogicalLink, int>) (ll => McdRoot.LocationPriority.IndexOf(ll.ProtocolLocation.Qualifier))))
      {
        EcuInterface ecuInterface = new EcuInterface(this.ecu, num++);
        ecuInterface.Acquire(logicalLinkInfo);
        this.Items.Add(ecuInterface);
      }
    }
    else
    {
      uint interfaceTypeCount = CaesarRoot.GetAvailableInterfaceTypeCount(this.ecu.Name);
      for (uint index = 0; index < interfaceTypeCount; ++index)
      {
        try
        {
          using (CaesarEcuInterface interfaceByIndex = CaesarRoot.GetInterfaceByIndex(this.ecu.Name, index))
          {
            if (interfaceByIndex != null)
            {
              EcuInterface ecuInterface = new EcuInterface(this.ecu, (int) index);
              ecuInterface.Acquire(interfaceByIndex);
              this.Items.Add(ecuInterface);
            }
          }
        }
        catch (CaesarErrorException ex)
        {
          Sapi.GetSapi().RaiseExceptionEvent((object) this.ecu, (Exception) new CaesarException(ex));
        }
      }
    }
  }

  public EcuInterface this[string qualifier]
  {
    get
    {
      return this.FirstOrDefault<EcuInterface>((Func<EcuInterface, bool>) (item => string.Equals(item.Qualifier, qualifier, StringComparison.Ordinal)));
    }
  }
}
