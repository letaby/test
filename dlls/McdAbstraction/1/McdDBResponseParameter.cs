// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBResponseParameter
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace McdAbstraction;

public class McdDBResponseParameter : IMcdDataItem
{
  private MCDDbResponseParameter responseParameter;
  private MCDParameterType mcdParameterType;
  private MCDDataType parameterType;
  private McdDBResponseParameter parent;
  private bool dataObjectPropRetrieveAttempted = false;
  private McdDBDataObjectProp dataObjectProp;
  private string unit;
  private int? decimalPlaces;
  private string description;
  private long? bytePos;
  private long? odxBytePos;
  private byte? bitPos;
  private long? byteLength;
  private long? bitLength;
  private Type type;
  private Type codedParameterType;
  private IEnumerable<McdTextTableElement> textTableElements;
  private IEnumerable<McdDBResponseParameter> parameters;

  internal McdDBResponseParameter(
    MCDDbResponseParameter responseParameter,
    McdDBResponseParameter parent = null)
  {
    this.responseParameter = responseParameter;
    this.mcdParameterType = this.responseParameter.MCDParameterType;
    this.parameterType = this.responseParameter.ParameterType;
    this.Qualifier = this.responseParameter.ShortName;
    this.Name = this.responseParameter.LongName;
    this.parent = parent;
    this.QualifierPath = this.parent != null ? $"{this.parent.QualifierPath}_{this.Qualifier}" : this.Qualifier;
  }

  public string QualifierPath { get; private set; }

  public string Qualifier { get; private set; }

  public string Name { get; private set; }

  public McdDBDataObjectProp DataObjectProp
  {
    get
    {
      if (!this.dataObjectPropRetrieveAttempted && this.dataObjectProp == null && !this.IsMatchingRequestParameter && !this.IsConst && !this.IsReserved)
      {
        try
        {
          this.dataObjectPropRetrieveAttempted = true;
          this.dataObjectProp = new McdDBDataObjectProp(((DtsDbParameter) this.responseParameter).DbDataObjectProp);
        }
        catch (DtsDatabaseException ex)
        {
        }
      }
      return this.dataObjectProp;
    }
  }

  public string PresentationName => this.DataObjectProp?.Name;

  public string PresentationQualifier => this.DataObjectProp?.Qualifier;

  public string Unit
  {
    get
    {
      if (this.unit == null && this.DataType != (Type) null)
        this.unit = this.responseParameter.Unit;
      return this.unit;
    }
  }

  public int? DecimalPlaces
  {
    get
    {
      if (!this.decimalPlaces.HasValue && (this.DataType == typeof (float) || this.DataType == typeof (double)))
        this.decimalPlaces = new int?((int) this.responseParameter.DecimalPlaces);
      return this.decimalPlaces;
    }
  }

  public bool IsConst => this.mcdParameterType == MCDParameterType.eCODED_CONST;

  public bool IsReserved => this.mcdParameterType == MCDParameterType.eRESERVED;

  public bool IsMatchingRequestParameter
  {
    get => this.mcdParameterType == MCDParameterType.eMATCHING_REQUEST_PARAM;
  }

  public bool IsStructure => this.parameterType == MCDDataType.eSTRUCTURE;

  public bool IsNoType => this.parameterType == MCDDataType.eNO_TYPE;

  public bool IsMultiplexer => this.parameterType == MCDDataType.eMULTIPLEXER;

  public bool IsDiagnosticTroubleCode => this.responseParameter.DataType == MCDDataType.eDTC;

  public bool IsEnvironmentalData => this.parameterType == MCDDataType.eENVDATA;

  public bool IsArray => this.parameterType == MCDDataType.eEND_OF_PDU;

  public McdDBResponseParameter ArrayDefinition
  {
    get
    {
      return this.parent != null ? (this.parent.IsArray ? this.parent : this.parent.ArrayDefinition) : (McdDBResponseParameter) null;
    }
  }

  public string Description
  {
    get
    {
      if (this.description == null)
        this.description = this.responseParameter.Description;
      return this.description;
    }
  }

  public long BytePos
  {
    get
    {
      if (!this.bytePos.HasValue)
      {
        try
        {
          if (this.parent != null)
          {
            this.bytePos = new long?(this.parent.BytePos);
            long? bytePos = this.bytePos;
            long odxBytePos = this.OdxBytePos;
            this.bytePos = bytePos.HasValue ? new long?(bytePos.GetValueOrDefault() + odxBytePos) : new long?();
          }
          else
            this.bytePos = new long?(this.OdxBytePos);
        }
        catch (McdException ex)
        {
          if (!this.bytePos.HasValue)
          {
            if (this.IsEnvironmentalData)
              this.bytePos = new long?(0L);
            else
              throw;
          }
        }
      }
      return this.bytePos.Value;
    }
  }

  public long OdxBytePos
  {
    get
    {
      if (!this.odxBytePos.HasValue)
      {
        try
        {
          this.odxBytePos = new long?((long) this.responseParameter.ODXBytePos);
        }
        catch (MCDException ex)
        {
          throw new McdException(ex, "BytePos");
        }
      }
      return this.odxBytePos.Value;
    }
  }

  public byte BitPos
  {
    get
    {
      if (!this.bitPos.HasValue)
        this.bitPos = new byte?(this.responseParameter.BitPos);
      return this.bitPos.Value;
    }
  }

  public long ByteLength
  {
    get
    {
      if (!this.byteLength.HasValue)
        this.byteLength = new long?((long) this.responseParameter.ByteLength);
      return this.byteLength.Value;
    }
  }

  public long BitLength
  {
    get
    {
      if (!this.bitLength.HasValue)
        this.bitLength = new long?((long) this.responseParameter.BitLength);
      return this.bitLength.Value;
    }
  }

  public Type DataType
  {
    get
    {
      if (this.type == (Type) null)
        this.type = McdRoot.MapDataType(this.responseParameter.DataType);
      return this.type;
    }
  }

  public Type CodedParameterType
  {
    get
    {
      if (this.codedParameterType == (Type) null)
        this.codedParameterType = McdRoot.MapDataType(((DtsDbParameter) this.responseParameter).CodedParameterType);
      return this.codedParameterType;
    }
  }

  public IEnumerable<McdTextTableElement> TextTableElements
  {
    get
    {
      if (this.textTableElements == null && this.DataType == typeof (McdTextTableElement))
        this.textTableElements = (IEnumerable<McdTextTableElement>) this.responseParameter.TextTableElements.OfType<MCDTextTableElement>().Select<MCDTextTableElement, McdTextTableElement>((Func<MCDTextTableElement, McdTextTableElement>) (tt => new McdTextTableElement(tt))).ToList<McdTextTableElement>();
      return this.textTableElements;
    }
  }

  public IEnumerable<McdDBResponseParameter> Parameters
  {
    get
    {
      if (this.parameters == null)
        this.parameters = (IEnumerable<McdDBResponseParameter>) this.responseParameter.DbParameters.OfType<MCDDbResponseParameter>().Select<MCDDbResponseParameter, McdDBResponseParameter>((Func<MCDDbResponseParameter, McdDBResponseParameter>) (p => new McdDBResponseParameter(p, this))).ToList<McdDBResponseParameter>();
      return this.parameters;
    }
  }

  public IEnumerable<McdDBResponseParameter> AllParameters
  {
    get
    {
      return McdRoot.FlattenStructures<McdDBResponseParameter>(this.Parameters, (Func<McdDBResponseParameter, IEnumerable<McdDBResponseParameter>>) (p => p.Parameters));
    }
  }

  public McdDBResponseParameter Parent => this.parent;

  IMcdDataItem IMcdDataItem.Parent => (IMcdDataItem) this.Parent;

  IEnumerable<IMcdDataItem> IMcdDataItem.Parameters => (IEnumerable<IMcdDataItem>) this.Parameters;
}
