// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBDataObjectProp
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace McdAbstraction;

public class McdDBDataObjectProp
{
  private DtsDbDataObjectProp dataObjectProp;
  private string qualifier;
  private string name;
  private Type codedParameterType;
  private bool? isHighLowByteOrder;
  private List<McdDBCompuScale> scaleEntries;

  internal McdDBDataObjectProp(DtsDbDataObjectProp dataObjectProp)
  {
    this.dataObjectProp = dataObjectProp;
    this.qualifier = this.dataObjectProp.ShortName;
  }

  public string Qualifier => this.qualifier;

  public string Name
  {
    get
    {
      if (this.name == null)
        this.name = this.dataObjectProp.LongName;
      return this.name;
    }
  }

  public Type CodedType
  {
    get
    {
      if (this.codedParameterType == (Type) null)
        this.codedParameterType = McdRoot.MapDataType(this.dataObjectProp.CodedType);
      return this.codedParameterType;
    }
  }

  public bool IsHighLowByteOrder
  {
    get
    {
      if (!this.isHighLowByteOrder.HasValue)
        this.isHighLowByteOrder = new bool?(this.dataObjectProp.IsHighlowByteOrder);
      return this.isHighLowByteOrder.Value;
    }
  }

  public IEnumerable<McdDBCompuScale> ScaleEntries
  {
    get
    {
      if (this.scaleEntries == null)
      {
        this.scaleEntries = new List<McdDBCompuScale>();
        if (this.dataObjectProp.IsCompuMethodValid)
        {
          DtsDbCompuMethod compuMethod = this.dataObjectProp.CompuMethod;
          if (compuMethod != null && compuMethod.IsCompuInternalToPhysValid)
          {
            string categoryName = compuMethod.CategoryName;
            if (categoryName == "Linear" || categoryName == "ScaleLinear")
              this.scaleEntries.AddRange(compuMethod.CompuInternalToPhys.DbCompuScales.OfType<DtsDbCompuScale>().Select<DtsDbCompuScale, McdDBCompuScale>((Func<DtsDbCompuScale, McdDBCompuScale>) (cs => new McdDBCompuScale(cs))));
          }
        }
      }
      return (IEnumerable<McdDBCompuScale>) this.scaleEntries;
    }
  }
}
