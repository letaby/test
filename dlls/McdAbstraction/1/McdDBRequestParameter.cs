// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBRequestParameter
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace McdAbstraction;

public class McdDBRequestParameter
{
  private MCDDbRequestParameter requestParameter;
  private McdDBRequestParameter parent;
  private MCDParameterType mcdParameterType;
  private MCDDataType parameterType;
  private bool? hasDefaultValue;
  private McdValue defaultValue;
  private bool dataObjectPropRetrieveAttempted = false;
  private McdDBDataObjectProp dataObjectProp;
  private string description;
  private long? bytePos;
  private byte? bitPos;
  private long? byteLength;
  private long? bitLength;
  private Type type;
  private string unit;
  private Type codedParameterType;
  private IEnumerable<McdTextTableElement> textTableElements;
  private IEnumerable<McdDBRequestParameter> parameters;
  private Dictionary<string, string> specialData;

  internal McdDBRequestParameter(
    MCDDbRequestParameter requestParameter,
    McdDBRequestParameter parent = null)
  {
    this.requestParameter = requestParameter;
    this.mcdParameterType = this.requestParameter.MCDParameterType;
    this.parameterType = this.requestParameter.ParameterType;
    this.Qualifier = this.requestParameter.ShortName;
    this.Name = this.requestParameter.LongName;
    this.parent = parent;
  }

  public McdValue GetDefaultValue()
  {
    if (this.defaultValue == null)
    {
      if (!this.hasDefaultValue.HasValue)
        this.hasDefaultValue = new bool?(((DtsDbParameter) this.requestParameter).HasDefaultValue);
      if (this.hasDefaultValue.Value)
      {
        try
        {
          this.defaultValue = new McdValue(this.requestParameter.DefaultValue);
        }
        catch (DtsDatabaseException ex)
        {
          throw new McdException(ex.Error, "DefaultValue");
        }
      }
    }
    return this.defaultValue;
  }

  public bool IsConst => this.mcdParameterType == MCDParameterType.eCODED_CONST;

  public bool IsReserved => this.mcdParameterType == MCDParameterType.eRESERVED;

  public bool IsStructure => this.parameterType == MCDDataType.eSTRUCTURE;

  public string Qualifier { get; private set; }

  public McdDBDataObjectProp DataObjectProp
  {
    get
    {
      if (!this.dataObjectPropRetrieveAttempted && this.dataObjectProp == null && !this.IsConst && !this.IsReserved)
      {
        try
        {
          this.dataObjectPropRetrieveAttempted = true;
          this.dataObjectProp = new McdDBDataObjectProp(((DtsDbParameter) this.requestParameter).DbDataObjectProp);
        }
        catch (DtsDatabaseException ex)
        {
        }
      }
      return this.dataObjectProp;
    }
  }

  public string PreparationName => this.DataObjectProp?.Name;

  public string PreparationQualifier => this.DataObjectProp?.Qualifier;

  public string Name { get; private set; }

  public string Description
  {
    get
    {
      if (this.description == null)
        this.description = this.requestParameter.Description;
      return this.description;
    }
  }

  public long BytePos
  {
    get
    {
      if (!this.bytePos.HasValue)
        this.bytePos = new long?((long) this.requestParameter.BytePos);
      return this.bytePos.Value;
    }
  }

  public byte BitPos
  {
    get
    {
      if (!this.bitPos.HasValue)
        this.bitPos = new byte?(this.requestParameter.BitPos);
      return this.bitPos.Value;
    }
  }

  public long ByteLength
  {
    get
    {
      if (!this.byteLength.HasValue)
        this.byteLength = new long?((long) this.requestParameter.ByteLength);
      return this.byteLength.Value;
    }
  }

  public long BitLength
  {
    get
    {
      if (!this.bitLength.HasValue)
        this.bitLength = new long?((long) this.requestParameter.BitLength);
      return this.bitLength.Value;
    }
  }

  public Type DataType
  {
    get
    {
      if (this.type == (Type) null)
        this.type = McdRoot.MapDataType(this.requestParameter.DataType);
      return this.type;
    }
  }

  public string Unit
  {
    get
    {
      if (this.unit == null && this.DataType != (Type) null)
        this.unit = this.requestParameter.Unit;
      return this.unit;
    }
  }

  public Type CodedParameterType
  {
    get
    {
      if (this.codedParameterType == (Type) null)
        this.codedParameterType = McdRoot.MapDataType(((DtsDbParameter) this.requestParameter).CodedParameterType);
      return this.codedParameterType;
    }
  }

  public IEnumerable<McdTextTableElement> TextTableElements
  {
    get
    {
      if (this.textTableElements == null && this.DataType == typeof (McdTextTableElement))
        this.textTableElements = (IEnumerable<McdTextTableElement>) this.requestParameter.TextTableElements.OfType<MCDTextTableElement>().Select<MCDTextTableElement, McdTextTableElement>((Func<MCDTextTableElement, McdTextTableElement>) (tt => new McdTextTableElement(tt))).ToList<McdTextTableElement>();
      return this.textTableElements;
    }
  }

  public IEnumerable<McdDBRequestParameter> Parameters
  {
    get
    {
      if (this.parameters == null)
        this.parameters = (IEnumerable<McdDBRequestParameter>) this.requestParameter.DbParameters.OfType<MCDDbRequestParameter>().Select<MCDDbRequestParameter, McdDBRequestParameter>((Func<MCDDbRequestParameter, McdDBRequestParameter>) (p => new McdDBRequestParameter(p, this))).ToList<McdDBRequestParameter>();
      return this.parameters;
    }
  }

  public McdDBRequestParameter Parent => this.parent;

  public Dictionary<string, string> SpecialData
  {
    get
    {
      if (this.specialData == null)
        this.specialData = McdDBDiagComPrimitive.GetSpecialData(this.requestParameter.DbSDGs);
      return this.specialData;
    }
  }
}
