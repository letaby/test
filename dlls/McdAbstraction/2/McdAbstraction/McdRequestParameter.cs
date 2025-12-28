using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Softing.Dts;

namespace McdAbstraction;

public class McdRequestParameter : IDisposable
{
	private MCDRequestParameter requestParameter;

	private McdRequestParameter parent;

	private List<McdRequestParameter> parameters;

	private bool disposedValue = false;

	public string Qualifier { get; private set; }

	public string Name { get; private set; }

	internal McdValue Value
	{
		get
		{
			try
			{
				MCDValue mcdValue = null;
				if (requestParameter.ValueRangeInfo == MCDRangeInfo.eVALUE_VALID)
				{
					try
					{
						mcdValue = requestParameter.Value;
					}
					catch (MCDException)
					{
					}
				}
				return new McdValue(mcdValue, requestParameter.CodedValue);
			}
			catch (MCDException ex2)
			{
				throw new McdException(ex2, "McdRequestParameter.Value");
			}
		}
	}

	public IEnumerable<McdRequestParameter> Parameters
	{
		get
		{
			if (parameters == null)
			{
				parameters = (from p in requestParameter.Parameters.OfType<MCDRequestParameter>()
					select new McdRequestParameter(p, this)).ToList();
			}
			return parameters;
		}
	}

	public McdRequestParameter Parent => parent;

	internal McdRequestParameter(MCDRequestParameter requestParameter, McdRequestParameter parent = null)
	{
		this.requestParameter = requestParameter;
		Qualifier = this.requestParameter.ShortName;
		Name = this.requestParameter.LongName;
		this.parent = parent;
	}

	internal void SetValue(object newValue)
	{
		try
		{
			MCDValue mCDValue = requestParameter.CreateValue();
			switch (mCDValue.DataType)
			{
			case MCDDataType.eA_FLOAT32:
				mCDValue.Float32 = (float)Convert.ChangeType(newValue, typeof(float), CultureInfo.InvariantCulture);
				break;
			case MCDDataType.eA_FLOAT64:
				mCDValue.Float64 = (double)Convert.ChangeType(newValue, typeof(double), CultureInfo.InvariantCulture);
				break;
			case MCDDataType.eA_INT16:
				mCDValue.Int16 = (short)Convert.ChangeType(newValue, typeof(short), CultureInfo.InvariantCulture);
				break;
			case MCDDataType.eA_INT32:
				mCDValue.Int32 = (int)Convert.ChangeType(newValue, typeof(int), CultureInfo.InvariantCulture);
				break;
			case MCDDataType.eA_INT64:
				mCDValue.Int64 = (long)Convert.ChangeType(newValue, typeof(long), CultureInfo.InvariantCulture);
				break;
			case MCDDataType.eA_INT8:
				mCDValue.Int8 = (char)Convert.ChangeType(newValue, typeof(char), CultureInfo.InvariantCulture);
				break;
			case MCDDataType.eA_UINT16:
				mCDValue.Uint16 = (ushort)Convert.ChangeType(newValue, typeof(ushort), CultureInfo.InvariantCulture);
				break;
			case MCDDataType.eA_UINT32:
				mCDValue.Uint32 = (uint)Convert.ChangeType(newValue, typeof(uint), CultureInfo.InvariantCulture);
				break;
			case MCDDataType.eA_UINT64:
				mCDValue.Uint64 = (ulong)Convert.ChangeType(newValue, typeof(ulong), CultureInfo.InvariantCulture);
				break;
			case MCDDataType.eA_UINT8:
				mCDValue.Uint8 = (byte)Convert.ChangeType(newValue, typeof(byte), CultureInfo.InvariantCulture);
				break;
			case MCDDataType.eA_BYTEFIELD:
				if (newValue is byte[] bytefield)
				{
					mCDValue.Bytefield = bytefield;
				}
				else
				{
					mCDValue.ValueAsString = newValue.ToString();
				}
				break;
			default:
				mCDValue.ValueAsString = newValue.ToString();
				break;
			}
			requestParameter.Value = mCDValue;
		}
		catch (MCDException ex)
		{
			throw new McdException(ex, "SetInput");
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposedValue)
		{
			return;
		}
		if (disposing)
		{
			if (parameters != null)
			{
				foreach (McdRequestParameter parameter in parameters)
				{
					parameter.Dispose();
				}
				parameters.Clear();
				parameters = null;
			}
			if (requestParameter != null)
			{
				requestParameter.Dispose();
				requestParameter = null;
			}
		}
		disposedValue = true;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
