using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class CodingChoice
{
	private string rawValueTranslationQualifier;

	private Dictionary<Parameter, object> relatedParameterValues;

	private int index;

	private string rawValue;

	private string meaning;

	private string description;

	private Part part;

	private ushort accessLevel;

	private CodingParameter parameter;

	private CodingParameterGroup parameterGroup;

	private DiagnosisSource diagnosisSource;

	private StringDictionary cPFData;

	public string RawValue
	{
		get
		{
			if (parameter != null)
			{
				if (rawValueTranslationQualifier == null)
				{
					string text = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", parameter.ParameterGroup.Name, parameter.Name);
					rawValueTranslationQualifier = Sapi.MakeTranslationIdentifier(text, rawValue.CreateQualifierFromName(), "Name");
				}
				if (parameter.ParameterGroup.DiagnosisVariants.Count > 0)
				{
					return parameter.ParameterGroup.DiagnosisVariants[0].Ecu.Translate(rawValueTranslationQualifier, rawValue);
				}
				return rawValue;
			}
			return rawValue;
		}
	}

	public string OriginalRawValue => rawValue;

	private string CombinedQualifier
	{
		get
		{
			List<string> list = new List<string>();
			list.Add(parameterGroup.Qualifier);
			if (parameter != null)
			{
				list.Add(parameter.Name.CreateQualifierFromName());
			}
			list.Add(meaning.CreateQualifierFromName());
			return Sapi.MakeTranslationIdentifier(list.ToArray());
		}
	}

	public int Index => index;

	public string Meaning
	{
		get
		{
			if (ParameterGroup.DiagnosisVariants.Count > 0)
			{
				return parameterGroup.DiagnosisVariants[0].Ecu.Translate(Sapi.MakeTranslationIdentifier(CombinedQualifier, "Meaning"), meaning);
			}
			return meaning;
		}
	}

	public string Description
	{
		get
		{
			if (parameterGroup.DiagnosisVariants.Count > 0)
			{
				return parameterGroup.DiagnosisVariants[0].Ecu.Translate(Sapi.MakeTranslationIdentifier(CombinedQualifier, "Description"), description);
			}
			return description;
		}
	}

	public Part Part => part;

	public int AccessLevel => accessLevel;

	public CodingParameter Parameter => parameter;

	public CodingParameterGroup ParameterGroup => parameterGroup;

	public bool IsValidForChannel
	{
		get
		{
			if (parameterGroup.Channel == null)
			{
				return false;
			}
			if (parameter != null)
			{
				if (diagnosisSource == DiagnosisSource.CaesarDatabase)
				{
					if (parameter.RelatedParameter == null || (parameter.RelatedParameter.Type == typeof(Choice) && !parameter.RelatedParameter.Choices.Any((Choice c) => c.OriginalName == OriginalRawValue)))
					{
						return false;
					}
				}
				else if (diagnosisSource == DiagnosisSource.McdDatabase && (!parameterGroup.ChannelByteLength.HasValue || parameter.BytePos >= parameterGroup.ChannelByteLength))
				{
					return false;
				}
			}
			else if (!parameterGroup.ChannelByteLength.HasValue || ContentLength.Value != parameterGroup.ChannelByteLength.Value)
			{
				return false;
			}
			return true;
		}
	}

	public IDictionary<Parameter, object> RelatedParameterValues
	{
		get
		{
			if (relatedParameterValues == null && parameter == null && IsValidForChannel)
			{
				lock (parameterGroup.Channel.OfflineVarcodingHandleLock)
				{
					Varcode varcode = parameterGroup.Channel.OfflineVarcodingHandle;
					if (varcode != null)
					{
						byte[] currentCodingString = varcode.GetCurrentCodingString(parameterGroup.Qualifier);
						if (varcode.Exception == null)
						{
							varcode.SetCurrentCodingString(parameterGroup.Qualifier, new Dump(rawValue).Data.ToArray());
							if (varcode.Exception == null)
							{
								relatedParameterValues = parameterGroup.Channel.ParameterGroups[parameterGroup.Qualifier].Parameters.ToDictionary((Parameter k) => k, (Parameter v) => v.GetPresentation(varcode));
							}
							varcode.SetCurrentCodingString(parameterGroup.Qualifier, currentCodingString);
						}
					}
				}
			}
			return relatedParameterValues;
		}
	}

	public int? ContentLength
	{
		get
		{
			if (parameter != null)
			{
				return null;
			}
			return RawValue.Length / 2;
		}
	}

	internal CodingChoice(CodingParameter parent)
	{
		parameter = parent;
		parameterGroup = parent.ParameterGroup;
	}

	internal CodingChoice(CodingParameterGroup parent)
	{
		parameterGroup = parent;
	}

	internal CodingChoice()
	{
	}

	internal void Acquire(CaesarDICcfFragValue fragmentValue, int index)
	{
		meaning = fragmentValue.Meaning;
		description = fragmentValue.Description;
		rawValue = fragmentValue.Value;
		string partNumber = fragmentValue.PartNumber;
		if (partNumber != null)
		{
			object partVersion = fragmentValue.PartVersion;
			if (partVersion != null)
			{
				part = new Part(partNumber, partVersion);
			}
			else
			{
				part = new Part(partNumber);
			}
		}
		accessLevel = fragmentValue.AccessLevel;
		diagnosisSource = DiagnosisSource.CaesarDatabase;
		this.index = index;
	}

	internal void Acquire(McdDBItemValue fragmentValue, int index)
	{
		meaning = fragmentValue.Meaning;
		description = fragmentValue.Description;
		if (fragmentValue.Value != null)
		{
			rawValue = fragmentValue.Value.Value.ToString();
		}
		part = new Part(fragmentValue.PartNumber);
		diagnosisSource = DiagnosisSource.McdDatabase;
		this.index = index;
	}

	internal void Acquire(CaesarDICcfDefaultString defaultStringValue)
	{
		meaning = defaultStringValue.Meaning;
		description = defaultStringValue.Description;
		string partNumber = defaultStringValue.PartNumber;
		if (partNumber != null)
		{
			object partVersion = defaultStringValue.PartVersion;
			if (partVersion != null)
			{
				part = new Part(partNumber, partVersion);
			}
			else
			{
				part = new Part(partNumber);
			}
		}
		accessLevel = defaultStringValue.AccessLevel;
		Dump dump = new Dump(defaultStringValue.Value);
		rawValue = dump.ToString();
		diagnosisSource = DiagnosisSource.CaesarDatabase;
	}

	internal void Acquire(McdDBDataRecord defaultStringValue)
	{
		meaning = defaultStringValue.Name;
		description = defaultStringValue.Description;
		part = new Part(defaultStringValue.PartNumber);
		Dump dump = new Dump(defaultStringValue.BinaryData);
		rawValue = dump.ToString();
		diagnosisSource = DiagnosisSource.McdDatabase;
	}

	internal void Acquire(StreamReader reader, string meaning, Part partNumber)
	{
		part = partNumber;
		this.meaning = meaning;
		accessLevel = 1;
		reader.BaseStream.Seek(0L, SeekOrigin.Begin);
		reader.DiscardBufferedData();
		rawValue = reader.ReadToEnd();
		cPFData = new StringDictionary();
		ParameterCollection.LoadDictionaryFromStream(reader, ParameterFileFormat.ParFile, cPFData);
	}

	internal CodingChoice Clone(CodingParameter newParameter, CodingParameterGroup newParameterGroup)
	{
		return new CodingChoice
		{
			rawValue = rawValue,
			meaning = meaning,
			description = description,
			part = part,
			accessLevel = accessLevel,
			cPFData = cPFData,
			index = index,
			parameter = newParameter,
			parameterGroup = newParameterGroup
		};
	}

	internal void AddStringsForTranslation(Dictionary<string, string> table)
	{
		table[Sapi.MakeTranslationIdentifier(CombinedQualifier, "Meaning")] = meaning;
		if (description != null)
		{
			table[Sapi.MakeTranslationIdentifier(CombinedQualifier, "Description")] = description;
		}
	}

	public void SetAsValue()
	{
		CaesarException ex = null;
		lock (parameterGroup.Channel.OfflineVarcodingHandleLock)
		{
			Varcode offlineVarcodingHandle = parameterGroup.Channel.OfflineVarcodingHandle;
			if (offlineVarcodingHandle != null)
			{
				int count = parameterGroup.Channel.Parameters.Count;
				if (cPFData != null)
				{
					foreach (DictionaryEntry cPFDatum in cPFData)
					{
						Parameter parameter = parameterGroup.Channel.Parameters[cPFDatum.Key.ToString()];
						if (parameter != null)
						{
							parameter.InternalSetValue(cPFDatum.Value.ToString(), respectAccessLevel: false);
							continue;
						}
						Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Missing parameter during SetAsValue: {0}", cPFDatum.Key));
						ex = new CaesarException(SapiError.ParameterSpecifiedDidNotExist);
					}
				}
				else
				{
					SetAsValue(offlineVarcodingHandle);
					if (offlineVarcodingHandle.IsErrorSet)
					{
						ex = offlineVarcodingHandle.Exception;
					}
					else
					{
						for (int i = 0; i < count; i++)
						{
							Parameter parameter2 = parameterGroup.Channel.Parameters[i];
							if (string.Equals(parameterGroup.Qualifier, parameter2.GroupQualifier, StringComparison.Ordinal))
							{
								if (this.parameter == null)
								{
									parameter2.InternalRead(offlineVarcodingHandle, fromDevice: false);
								}
								else if (string.Equals(this.parameter.Name, parameter2.OriginalName, StringComparison.Ordinal))
								{
									parameter2.InternalRead(offlineVarcodingHandle, fromDevice: false);
									break;
								}
							}
						}
						if (this.parameter == null)
						{
							parameterGroup.Channel.Parameters.UpdateCodingString(parameterGroup.Qualifier, offlineVarcodingHandle, CodingStringSource.FromDefaultString);
						}
						else
						{
							parameterGroup.Channel.Parameters.ResetGroupCodingString(parameterGroup.Qualifier);
						}
					}
				}
			}
		}
		if (ex != null)
		{
			throw ex;
		}
	}

	internal void SetAsValue(Varcode varcode)
	{
		if (parameter == null)
		{
			if (part.Version != null)
			{
				varcode.SetDefaultStringByPartNumberAndPartVersion(part.Number, (uint)part.Version);
			}
			else
			{
				varcode.SetDefaultStringByPartNumber(part.Number);
			}
		}
		else if (varcode is VarcodeCaesar varcodeCaesar)
		{
			varcodeCaesar.SetFragmentMeaningByIndex(parameter.RelatedParameter, index);
		}
		else if (part.Version != null)
		{
			varcode.SetFragmentMeaningByPartNumberAndPartVersion(part.Number, (uint)part.Version);
		}
		else
		{
			varcode.SetFragmentMeaningByPartNumber(part.Number);
		}
	}

	public void Mark(bool choice)
	{
		if (cPFData != null)
		{
			foreach (DictionaryEntry cPFDatum in cPFData)
			{
				Parameter parameter = parameterGroup.Channel.Parameters[cPFDatum.Key.ToString()];
				if (parameter != null)
				{
					parameter.Marked = choice;
				}
			}
			return;
		}
		for (int i = 0; i < parameterGroup.Channel.Parameters.Count; i++)
		{
			Parameter parameter2 = parameterGroup.Channel.Parameters[i];
			if (string.Equals(parameterGroup.Qualifier, parameter2.GroupQualifier, StringComparison.Ordinal))
			{
				if (this.parameter == null)
				{
					parameter2.Marked = choice;
				}
				else if (string.Equals(this.parameter.Name, parameter2.OriginalName, StringComparison.Ordinal))
				{
					parameter2.Marked = choice;
					break;
				}
			}
		}
	}
}
