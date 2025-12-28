using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class FlashFileCollection : LateLoadReadOnlyCollection<FlashFile>
{
	public FlashFile this[string qualifier] => this.FirstOrDefault((FlashFile item) => string.Equals(item.Qualifier, qualifier, StringComparison.Ordinal));

	internal FlashFileCollection()
	{
	}

	protected override void AcquireList()
	{
		base.Items.Clear();
		Sapi.GetSapi().EnsureFlashFilesLoaded();
		AcquireCaesarList().ForEach(delegate(FlashFile ff)
		{
			base.Items.Add(ff);
		});
		AcquireMcdList().ForEach(delegate(FlashFile ff)
		{
			base.Items.Add(ff);
		});
	}

	private static List<FlashFile> AcquireCaesarList()
	{
		List<FlashFile> list = new List<FlashFile>();
		uint cffFileCount = CaesarRoot.CffFileCount;
		for (uint num = 0u; num < cffFileCount; num++)
		{
			CaesarDICff val = CaesarRoot.OpenCffFile(num);
			try
			{
				if (val != null)
				{
					FlashFile flashFile = new FlashFile();
					flashFile.Acquire(val);
					list.Add(flashFile);
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		return list;
	}

	private static List<FlashFile> AcquireMcdList()
	{
		List<FlashFile> list = new List<FlashFile>();
		foreach (string dBEcuMemName in McdRoot.DBEcuMemNames)
		{
			FlashFile flashFile = new FlashFile();
			flashFile.Acquire(McdRoot.GetDBEcuMem(dBEcuMemName));
			list.Add(flashFile);
		}
		return list;
	}

	internal void ClearList()
	{
		base.Items.Clear();
		ResetList();
	}

	internal void RebuildList(DiagnosisSource diagnosisSource)
	{
		List<FlashFile> list = new List<FlashFile>();
		switch (diagnosisSource)
		{
		case DiagnosisSource.CaesarDatabase:
			list.AddRange(AcquireCaesarList());
			list.AddRange(base.Items.Where((FlashFile ff) => ff.DiagnosisSource == DiagnosisSource.McdDatabase));
			break;
		case DiagnosisSource.McdDatabase:
			list.AddRange(base.Items.Where((FlashFile ff) => ff.DiagnosisSource == DiagnosisSource.CaesarDatabase));
			list.AddRange(AcquireMcdList());
			break;
		}
		base.Items.Clear();
		list.ForEach(delegate(FlashFile ff)
		{
			base.Items.Add(ff);
		});
	}

	public static DiagnosisSource GetFlashFileDiagnosisSource(string path)
	{
		if (!(Path.GetExtension(path).ToUpperInvariant() == ".SMR-F"))
		{
			return DiagnosisSource.CaesarDatabase;
		}
		return DiagnosisSource.McdDatabase;
	}
}
