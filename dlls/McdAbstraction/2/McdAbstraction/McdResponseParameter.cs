using System;
using System.Collections.Generic;
using System.Linq;
using Softing.Dts;

namespace McdAbstraction;

public class McdResponseParameter : IDisposable, IMcdDataItem
{
	private MCDResponseParameter responseParameter;

	private McdResponseParameter parent;

	private List<McdResponseParameter> parameters;

	private bool disposedValue = false;

	public string QualifierPath { get; private set; }

	public string Qualifier { get; private set; }

	public string Name { get; private set; }

	public bool IsDiagnosticTroubleCode => responseParameter.DataType == MCDDataType.eDTC;

	public bool IsEnvironmentalData => responseParameter.DataType == MCDDataType.eENVDATA;

	public McdDBDiagTroubleCode DBDiagTroubleCode
	{
		get
		{
			try
			{
				return IsDiagnosticTroubleCode ? new McdDBDiagTroubleCode(responseParameter.DbDTC) : null;
			}
			catch (DtsSystemException)
			{
				return null;
			}
		}
	}

	public McdValue Value
	{
		get
		{
			MCDValue mCDValue = ((responseParameter.DataType != MCDDataType.eA_BYTEFIELD && responseParameter.DataType != MCDDataType.eA_ASCIISTRING && responseParameter.DataType != MCDDataType.eA_UNICODE2STRING) ? responseParameter.CodedValue : null);
			try
			{
				switch (responseParameter.ValueRangeInfo)
				{
				case MCDRangeInfo.eVALUE_VALID:
					return new McdValue(responseParameter.Value, mCDValue);
				case MCDRangeInfo.eVALUE_NOT_DEFINED:
					return new McdValue(mCDValue);
				case MCDRangeInfo.eVALUE_NOT_AVAILABLE:
				case MCDRangeInfo.eVALUE_NOT_VALID:
					try
					{
						return new McdValue(responseParameter.InternalScaleConstraint, mCDValue);
					}
					catch (DtsParameterizationException)
					{
						return new McdValue((MCDValue)null, mCDValue);
					}
					catch (DtsDatabaseException)
					{
						return new McdValue((MCDValue)null, mCDValue);
					}
				default:
					return new McdValue((MCDValue)null, mCDValue);
				}
			}
			catch (DtsDatabaseException ex3)
			{
				if (ex3.Error.VendorCode == 57661)
				{
					return null;
				}
				throw;
			}
		}
	}

	public bool IsValueValid => responseParameter.ValueRangeInfo == MCDRangeInfo.eVALUE_VALID;

	public IEnumerable<McdResponseParameter> Parameters
	{
		get
		{
			if (parameters == null)
			{
				parameters = (from p in responseParameter.Parameters.OfType<MCDResponseParameter>()
					select new McdResponseParameter(p, this)).ToList();
			}
			return parameters;
		}
	}

	public IEnumerable<McdResponseParameter> AllParameters => McdRoot.FlattenStructures(Parameters, (McdResponseParameter p) => p.Parameters);

	public McdResponseParameter Parent => parent;

	IEnumerable<IMcdDataItem> IMcdDataItem.Parameters => Parameters;

	IMcdDataItem IMcdDataItem.Parent => Parent;

	internal McdResponseParameter(MCDResponseParameter responseParameter, McdResponseParameter parent = null)
	{
		this.responseParameter = responseParameter;
		Qualifier = this.responseParameter.ShortName;
		Name = this.responseParameter.LongName;
		this.parent = parent;
		QualifierPath = ((this.parent != null) ? (this.parent.QualifierPath + "_" + Qualifier) : Qualifier);
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
				foreach (McdResponseParameter parameter in parameters)
				{
					parameter.Dispose();
				}
				parameters.Clear();
				parameters = null;
			}
			if (responseParameter != null)
			{
				responseParameter.Dispose();
				responseParameter = null;
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
