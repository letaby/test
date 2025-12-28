using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

internal class VarcodeMcd : Varcode
{
	private Channel channel;

	private McdLogicalLink mcdVarcoding;

	private bool mcdVarcodingOwner;

	private List<McdDiagComPrimitive> pendingPrimitives = new List<McdDiagComPrimitive>();

	internal VarcodeMcd(Channel channel, string interfaceQualifier, McdInterface mcdInterface)
	{
		this.channel = channel;
		mcdVarcoding = McdRoot.CreateOfflineLogicalLink(interfaceQualifier, this.channel.DiagnosisVariant.Name, mcdInterface);
		mcdVarcodingOwner = true;
	}

	internal VarcodeMcd(Channel channel, McdLogicalLink mcdLogicalLink)
	{
		this.channel = channel;
		mcdVarcoding = mcdLogicalLink;
		mcdVarcodingOwner = false;
	}

	private static bool IsSplittedParameterGroup(McdDiagComPrimitive diagComPrimitive)
	{
		return IsSplittedParameterGroup(diagComPrimitive.SpecialData);
	}

	internal static bool IsSplittedParameterGroup(Dictionary<string, string> sdgs)
	{
		if (sdgs != null && sdgs.TryGetValue(".Splitted_Parameter_Group", out var value))
		{
			return value.Equals("yes", StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	private static CaesarException GetSplittedParameterGroupResult(McdDiagComPrimitive job)
	{
		return Convert.ToUInt32(job.AllPositiveResponseParameters.FirstOrDefault((McdResponseParameter r) => r.Qualifier == "Status").Value.CodedValue, CultureInfo.InvariantCulture) switch
		{
			0u => null, 
			1u => new CaesarException(job.AllPositiveResponseParameters.FirstOrDefault((McdResponseParameter r) => r.Qualifier == "ErrorMessage").Value.Value as string), 
			2u => new CaesarException(job.AllPositiveResponseParameters.FirstOrDefault((McdResponseParameter r) => r.Qualifier == "NRC").Value), 
			_ => null, 
		};
	}

	internal override void DoCoding()
	{
		base.Exception = null;
		foreach (McdDiagComPrimitive pendingPrimitive in pendingPrimitives)
		{
			try
			{
				if (IsSplittedParameterGroup(pendingPrimitive))
				{
					McdDiagComPrimitive service = mcdVarcoding.GetService("DiagJob_" + pendingPrimitive.Qualifier);
					if (service != null)
					{
						if (service.SetInput("Data", pendingPrimitive.RequestMessage.Skip(pendingPrimitive.PduActualDataStartPos).ToArray()))
						{
							service.Execute(0);
							CaesarException splittedParameterGroupResult = GetSplittedParameterGroupResult(service);
							if (splittedParameterGroupResult != null)
							{
								base.Exception = splittedParameterGroupResult;
							}
						}
					}
					else
					{
						base.Exception = new CaesarException(SapiError.SplitGroupDiagjobNotFound);
					}
				}
				else
				{
					pendingPrimitive.Execute(0);
					if (pendingPrimitive.IsNegativeResponse)
					{
						base.Exception = new CaesarException(pendingPrimitive);
					}
				}
			}
			catch (McdException mcdError)
			{
				base.Exception = new CaesarException(mcdError);
			}
		}
		pendingPrimitives.Clear();
	}

	internal override bool AllowSetDefaultString(string groupQualifier)
	{
		return true;
	}

	internal override void EnableReadCodingStringFromEcu(bool enableReadCodingStringFromEcu)
	{
	}

	internal override byte[] GetCurrentCodingString(string groupQualifier)
	{
		base.Exception = null;
		Parameter itemFirstInGroup = channel.Parameters.GetItemFirstInGroup(groupQualifier);
		McdDiagComPrimitive service = mcdVarcoding.GetService(itemFirstInGroup.WriteService.McdQualifier);
		if (mcdVarcoding.State == mcdVarcoding.TargetState)
		{
			try
			{
				McdDiagComPrimitive service2 = mcdVarcoding.GetService(itemFirstInGroup.ReadService.McdQualifier);
				if (IsSplittedParameterGroup(service2))
				{
					McdDiagComPrimitive service3 = mcdVarcoding.GetService("DiagJob_" + service2.Qualifier);
					if (service3 != null)
					{
						service3.Execute(0);
						CaesarException splittedParameterGroupResult = GetSplittedParameterGroupResult(service3);
						if (splittedParameterGroupResult != null)
						{
							base.Exception = splittedParameterGroupResult;
						}
						else
						{
							byte[] second = service3.AllPositiveResponseParameters.FirstOrDefault((McdResponseParameter p) => p.Qualifier == "Data").Value.Value as byte[];
							service.RequestMessage = itemFirstInGroup.WritePrefix.Concat(second);
						}
					}
					else
					{
						base.Exception = new CaesarException(SapiError.SplitGroupDiagjobNotFound);
					}
				}
				else
				{
					service2.Execute(0);
					if (service2.IsNegativeResponse)
					{
						base.Exception = new CaesarException(service2);
					}
					else
					{
						byte[] second2 = service2.ResponseMessage.Skip(itemFirstInGroup.WritePrefix.Length).ToArray();
						service.RequestMessage = itemFirstInGroup.WritePrefix.Concat(second2);
					}
				}
			}
			catch (McdException mcdError)
			{
				base.Exception = new CaesarException(mcdError);
			}
		}
		if (base.Exception != null)
		{
			return new byte[0];
		}
		return service.RequestMessage.Skip(itemFirstInGroup.WritePrefix.Length).ToArray();
	}

	internal override void SetCurrentCodingString(string groupQualifier, byte[] content)
	{
		base.Exception = null;
		Parameter itemFirstInGroup = channel.Parameters.GetItemFirstInGroup(groupQualifier);
		McdDiagComPrimitive service = mcdVarcoding.GetService(itemFirstInGroup.WriteService.McdQualifier);
		service.RequestMessage = itemFirstInGroup.WritePrefix.Concat(content);
		AddPendingPrimitive(service);
	}

	internal override void SetDefaultStringByPartNumber(string partNumber)
	{
		base.Exception = null;
		try
		{
			McdConfigurationRecord mcdConfigurationRecord = mcdVarcoding.SetDefaultStringByPartNumber(partNumber);
			if (mcdConfigurationRecord == null)
			{
				base.Exception = new CaesarException(SapiError.DefaultStringKeyNotFound, partNumber);
				return;
			}
			McdDiagComPrimitive mcdDiagComPrimitive = mcdVarcoding.UpdateDiagCodingString(mcdConfigurationRecord);
			if (mcdDiagComPrimitive != null)
			{
				AddPendingPrimitive(mcdDiagComPrimitive);
			}
			else
			{
				base.Exception = new CaesarException(SapiError.DiagComPrimitiveReferenceFromCodingFileNotFound, mcdConfigurationRecord.DBRecord.Qualifier);
			}
		}
		catch (McdException mcdError)
		{
			base.Exception = new CaesarException(mcdError);
		}
	}

	internal override void SetDefaultStringByPartNumberAndPartVersion(string partNumber, uint partVersion)
	{
		SetDefaultStringByPartNumber(string.Format(CultureInfo.InvariantCulture, "{0}_{1,2:000}", partNumber, partVersion));
	}

	internal override void SetFragmentMeaningByPartNumber(string partNumber)
	{
		base.Exception = null;
		try
		{
			McdConfigurationRecord mcdConfigurationRecord = mcdVarcoding.SetFragmentMeaningByPartNumber(partNumber);
			if (mcdConfigurationRecord == null)
			{
				base.Exception = new CaesarException(SapiError.FragmentKeyNotFound, partNumber);
				return;
			}
			McdDiagComPrimitive mcdDiagComPrimitive = mcdVarcoding.UpdateDiagCodingString(mcdConfigurationRecord);
			if (mcdDiagComPrimitive != null)
			{
				AddPendingPrimitive(mcdDiagComPrimitive);
			}
			else
			{
				base.Exception = new CaesarException(SapiError.DiagComPrimitiveReferenceFromCodingFileNotFound, mcdConfigurationRecord.DBRecord.Qualifier);
			}
		}
		catch (McdException mcdError)
		{
			base.Exception = new CaesarException(mcdError);
		}
	}

	internal override void SetFragmentMeaningByPartNumberAndPartVersion(string partNumber, uint partVersion)
	{
		SetFragmentMeaningByPartNumber(string.Format(CultureInfo.InvariantCulture, "{0}_{1,2:000}", partNumber, partVersion));
	}

	internal override void SetFragmentValue(Parameter parameter, object value)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		base.Exception = null;
		ParamType paramType = parameter.ParamType;
		if ((int)paramType != 15)
		{
			if ((int)paramType == 18)
			{
				value = ((Choice)value).OriginalName;
			}
		}
		else
		{
			value = ((Dump)value).Data.ToArray();
		}
		McdDiagComPrimitive service = mcdVarcoding.GetService(parameter.WriteService.McdQualifier);
		try
		{
			if (!service.SetInput(parameter.McdQualifier, value))
			{
				base.Exception = new CaesarException(SapiError.FragmentNotFound);
			}
		}
		catch (McdException mcdError)
		{
			base.Exception = new CaesarException(mcdError);
		}
		catch (FormatException innerexception)
		{
			base.Exception = new CaesarException("Exception setting fragment value", innerexception);
		}
		if (base.Exception == null)
		{
			AddPendingPrimitive(service);
		}
	}

	internal void AddPendingPrimitive(McdDiagComPrimitive diagComPrimitive)
	{
		if (mcdVarcoding.State == mcdVarcoding.TargetState && !pendingPrimitives.Contains(diagComPrimitive))
		{
			pendingPrimitives.Add(diagComPrimitive);
		}
	}

	internal override object GetFragmentValue(Parameter parameter)
	{
		base.Exception = null;
		McdDiagComPrimitive service = mcdVarcoding.GetService(parameter.WriteService.McdQualifier);
		try
		{
			McdValue input = service.GetInput(parameter.McdQualifier);
			if (input != null)
			{
				if (input.IsValueValid)
				{
					return input.GetValue(parameter.Type, parameter.Choices);
				}
				if (parameter.Choices == null || parameter.Choices.Count == 0)
				{
					ScaleEntry scaleEntry = ((parameter.Scales != null && parameter.Scales.Any()) ? parameter.Scales.First() : parameter.FactorOffsetScale);
					if (scaleEntry != null)
					{
						return Convert.ChangeType(scaleEntry.ToPhysicalValue(Convert.ToDecimal(input.CodedValue, CultureInfo.InvariantCulture)), parameter.Type, CultureInfo.InvariantCulture);
					}
					base.Exception = new CaesarException(SapiError.NoMatchingIntervalWasFound);
				}
				else
				{
					base.Exception = new CaesarException(SapiError.NoMatchingChoiceValueInVarcodingString);
				}
			}
		}
		catch (McdException mcdError)
		{
			base.Exception = new CaesarException(mcdError);
		}
		return null;
	}

	internal bool IsOwner(McdLogicalLink link)
	{
		if (mcdVarcodingOwner)
		{
			return mcdVarcoding == link;
		}
		return false;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposedValue)
		{
			return;
		}
		if (disposing && mcdVarcoding != null)
		{
			if (mcdVarcodingOwner)
			{
				mcdVarcoding.Dispose();
			}
			mcdVarcoding = null;
		}
		disposedValue = true;
	}
}
