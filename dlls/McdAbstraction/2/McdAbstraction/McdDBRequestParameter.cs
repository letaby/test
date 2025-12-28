using System;
using System.Collections.Generic;
using System.Linq;
using Softing.Dts;

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

	public bool IsConst => mcdParameterType == MCDParameterType.eCODED_CONST;

	public bool IsReserved => mcdParameterType == MCDParameterType.eRESERVED;

	public bool IsStructure => parameterType == MCDDataType.eSTRUCTURE;

	public string Qualifier { get; private set; }

	public McdDBDataObjectProp DataObjectProp
	{
		get
		{
			if (!dataObjectPropRetrieveAttempted && dataObjectProp == null && !IsConst && !IsReserved)
			{
				try
				{
					dataObjectPropRetrieveAttempted = true;
					dataObjectProp = new McdDBDataObjectProp(((DtsDbRequestParameter)requestParameter).DbDataObjectProp);
				}
				catch (DtsDatabaseException)
				{
				}
			}
			return dataObjectProp;
		}
	}

	public string PreparationName => DataObjectProp?.Name;

	public string PreparationQualifier => DataObjectProp?.Qualifier;

	public string Name { get; private set; }

	public string Description
	{
		get
		{
			if (description == null)
			{
				description = requestParameter.Description;
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
				bytePos = requestParameter.BytePos;
			}
			return bytePos.Value;
		}
	}

	public byte BitPos
	{
		get
		{
			if (!bitPos.HasValue)
			{
				bitPos = requestParameter.BitPos;
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
				byteLength = requestParameter.ByteLength;
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
				bitLength = requestParameter.BitLength;
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
				type = McdRoot.MapDataType(requestParameter.DataType);
			}
			return type;
		}
	}

	public string Unit
	{
		get
		{
			if (unit == null && DataType != null)
			{
				unit = requestParameter.Unit;
			}
			return unit;
		}
	}

	public Type CodedParameterType
	{
		get
		{
			if (codedParameterType == null)
			{
				codedParameterType = McdRoot.MapDataType(((DtsDbRequestParameter)requestParameter).CodedParameterType);
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
				textTableElements = (from tt in requestParameter.TextTableElements.OfType<MCDTextTableElement>()
					select new McdTextTableElement(tt)).ToList();
			}
			return textTableElements;
		}
	}

	public IEnumerable<McdDBRequestParameter> Parameters
	{
		get
		{
			if (parameters == null)
			{
				parameters = (from p in requestParameter.DbParameters.OfType<MCDDbRequestParameter>()
					select new McdDBRequestParameter(p, this)).ToList();
			}
			return parameters;
		}
	}

	public McdDBRequestParameter Parent => parent;

	public Dictionary<string, string> SpecialData
	{
		get
		{
			if (specialData == null)
			{
				specialData = McdDBDiagComPrimitive.GetSpecialData(requestParameter.DbSDGs);
			}
			return specialData;
		}
	}

	internal McdDBRequestParameter(MCDDbRequestParameter requestParameter, McdDBRequestParameter parent = null)
	{
		this.requestParameter = requestParameter;
		mcdParameterType = this.requestParameter.MCDParameterType;
		parameterType = this.requestParameter.ParameterType;
		Qualifier = this.requestParameter.ShortName;
		Name = this.requestParameter.LongName;
		this.parent = parent;
	}

	public McdValue GetDefaultValue()
	{
		if (defaultValue == null)
		{
			if (!hasDefaultValue.HasValue)
			{
				hasDefaultValue = ((DtsDbRequestParameter)requestParameter).HasDefaultValue;
			}
			if (hasDefaultValue.Value)
			{
				try
				{
					defaultValue = new McdValue(requestParameter.DefaultValue);
				}
				catch (DtsDatabaseException ex)
				{
					throw new McdException(ex.Error, "DefaultValue");
				}
			}
		}
		return defaultValue;
	}
}
