using System;
using System.Collections.Generic;
using System.Linq;
using Softing.Dts;

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

	public string QualifierPath { get; private set; }

	public string Qualifier { get; private set; }

	public string Name { get; private set; }

	public McdDBDataObjectProp DataObjectProp
	{
		get
		{
			if (!dataObjectPropRetrieveAttempted && dataObjectProp == null && !IsMatchingRequestParameter && !IsConst && !IsReserved)
			{
				try
				{
					dataObjectPropRetrieveAttempted = true;
					dataObjectProp = new McdDBDataObjectProp(((DtsDbResponseParameter)responseParameter).DbDataObjectProp);
				}
				catch (DtsDatabaseException)
				{
				}
			}
			return dataObjectProp;
		}
	}

	public string PresentationName => DataObjectProp?.Name;

	public string PresentationQualifier => DataObjectProp?.Qualifier;

	public string Unit
	{
		get
		{
			if (unit == null && DataType != null)
			{
				unit = responseParameter.Unit;
			}
			return unit;
		}
	}

	public int? DecimalPlaces
	{
		get
		{
			if (!decimalPlaces.HasValue && (DataType == typeof(float) || DataType == typeof(double)))
			{
				decimalPlaces = responseParameter.DecimalPlaces;
			}
			return decimalPlaces;
		}
	}

	public bool IsConst => mcdParameterType == MCDParameterType.eCODED_CONST;

	public bool IsReserved => mcdParameterType == MCDParameterType.eRESERVED;

	public bool IsMatchingRequestParameter => mcdParameterType == MCDParameterType.eMATCHING_REQUEST_PARAM;

	public bool IsStructure => parameterType == MCDDataType.eSTRUCTURE;

	public bool IsNoType => parameterType == MCDDataType.eNO_TYPE;

	public bool IsMultiplexer => parameterType == MCDDataType.eMULTIPLEXER;

	public bool IsDiagnosticTroubleCode => responseParameter.DataType == MCDDataType.eDTC;

	public bool IsEnvironmentalData => parameterType == MCDDataType.eENVDATA;

	public bool IsArray => parameterType == MCDDataType.eEND_OF_PDU;

	public McdDBResponseParameter ArrayDefinition => (parent == null) ? null : (parent.IsArray ? parent : parent.ArrayDefinition);

	public string Description
	{
		get
		{
			if (description == null)
			{
				description = responseParameter.Description;
			}
			return description;
		}
	}

	public long BytePos
	{
		get
		{
			if (!bytePos.HasValue)
			{
				try
				{
					if (parent != null)
					{
						bytePos = parent.BytePos;
						bytePos += OdxBytePos;
					}
					else
					{
						bytePos = OdxBytePos;
					}
				}
				catch (McdException)
				{
					if (!bytePos.HasValue)
					{
						if (!IsEnvironmentalData)
						{
							throw;
						}
						bytePos = 0L;
					}
				}
			}
			return bytePos.Value;
		}
	}

	public long OdxBytePos
	{
		get
		{
			if (!odxBytePos.HasValue)
			{
				try
				{
					odxBytePos = responseParameter.ODXBytePos;
				}
				catch (MCDException ex)
				{
					throw new McdException(ex, "BytePos");
				}
			}
			return odxBytePos.Value;
		}
	}

	public byte BitPos
	{
		get
		{
			if (!bitPos.HasValue)
			{
				bitPos = responseParameter.BitPos;
			}
			return bitPos.Value;
		}
	}

	public long ByteLength
	{
		get
		{
			if (!byteLength.HasValue)
			{
				byteLength = responseParameter.ByteLength;
			}
			return byteLength.Value;
		}
	}

	public long BitLength
	{
		get
		{
			if (!bitLength.HasValue)
			{
				bitLength = responseParameter.BitLength;
			}
			return bitLength.Value;
		}
	}

	public Type DataType
	{
		get
		{
			if (type == null)
			{
				type = McdRoot.MapDataType(responseParameter.DataType);
			}
			return type;
		}
	}

	public Type CodedParameterType
	{
		get
		{
			if (codedParameterType == null)
			{
				codedParameterType = McdRoot.MapDataType(((DtsDbResponseParameter)responseParameter).CodedParameterType);
			}
			return codedParameterType;
		}
	}

	public IEnumerable<McdTextTableElement> TextTableElements
	{
		get
		{
			if (textTableElements == null && DataType == typeof(McdTextTableElement))
			{
				textTableElements = (from tt in responseParameter.TextTableElements.OfType<MCDTextTableElement>()
					select new McdTextTableElement(tt)).ToList();
			}
			return textTableElements;
		}
	}

	public IEnumerable<McdDBResponseParameter> Parameters
	{
		get
		{
			if (parameters == null)
			{
				parameters = (from p in responseParameter.DbParameters.OfType<MCDDbResponseParameter>()
					select new McdDBResponseParameter(p, this)).ToList();
			}
			return parameters;
		}
	}

	public IEnumerable<McdDBResponseParameter> AllParameters => McdRoot.FlattenStructures(Parameters, (McdDBResponseParameter p) => p.Parameters);

	public McdDBResponseParameter Parent => parent;

	IMcdDataItem IMcdDataItem.Parent => Parent;

	IEnumerable<IMcdDataItem> IMcdDataItem.Parameters => Parameters;

	internal McdDBResponseParameter(MCDDbResponseParameter responseParameter, McdDBResponseParameter parent = null)
	{
		this.responseParameter = responseParameter;
		mcdParameterType = this.responseParameter.MCDParameterType;
		parameterType = this.responseParameter.ParameterType;
		Qualifier = this.responseParameter.ShortName;
		Name = this.responseParameter.LongName;
		this.parent = parent;
		QualifierPath = ((this.parent != null) ? (this.parent.QualifierPath + "_" + Qualifier) : Qualifier);
	}
}
