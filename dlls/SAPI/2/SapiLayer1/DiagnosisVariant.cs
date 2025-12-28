using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using CaesarAbstraction;
using McdAbstraction;
using Microsoft.VisualBasic.FileIO;

namespace SapiLayer1;

public sealed class DiagnosisVariant
{
	private Ecu ecu;

	private string name;

	private string description;

	private bool isBase;

	private IEnumerable<Tuple<string, string>> parameterQualifiers;

	private Part partNumber;

	private IEnumerable<CaesarIdBlock> idBlocks;

	private IEnumerable<McdDBMatchingPattern> mcdIdBlocks;

	private readonly IEnumerable<Tuple<RollCall.ID, string>> rollCallIdBlock;

	private IEnumerable<string> diagServiceQualifiers;

	private IEnumerable<string> flashJobQualifiers;

	private IDictionary<string, string> variantAttributes;

	private List<Tuple<string, string>> controlPrimitiveNames;

	public IEnumerable<string> Locations { get; internal set; }

	internal IEnumerable<Tuple<string, string>> ParameterQualifiers
	{
		get
		{
			if (parameterQualifiers == null)
			{
				if (!ecu.IsRollCall)
				{
					if (ecu.IsMcd)
					{
						parameterQualifiers = GetQualifierList(GetMcdDBLocationForProtocol(ecu.ProtocolName));
					}
					else
					{
						CaesarEcu val = OpenEcuHandle();
						try
						{
							parameterQualifiers = GetQualifierList(val);
						}
						finally
						{
							((IDisposable)val)?.Dispose();
						}
					}
				}
				else
				{
					parameterQualifiers = (from p in GetQualifierList(ecu.GetDefParser())
						select Tuple.Create(string.Empty, p)).ToList();
				}
			}
			return parameterQualifiers;
		}
	}

	public Ecu Ecu => ecu;

	public string Name => name;

	public string Description => description;

	public string FixedEquipmentType { get; private set; }

	public bool IsBase
	{
		get
		{
			if (ecu.IsRollCall)
			{
				if (rollCallIdBlock != null)
				{
					return !rollCallIdBlock.Any();
				}
				return true;
			}
			return isBase;
		}
	}

	public bool IsBoot
	{
		get
		{
			long? diagnosticVersionLong = DiagnosticVersionLong;
			if (diagnosticVersionLong.HasValue)
			{
				return (diagnosticVersionLong & 0x10000) == 65536;
			}
			return false;
		}
	}

	public Part PartNumber => partNumber;

	public IDictionary<string, string> VariantAttributes => variantAttributes?.ToDictionary((KeyValuePair<string, string> kv) => kv.Key, (KeyValuePair<string, string> kv) => kv.Value);

	public long? DiagnosticVersionLong
	{
		get
		{
			switch (ecu.DiagnosisSource)
			{
			case DiagnosisSource.CaesarDatabase:
				if (idBlocks != null)
				{
					return (from x in idBlocks
						where x.DiagVersionLong.HasValue
						select x.DiagVersionLong).FirstOrDefault();
				}
				break;
			case DiagnosisSource.McdDatabase:
				if (mcdIdBlocks == null)
				{
					break;
				}
				foreach (McdDBMatchingPattern mcdIdBlock in mcdIdBlocks)
				{
					foreach (McdDBMatchingParameter dBMatchingParameter in mcdIdBlock.DBMatchingParameters)
					{
						if (dBMatchingParameter.Primitive == "ActiveDiagnosticInformation_Read" && dBMatchingParameter.ResponseParameter == "Identification")
						{
							return Convert.ToInt64(dBMatchingParameter.ExpectedValue.Value, CultureInfo.InvariantCulture);
						}
					}
				}
				break;
			}
			return null;
		}
	}

	internal IEnumerable<string> DiagServiceQualifiers
	{
		get
		{
			if (this.diagServiceQualifiers == null && !Ecu.IsRollCall)
			{
				if (!Ecu.IsMcd)
				{
					CaesarEcu val = OpenEcuHandle();
					try
					{
						this.diagServiceQualifiers = val.GetServices((ServiceTypes)16582227).Cast<string>().ToList();
					}
					finally
					{
						((IDisposable)val)?.Dispose();
					}
				}
				else
				{
					List<string> diagServiceQualifiers = new List<string>();
					foreach (McdCaesarEquivalence item in from s in McdCaesarEquivalence.FromDBLocation(GetMcdDBLocationForProtocol(ecu.ProtocolName))
						where (s.ServiceTypes & (ServiceTypes.Actuator | ServiceTypes.Adjustment | ServiceTypes.Data | ServiceTypes.Download | ServiceTypes.DiagJob | ServiceTypes.Function | ServiceTypes.Global | ServiceTypes.IOControl | ServiceTypes.Routine | ServiceTypes.Security | ServiceTypes.Session | ServiceTypes.Static | ServiceTypes.StoredData)) != 0
						select s)
					{
						McdCaesarEquivalence caesarEquivalentStructuredService = item;
						Dictionary<string, int> caesarEquivalentQualifiers = new Dictionary<string, int>();
						AcquireCaesarEquivalentQualifiers(caesarEquivalentStructuredService.Service.ResponseParameters, new List<string>());
						void AcquireCaesarEquivalentQualifiers(IEnumerable<McdDBResponseParameter> responseParameterSet, List<string> parentParameterNames)
						{
							bool flag = responseParameterSet.AllSiblingsAreStructures();
							foreach (McdDBResponseParameter item2 in responseParameterSet)
							{
								if (!item2.IsConst && !item2.IsMatchingRequestParameter && !item2.IsReserved)
								{
									if (item2.DataType != null && !item2.IsStructure && !item2.IsArray)
									{
										string serviceQualifier = caesarEquivalentStructuredService.Qualifier;
										string serviceName = caesarEquivalentStructuredService.Name;
										McdCaesarEquivalence.AdjustServiceQualifierName(caesarEquivalentStructuredService.Service, item2, ref serviceQualifier, ref serviceName);
										diagServiceQualifiers.Add(McdCaesarEquivalence.MakeQualifier(serviceQualifier + "_" + string.Join("_", parentParameterNames.Concat(Enumerable.Repeat((Ecu.CaesarEquivalentResponseParameterQualifierSource != Ecu.ResponseParameterQualifierSource.Qualifier) ? item2.Name : item2.Qualifier, 1))), caesarEquivalentQualifiers));
									}
									if (item2.Parameters.Any())
									{
										List<string> list = parentParameterNames.ToList();
										if (!item2.IsStructure || flag)
										{
											list.Add(item2.Name);
										}
										AcquireCaesarEquivalentQualifiers(item2.Parameters, list.ToList());
									}
								}
							}
						}
					}
					this.diagServiceQualifiers = diagServiceQualifiers;
				}
			}
			return this.diagServiceQualifiers;
		}
	}

	public IEnumerable<string> FlashJobQualifiers
	{
		get
		{
			if (flashJobQualifiers == null && !Ecu.IsRollCall)
			{
				List<string> list = new List<string>();
				if (!Ecu.IsMcd)
				{
					CaesarEcu val = OpenEcuHandle();
					try
					{
						foreach (string item in val.GetServices((ServiceTypes)33554432).Cast<string>().ToList())
						{
							CaesarDiagService val2 = val.OpenDiagServiceHandle(item);
							try
							{
								if (val2 != null && val2.RequestMessage == null)
								{
									list.Add(item);
								}
							}
							finally
							{
								((IDisposable)val2)?.Dispose();
							}
						}
						flashJobQualifiers = list;
					}
					finally
					{
						((IDisposable)val)?.Dispose();
					}
				}
				else
				{
					flashJobQualifiers = from j in GetMcdDBLocationForProtocol(ecu.ProtocolName).DBJobs
						where j.IsFlashJob
						select j.Qualifier;
				}
			}
			return flashJobQualifiers;
		}
	}

	public IDictionary<string, string> ControlPrimitiveQualifiers
	{
		get
		{
			if (controlPrimitiveNames == null && ecu.IsMcd)
			{
				controlPrimitiveNames = GetMcdDBLocationForProtocol(ecu.ProtocolName).DBControlPrimitives.Select((McdDBControlPrimitive cp) => Tuple.Create(cp.Qualifier, cp.Name)).ToList();
			}
			return controlPrimitiveNames?.ToDictionary((Tuple<string, string> k) => k.Item1, (Tuple<string, string> v) => v.Item2);
		}
	}

	public IDictionary<string, string> MatchingParameters
	{
		get
		{
			if (mcdIdBlocks != null && mcdIdBlocks.Count() > 0)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				{
					foreach (McdDBMatchingParameter dBMatchingParameter in mcdIdBlocks.First().DBMatchingParameters)
					{
						dictionary.Add(dBMatchingParameter.Primitive + "." + dBMatchingParameter.ResponseParameter, dBMatchingParameter.ExpectedValue?.Value?.ToString() ?? string.Empty);
					}
					return dictionary;
				}
			}
			return null;
		}
	}

	internal int RollCallIdentificationCount
	{
		get
		{
			if (rollCallIdBlock == null)
			{
				return 0;
			}
			return rollCallIdBlock.Count();
		}
	}

	internal DiagnosisVariant(Ecu ecu, string name, string description, bool isBase)
		: this(ecu, name, description, null, new List<CaesarIdBlock>())
	{
		this.isBase = isBase;
	}

	internal DiagnosisVariant(Ecu ecu, string name, string equipmentType, IEnumerable<Tuple<RollCall.ID, string>> rollCallIdBlock, IEnumerable<string> rollCallQualifiers)
		: this(ecu, name, string.Empty, null, new List<CaesarIdBlock>())
	{
		this.rollCallIdBlock = rollCallIdBlock;
		diagServiceQualifiers = rollCallQualifiers;
		FixedEquipmentType = equipmentType;
	}

	internal DiagnosisVariant(Ecu ecu, string name, string description, Part partNumber, IEnumerable<CaesarIdBlock> idBlocks)
	{
		this.name = name;
		this.ecu = ecu;
		this.description = description;
		this.partNumber = partNumber;
		this.idBlocks = idBlocks;
	}

	internal DiagnosisVariant(Ecu ecu, string name, string description, Part partNumber, IDictionary<string, string> variantAttributes, IEnumerable<McdDBMatchingPattern> idBlocks, bool isBase)
	{
		this.name = name;
		this.ecu = ecu;
		this.description = description;
		this.partNumber = partNumber;
		mcdIdBlocks = idBlocks;
		this.variantAttributes = variantAttributes;
		this.isBase = isBase;
	}

	internal McdDBLocation GetMcdDBLocationForProtocol(string protocol)
	{
		McdDBEcuBaseVariant mcdHandle = ecu.GetMcdHandle();
		if (mcdHandle != null)
		{
			if (IsBase)
			{
				return mcdHandle.GetDBLocationForProtocol(protocol);
			}
			McdDBEcuVariant dBEcuVariant = mcdHandle.GetDBEcuVariant(name);
			if (dBEcuVariant != null)
			{
				return dBEcuVariant.GetDBLocationForProtocol(protocol);
			}
		}
		return null;
	}

	internal CaesarEcu OpenEcuHandle()
	{
		CaesarEcu val = ecu.OpenEcuHandle();
		if (val != null && !IsBase)
		{
			val.SetVariant(Name);
		}
		return val;
	}

	public override string ToString()
	{
		return name;
	}

	private static bool IsMatch(Tuple<RollCall.ID, string> requiredId, Tuple<RollCall.ID, object> readId)
	{
		if (requiredId.Item1 == readId.Item1)
		{
			if (readId.Item2 is string text)
			{
				Match match = Regex.Match(text, requiredId.Item2);
				if (!match.Success)
				{
					return false;
				}
				return match.Value == text;
			}
			return requiredId.Item2 == readId.Item2.ToString();
		}
		return false;
	}

	internal bool IsMatch(IEnumerable<Tuple<RollCall.ID, object>> readIdBlock)
	{
		if (rollCallIdBlock != null)
		{
			foreach (Tuple<RollCall.ID, string> requiredId in rollCallIdBlock)
			{
				if (!readIdBlock.Any((Tuple<RollCall.ID, object> readId) => IsMatch(requiredId, readId)))
				{
					return false;
				}
			}
			return true;
		}
		return true;
	}

	internal bool IsMatch(McdLogicalLink link)
	{
		if (mcdIdBlocks != null)
		{
			return mcdIdBlocks.Any((McdDBMatchingPattern id) => id.DBMatchingParameters.All((McdDBMatchingParameter mp) => mp.IsMatch(link)));
		}
		return false;
	}

	private static IEnumerable<string> GetQualifierList(TextFieldParser parser)
	{
		if (parser == null)
		{
			yield break;
		}
		while (!parser.EndOfData)
		{
			string[] array = parser.ReadFields();
			if (array[0] == "P" && array.Length >= 10)
			{
				yield return array[3];
			}
		}
	}

	private static IEnumerable<Tuple<string, string>> GetQualifierList(CaesarEcu ecu)
	{
		StringCollection varCodeDomains = ecu.VarCodeDomains;
		List<Tuple<string, string>> list = new List<Tuple<string, string>>();
		for (int i = 0; i < varCodeDomains.Count; i++)
		{
			CaesarDIVarCodeDom val = ecu.OpenVarCodeDomain(varCodeDomains[i]);
			try
			{
				if (val == null)
				{
					continue;
				}
				uint varCodeFragCount = val.VarCodeFragCount;
				for (uint num = 0u; num < varCodeFragCount; num++)
				{
					CaesarDIVarCodeFrag val2 = val.OpenVarCodeFrag(num);
					try
					{
						if (val2 != null)
						{
							list.Add(Tuple.Create(varCodeDomains[i], val2.Qualifier));
						}
					}
					finally
					{
						((IDisposable)val2)?.Dispose();
					}
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		return list;
	}

	private static IEnumerable<Tuple<string, string>> GetQualifierList(McdDBLocation ecu)
	{
		List<Tuple<string, string>> list = new List<Tuple<string, string>>();
		foreach (McdDBService variantCodingWriteDBService in ecu.VariantCodingWriteDBServices)
		{
			string domainQualifier = McdCaesarEquivalence.GetDomainQualifier(variantCodingWriteDBService.Name);
			foreach (McdDBRequestParameter allRequestParameter in variantCodingWriteDBService.AllRequestParameters)
			{
				if (!allRequestParameter.IsConst && !allRequestParameter.IsReserved)
				{
					list.Add(Tuple.Create(domainQualifier, McdCaesarEquivalence.MakeQualifier(allRequestParameter.Name)));
				}
			}
		}
		return list;
	}

	internal T GetEcuInfoAttribute<T>(string attribute, string qualifier)
	{
		return Ecu.GetEcuInfoAttribute<T>(attribute, qualifier, Name);
	}

	internal int? GetEcuInfoReadAccessLevel(string qualifier)
	{
		return GetEcuInfoAttribute<int?>("ReadAccess", qualifier);
	}

	internal int? GetEcuInfoWriteAccessLevel(string qualifier)
	{
		return GetEcuInfoAttribute<int?>("WriteAccess", qualifier);
	}

	internal int? GetEcuInfoLimitedRangeMin(string qualifier)
	{
		return GetEcuInfoAttribute<int?>("LimitedRangeMin", qualifier);
	}

	internal int? GetEcuInfoLimitedRangeMax(string qualifier)
	{
		return GetEcuInfoAttribute<int?>("LimitedRangeMax", qualifier);
	}

	internal string GetEcuInfoProhibitedChoices(string qualifier)
	{
		return GetEcuInfoAttribute<string>("ProhibitedChoices", qualifier);
	}

	internal byte[] GetEcuInfoIgnoreNegativeResponses(string qualifier)
	{
		string ecuInfoAttribute = GetEcuInfoAttribute<string>("IgnoreNegativeResponse", qualifier);
		if (ecuInfoAttribute == null)
		{
			return null;
		}
		return new Dump(ecuInfoAttribute.Replace(", ", string.Empty)).Data.ToArray();
	}
}
