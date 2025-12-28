using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class CodingParameterGroup : ICodingItem
{
	private string mcdQualifier;

	private int? channelByteLength;

	private Dictionary<string, IEnumerable<CodingChoicesForCoding>> options = new Dictionary<string, IEnumerable<CodingChoicesForCoding>>();

	private Dump defaultStringMask;

	private string qualifier;

	private string name;

	private string description;

	private int? byteLength;

	private CodingChoiceCollection choices;

	private CodingParameterCollection parameters;

	private IList<DiagnosisVariant> diagnosisVariants;

	private CodingParameterGroupCollection parameterGroups;

	private Service readComService;

	public string Qualifier => qualifier;

	public string McdQualifier => mcdQualifier;

	public string Name => name ?? qualifier;

	public string Description => description;

	public int? ByteLength => byteLength;

	public int? ChannelByteLength
	{
		get
		{
			if (!channelByteLength.HasValue)
			{
				channelByteLength = Channel?.Parameters.GetItemFirstInGroup(Qualifier)?.GroupLength;
			}
			return channelByteLength;
		}
	}

	public CodingChoiceCollection Choices => choices;

	public CodingParameterCollection Parameters => parameters;

	public CodingParameterGroupCollection ParameterGroups => parameterGroups;

	public Channel Channel => ParameterGroups.Channel;

	public IList<DiagnosisVariant> DiagnosisVariants => diagnosisVariants;

	public Service ReadService => readComService;

	public Dump DefaultStringMask
	{
		get
		{
			if (defaultStringMask == null && ByteLength.HasValue)
			{
				defaultStringMask = CodingParameter.CreateCodingStringMask(ChannelByteLength.HasValue ? ChannelByteLength.Value : ByteLength.Value, Parameters, includeExclude: false);
			}
			return defaultStringMask;
		}
	}

	internal CodingParameterGroup(CodingParameterGroupCollection parent)
	{
		parameterGroups = parent;
		choices = new CodingChoiceCollection();
		parameters = new CodingParameterCollection();
	}

	internal void Acquire(CaesarDIVcd varcode)
	{
		qualifier = varcode.Qualifier;
		description = varcode.Description;
		choices.AcquireList(this, varcode);
		parameters.AcquireList(this, varcode);
		List<DiagnosisVariant> list = new List<DiagnosisVariant>();
		Sapi sapi = Sapi.GetSapi();
		uint allowedEcuCount = varcode.AllowedEcuCount;
		for (uint num = 0u; num < allowedEcuCount; num++)
		{
			string ecuName = varcode.GetAllowedEcuByIndex(num);
			Ecu ecu = sapi.Ecus.FirstOrDefault((Ecu e) => e.Name == ecuName && e.DiagnosisSource == DiagnosisSource.CaesarDatabase);
			if (ecu == null)
			{
				continue;
			}
			uint allowedEcuVariantCount = varcode.GetAllowedEcuVariantCount(num);
			if (allowedEcuVariantCount != 0)
			{
				for (uint num2 = 0u; num2 < allowedEcuVariantCount; num2++)
				{
					string allowedEcuVariantByIndex = varcode.GetAllowedEcuVariantByIndex(num, num2);
					DiagnosisVariant diagnosisVariant = ecu.DiagnosisVariants[allowedEcuVariantByIndex];
					if (diagnosisVariant != null)
					{
						list.Add(diagnosisVariant);
					}
				}
				continue;
			}
			foreach (DiagnosisVariant diagnosisVariant2 in ecu.DiagnosisVariants)
			{
				list.Add(diagnosisVariant2);
			}
		}
		diagnosisVariants = list.AsReadOnly();
	}

	internal void Acquire(McdDBConfigurationRecord domain, IEnumerable<DiagnosisVariant> variants)
	{
		qualifier = McdCaesarEquivalence.GetDomainQualifier(domain.Name);
		mcdQualifier = domain.Qualifier;
		name = domain.Name;
		description = domain.Description;
		byteLength = domain.ByteLength;
		diagnosisVariants = variants.ToList().AsReadOnly();
		choices.AcquireList(this, domain);
		parameters.AcquireList(this, domain);
	}

	internal void Acquire(StreamReader reader, string meaning, Ecu ecu, Part partNumber)
	{
		qualifier = "*";
		List<DiagnosisVariant> list = new List<DiagnosisVariant>();
		string identificationRecordValue = ParameterCollection.GetIdentificationRecordValue("DIAGNOSISVARIANT", reader);
		if (string.IsNullOrEmpty(identificationRecordValue))
		{
			foreach (DiagnosisVariant diagnosisVariant2 in ecu.DiagnosisVariants)
			{
				list.Add(diagnosisVariant2);
			}
		}
		else
		{
			DiagnosisVariant diagnosisVariant = ecu.DiagnosisVariants[identificationRecordValue];
			if (diagnosisVariant != null)
			{
				list.Add(diagnosisVariant);
			}
			else
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "A Target diagnostic variant specified in a CPF file does not exist: {0}", identificationRecordValue));
			}
		}
		diagnosisVariants = list.AsReadOnly();
		CodingChoice codingChoice = new CodingChoice(this);
		codingChoice.Acquire(reader, meaning, partNumber);
		choices.Add(codingChoice);
	}

	internal CodingParameterGroup Clone(CodingParameterGroupCollection newParent)
	{
		CodingParameterGroup codingParameterGroup = new CodingParameterGroup(newParent);
		codingParameterGroup.qualifier = qualifier;
		codingParameterGroup.mcdQualifier = mcdQualifier;
		codingParameterGroup.byteLength = byteLength;
		codingParameterGroup.name = name;
		codingParameterGroup.description = description;
		codingParameterGroup.diagnosisVariants = new List<DiagnosisVariant>(diagnosisVariants).AsReadOnly();
		codingParameterGroup.parameters = new CodingParameterCollection();
		codingParameterGroup.choices = new CodingChoiceCollection();
		CopyTo(codingParameterGroup);
		return codingParameterGroup;
	}

	internal void CopyTo(CodingParameterGroup destination)
	{
		List<Parameter> list = new List<Parameter>();
		if (destination.Channel != null)
		{
			list.AddRange(destination.Channel.Parameters.Where((Parameter p) => p.GroupQualifier == Qualifier));
			destination.readComService = (list.Any() ? list.First().ReadService : null);
		}
		foreach (CodingParameter parameter in parameters)
		{
			destination.parameters.Add(parameter.Clone(destination, list.FirstOrDefault((Parameter p) => p.OriginalName == parameter.Name || (parameter.Qualifier != null && (p.Qualifier == parameter.Qualifier || p.McdQualifier == parameter.Qualifier)))));
		}
		foreach (CodingChoice choice in choices)
		{
			destination.choices.Add(choice.Clone(null, destination));
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("The GetDiagnosisVariants method is deprecated, please use the DiagnosisVariants property instead.")]
	public DiagnosisVariant[] GetDiagnosisVariants()
	{
		return diagnosisVariants.ToArray();
	}

	public bool VariantAllowed(DiagnosisVariant variant)
	{
		foreach (DiagnosisVariant diagnosisVariant in diagnosisVariants)
		{
			if (diagnosisVariant == variant)
			{
				return true;
			}
		}
		return false;
	}

	private IEnumerable<CodingChoice> GetApplicableFragments(VarcodeCaesar varcode, IEnumerable<CodingChoice> allFragmentValues, byte[] setData)
	{
		List<CodingChoice> list = new List<CodingChoice>();
		foreach (CodingChoice item in allFragmentValues.Where((CodingChoice fv) => fv.IsValidForChannel))
		{
			varcode.SetCurrentCodingString(qualifier, setData);
			if (varcode.IsErrorSet)
			{
				CaesarException exception = varcode.Exception;
				Sapi.GetSapi().RaiseDebugInfoEvent(this, item.Parameter.Channel.Ecu.Name + "." + item.Parameter.ParameterGroup.Qualifier + ": " + exception.Message + " while attempting to set initial coding string'" + BitConverter.ToString(setData).Replace("-", "") + "'");
				break;
			}
			item.SetAsValue(varcode);
			if (varcode.IsErrorSet)
			{
				CaesarException exception2 = varcode.Exception;
				Sapi.GetSapi().RaiseDebugInfoEvent(this, item.Parameter.Channel.Ecu.Name + "." + item.Parameter.ParameterGroup.Qualifier + "." + item.Parameter.Name + ": " + exception2.Message + " while attempting to apply fragment value '" + item.RawValue + "'");
			}
			else if (setData.SequenceEqual(varcode.GetCurrentCodingString(qualifier)))
			{
				list.Add(item);
			}
		}
		return list;
	}

	private IEnumerable<CodingChoice> GetApplicableFragments(VarcodeMcd varcode, IEnumerable<CodingChoice> allFragmentValues, byte[] setData)
	{
		List<CodingChoice> list = new List<CodingChoice>();
		CodingChoice codingChoice = Choices.FirstOrDefault((CodingChoice c) => c.ContentLength == setData.Length);
		if (codingChoice != null)
		{
			codingChoice.SetAsValue(varcode);
			if (varcode.IsErrorSet)
			{
				CaesarException exception = varcode.Exception;
				Sapi.GetSapi().RaiseDebugInfoEvent(this, Channel.Ecu.Name + "." + Qualifier + ": " + exception.Message + " while attempting to apply dummy default string value '" + codingChoice.RawValue + "'");
				return list;
			}
		}
		foreach (IGrouping<CodingParameter, CodingChoice> item in from c in allFragmentValues
			where c.Parameter.BytePos < setData.Length
			group c by c.Parameter)
		{
			foreach (CodingChoice item2 in item)
			{
				item2.SetAsValue(varcode);
				if (varcode.IsErrorSet)
				{
					CaesarException exception2 = varcode.Exception;
					Sapi.GetSapi().RaiseDebugInfoEvent(this, item2.Parameter.Channel.Ecu.Name + "." + item2.Parameter.ParameterGroup.Qualifier + "." + item2.Parameter.Name + ": " + exception2.Message + " while attempting to apply fragment value '" + item2.RawValue + "'");
					continue;
				}
				byte[] currentCodingString = varcode.GetCurrentCodingString(qualifier);
				if (varcode.IsErrorSet)
				{
					CaesarException exception3 = varcode.Exception;
					Sapi.GetSapi().RaiseDebugInfoEvent(this, item2.Parameter.Channel.Ecu.Name + "." + item2.Parameter.ParameterGroup.Qualifier + "." + item2.Parameter.Name + ": " + exception3.Message + " while attempting to read coding string after applying fragment value '" + item2.RawValue + "'");
				}
				else if (currentCodingString.Length != setData.Length)
				{
					Sapi.GetSapi().RaiseDebugInfoEvent(this, item2.Parameter.Channel.Ecu.Name + "." + item2.Parameter.ParameterGroup.Qualifier + "." + item2.Parameter.Name + ": got mismatched coding string length " + currentCodingString.Length + " instead of set data length " + setData.Length + " after applying fragment value '" + item2.RawValue + "'");
				}
				else if (item.Key.AreMaskedCodingStringsEquivalent(currentCodingString, setData))
				{
					list.Add(item2);
					break;
				}
			}
		}
		return list;
	}

	private IEnumerable<CodingChoice> GetApplicableFragments(Varcode varcode, IEnumerable<CodingChoice> allFragmentValues, byte[] setData)
	{
		if (!(varcode is VarcodeCaesar varcode2))
		{
			return GetApplicableFragments((VarcodeMcd)varcode, allFragmentValues, setData);
		}
		return GetApplicableFragments(varcode2, allFragmentValues, setData);
	}

	public IEnumerable<CodingChoicesForCoding> GetChoicesAndCodingForCoding(string coding)
	{
		lock (Channel.OfflineVarcodingHandleLock)
		{
			if (!options.ContainsKey(coding))
			{
				if (Parameters.Count == 0)
				{
					options[coding] = from d in Choices
						where d.RawValue != null && d.RawValue.Length == coding.Length
						select new CodingChoicesForCoding(new List<CodingChoice> { d }.AsEnumerable(), new Dump(d.RawValue));
				}
				else
				{
					Varcode offlineVarcodingHandle = Channel.OfflineVarcodingHandle;
					if (offlineVarcodingHandle == null)
					{
						throw new CaesarException(SapiError.OfflineVarcodingNotAvailable);
					}
					AcquireDefaultStringandFragmentChoicesForCoding(coding, offlineVarcodingHandle);
				}
			}
			return options[coding];
		}
	}

	public IEnumerable<IEnumerable<CodingChoice>> GetChoicesForCoding(string coding)
	{
		return from set in GetChoicesAndCodingForCoding(coding)
			where set.Coding == null || set.Coding.ToString() == coding
			select set.CodingChoices;
	}

	internal void AcquireDefaultStringandFragmentChoicesForCoding(string coding)
	{
		lock (Channel.OfflineVarcodingHandleLock)
		{
			Varcode offlineVarcodingHandle = Channel.OfflineVarcodingHandle;
			if (offlineVarcodingHandle != null && !options.ContainsKey(coding) && Parameters.Count > 0)
			{
				AcquireDefaultStringandFragmentChoicesForCoding(coding, offlineVarcodingHandle);
			}
		}
	}

	private void AcquireDefaultStringandFragmentChoicesForCoding(string coding, Varcode varcode)
	{
		byte[] setData = new Dump(coding).Data.ToArray();
		IEnumerable<CodingChoice> allFragmentValues = (from parameter in Parameters
			from fragmentValue in parameter.Choices
			where fragmentValue.Part != null
			select fragmentValue).ToList();
		List<CodingChoicesForCoding> list = new List<CodingChoicesForCoding>();
		IEnumerable<CodingChoice> applicableFragments = GetApplicableFragments(varcode, allFragmentValues, setData);
		foreach (CodingChoice item in Choices.Where((CodingChoice c) => c.RawValue.Length == coding.Length))
		{
			byte[] setData2 = new Dump(item.RawValue).Data.ToArray();
			IEnumerable<CodingChoice> applicableFragments2 = GetApplicableFragments(varcode, applicableFragments, setData2);
			IEnumerable<CodingChoice> enumerable = applicableFragments.Except(applicableFragments2);
			varcode.AllowSetDefaultString(qualifier);
			item.SetAsValue(varcode);
			if (varcode.IsErrorSet)
			{
				CaesarException exception = varcode.Exception;
				Sapi.GetSapi().RaiseDebugInfoEvent(this, exception.Message + " while attempting to apply default string value " + item.RawValue + " for " + qualifier);
				continue;
			}
			foreach (CodingChoice item2 in enumerable)
			{
				item2.SetAsValue(varcode);
				if (varcode.IsErrorSet)
				{
					CaesarException exception2 = varcode.Exception;
					Sapi.GetSapi().RaiseDebugInfoEvent(this, exception2.Message + " while attempting to re-apply fragment value " + item2.RawValue + " for " + item2.Parameter.Name);
				}
			}
			byte[] currentCodingString = varcode.GetCurrentCodingString(qualifier);
			list.Add(new CodingChoicesForCoding(new List<CodingChoice> { item }.Union(enumerable), new Dump(currentCodingString)));
		}
		if (list.Count((CodingChoicesForCoding res) => res.Coding.ToString() == coding) == 0)
		{
			list.Add(new CodingChoicesForCoding(applicableFragments, null));
		}
		options[coding] = list.OrderBy((CodingChoicesForCoding set) => set.CodingChoices.Count());
		if (!varcode.AllowSetDefaultString(qualifier) && varcode.IsErrorSet)
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Error after VCAllowSetDefaultString({0}): {1}", qualifier, varcode.Exception.Message));
		}
	}
}
