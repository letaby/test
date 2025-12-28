using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class CodingFile
{
	private string qualifier;

	private string name;

	private string description;

	private string fileName;

	private string preamble;

	private string author;

	private string date;

	private string version;

	private CodingParameterGroupCollection codingParameterGroups;

	private IList<Ecu> ecus;

	public string Qualifier => qualifier;

	public string Name => name;

	public string Description => description;

	public string FileName => fileName;

	public string Preamble => preamble;

	public string Author => author;

	public string Date => date;

	public string Version => version;

	public CodingParameterGroupCollection CodingParameterGroups => codingParameterGroups;

	public IList<Ecu> Ecus => ecus;

	internal CodingFile()
	{
		codingParameterGroups = new CodingParameterGroupCollection(null);
	}

	internal void Acquire(CaesarDICcf file)
	{
		qualifier = file.Qualifier;
		name = file.Name;
		description = file.Description;
		author = file.VarCodeAuthor;
		date = file.VarCodeDate;
		version = file.VarCodeVersion;
		fileName = file.FileName;
		preamble = file.Preamble;
		uint varCodeDomainCount = file.VarCodeDomainCount;
		for (uint num = 0u; num < varCodeDomainCount; num++)
		{
			CaesarDIVcd val = file.OpenVarCodeDomain(num);
			try
			{
				if (val != null)
				{
					CodingParameterGroup codingParameterGroup = new CodingParameterGroup(codingParameterGroups);
					codingParameterGroup.Acquire(val);
					codingParameterGroups.Add(codingParameterGroup);
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		List<Ecu> list = new List<Ecu>();
		for (uint num2 = 0u; num2 < file.EcuRefCount; num2++)
		{
			string ecuName = file.GetEcuRefByIndex(num2);
			Ecu ecu = Sapi.GetSapi().Ecus.FirstOrDefault((Ecu e) => e.Name == ecuName && e.DiagnosisSource == DiagnosisSource.CaesarDatabase);
			if (ecu != null)
			{
				list.Add(ecu);
				continue;
			}
			Sapi.GetSapi().RaiseDebugInfoEvent(this, "CodingFile " + fileName + " references unknown ECU-qualifier " + ecuName + ". A CBF is corrupt or missing.");
		}
		ecus = list.AsReadOnly();
	}

	internal void Acquire(McdDBConfigurationData file)
	{
		Sapi sapi = Sapi.GetSapi();
		qualifier = file.Qualifier;
		name = file.Name;
		description = file.Description;
		version = file.Version;
		fileName = file.DatabaseFile;
		preamble = file.Preamble;
		ecus = file.EcuNames.Select((string name) => sapi.Ecus.FirstOrDefault((Ecu ecu2) => ecu2.Name == name && ecu2.DiagnosisSource == DiagnosisSource.McdDatabase)).ToList().AsReadOnly();
		List<DiagnosisVariant> list = new List<DiagnosisVariant>();
		foreach (Ecu ecu in ecus)
		{
			if (file.EcuVariantNames.Any())
			{
				list.AddRange(from evn in file.EcuVariantNames
					select ecu.DiagnosisVariants[evn] into dv
					where dv != null
					select dv);
			}
			else
			{
				list.AddRange(ecu.DiagnosisVariants.Where((DiagnosisVariant v) => !v.IsBase && !v.IsBoot));
			}
		}
		foreach (McdDBConfigurationRecord dBConfigurationRecord in file.DBConfigurationRecords)
		{
			CodingParameterGroup codingParameterGroup = new CodingParameterGroup(codingParameterGroups);
			codingParameterGroup.Acquire(dBConfigurationRecord, list);
			codingParameterGroups.Add(codingParameterGroup);
		}
	}

	internal void Acquire(StreamReader reader, string path, Ecu ecu, Part partNumber)
	{
		name = Path.GetFileNameWithoutExtension(path);
		fileName = (qualifier = Path.GetFileName(path));
		date = Sapi.TimeToString(File.GetCreationTime(path));
		preamble = string.Empty;
		List<Ecu> list = new List<Ecu>();
		list.Add(ecu);
		ecus = new ReadOnlyCollection<Ecu>(list);
		CodingParameterGroup codingParameterGroup = new CodingParameterGroup(codingParameterGroups);
		codingParameterGroup.Acquire(reader, name, ecu, partNumber);
		codingParameterGroups.Add(codingParameterGroup);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("The GetEcus method is deprecated, please use the Ecus property instead.")]
	public Ecu[] GetEcus()
	{
		return ecus.ToArray();
	}
}
