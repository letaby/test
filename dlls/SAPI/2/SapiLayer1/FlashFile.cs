using System;
using System.Collections.Generic;
using System.Linq;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class FlashFile
{
	private string qualifier;

	private string name;

	private string description;

	private string fileName;

	private IList<string> ecus;

	private IEnumerable<FlashArea> flashAreas;

	private DiagnosisSource diagnosisSource;

	public string Qualifier => qualifier;

	public string Name => name;

	public string Description => description;

	public string FileName => fileName;

	public IList<string> Ecus => ecus;

	public IEnumerable<FlashArea> FlashAreas => flashAreas;

	public DiagnosisSource DiagnosisSource => diagnosisSource;

	internal FlashFile()
	{
	}

	internal void Acquire(CaesarDICff file)
	{
		//IL_00ca: Expected O, but got Unknown
		diagnosisSource = DiagnosisSource.CaesarDatabase;
		qualifier = file.Qualifier;
		name = file.Name;
		description = file.Description;
		fileName = file.FileName;
		List<string> list = new List<string>();
		for (uint num = 0u; num < file.EcuRefCount; num++)
		{
			list.Add(file.GetEcuRefByIndex(num));
		}
		ecus = list.AsReadOnly();
		List<FlashArea> list2 = new List<FlashArea>();
		uint flashAreaCount = file.FlashAreaCount;
		for (uint num2 = 0u; num2 < flashAreaCount; num2++)
		{
			try
			{
				CaesarDIFlashArea flashAreaByIndex = file.GetFlashAreaByIndex(num2);
				try
				{
					if (flashAreaByIndex != null)
					{
						FlashArea flashArea = new FlashArea(null, flashAreaByIndex.Qualifier);
						flashArea.Acquire(flashAreaByIndex, num2);
						list2.Add(flashArea);
					}
				}
				finally
				{
					((IDisposable)flashAreaByIndex)?.Dispose();
				}
			}
			catch (CaesarErrorException ex)
			{
				CaesarException e = new CaesarException(ex, null, null);
				Sapi.GetSapi().RaiseExceptionEvent(this, e);
			}
		}
		flashAreas = list2.AsReadOnly();
	}

	internal void Acquire(McdDBEcuMem file)
	{
		diagnosisSource = DiagnosisSource.McdDatabase;
		qualifier = file.Qualifier;
		name = file.Name;
		description = file.Description;
		ecus = file.Ecus.ToList();
		List<FlashArea> list = new List<FlashArea>();
		uint num = 0u;
		foreach (McdDBFlashSession dBFlashSession in file.DBFlashSessions)
		{
			FlashArea flashArea = new FlashArea(null, dBFlashSession.Qualifier);
			flashArea.Acquire(dBFlashSession, ecus, num++);
			list.Add(flashArea);
		}
		flashAreas = list.AsReadOnly();
		fileName = (from fm in flashAreas.SelectMany((FlashArea fa) => fa.FlashMeanings)
			select fm.FileName).FirstOrDefault();
	}
}
