using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class CodingFileCollection : LateLoadReadOnlyCollection<CodingFile>
{
	private struct CcfVersionInfo
	{
		public readonly string EcuQualifier;

		public readonly string Path;

		public readonly Version VarcodeVersion;

		public CcfVersionInfo(string ccfFile)
		{
			CaesarRoot.AddCcfFile(ccfFile);
			if (CaesarRoot.CcfFileCount == 1)
			{
				CaesarDICcf val = CaesarRoot.OpenCcfFile(0u);
				try
				{
					EcuQualifier = val.GetEcuRefByIndex(0u);
					Path = val.FileName;
					if (!Version.TryParse(val.VarCodeVersion, out VarcodeVersion))
					{
						VarcodeVersion = new Version();
						Sapi.GetSapi().RaiseDebugInfoEvent(val.FileName, "Version cannot be parsed");
					}
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
				CaesarRoot.RemoveCcfFile(ccfFile);
				return;
			}
			throw new CaesarException(string.Format(CultureInfo.InvariantCulture, "Version number of CCF {0} could not be determined because the file could not be loaded into CAESAR. the file may be corrupt.", System.IO.Path.GetFileName(ccfFile)));
		}
	}

	public CodingFile this[string qualifier] => this.FirstOrDefault((CodingFile item) => string.Equals(item.Qualifier, qualifier, StringComparison.Ordinal));

	internal CodingFileCollection()
	{
	}

	protected override void AcquireList()
	{
		foreach (IGrouping<string, CcfVersionInfo> item in from cv in (from path in Directory.GetFiles(Sapi.GetSapi().ConfigurationItems["CCFFiles"].Value, "*.ccf")
				select new CcfVersionInfo(path)).ToList()
			orderby cv.VarcodeVersion
			group cv by cv.EcuQualifier into g
			orderby g.Key
			select g)
		{
			CcfVersionInfo element = item.Last();
			if (item.Count() > 1)
			{
				foreach (CcfVersionInfo item2 in item.Except(Enumerable.Repeat(element, 1)))
				{
					Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Multiple CCFs found for {0}, not adding {1} ({2}) to CAESAR", item2.EcuQualifier, item2.VarcodeVersion, Path.GetFileName(item2.Path)));
				}
				Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "CCF used for {0} is {1} ({2})", element.EcuQualifier, element.VarcodeVersion, Path.GetFileName(element.Path)));
			}
			CaesarRoot.AddCcfFile(element.Path);
		}
		base.Items.Clear();
		uint ccfFileCount = CaesarRoot.CcfFileCount;
		for (uint num = 0u; num < ccfFileCount; num++)
		{
			CaesarDICcf val = CaesarRoot.OpenCcfFile(num);
			try
			{
				if (val != null)
				{
					CodingFile codingFile = new CodingFile();
					codingFile.Acquire(val);
					base.Items.Add(codingFile);
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		string[] files = Directory.GetFiles(Sapi.GetSapi().ConfigurationItems["CCFFiles"].Value, "*.CPF", SearchOption.TopDirectoryOnly);
		for (int num2 = 0; num2 < files.Length; num2++)
		{
			StreamReader streamReader = new StreamReader(files[num2]);
			try
			{
				string identificationRecordValue = ParameterCollection.GetIdentificationRecordValue("PARTNUMBER", streamReader);
				if (!string.IsNullOrEmpty(identificationRecordValue))
				{
					string identificationRecordValue2 = ParameterCollection.GetIdentificationRecordValue("ECU", streamReader);
					Ecu ecu = Sapi.GetSapi().Ecus[identificationRecordValue2];
					if (ecu != null)
					{
						CodingFile codingFile2 = new CodingFile();
						codingFile2.Acquire(streamReader, files[num2], ecu, new Part(identificationRecordValue));
						base.Items.Add(codingFile2);
					}
					else
					{
						Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Unable to locate ECU defined in CPF file: {0}", identificationRecordValue2));
					}
				}
			}
			finally
			{
				streamReader.Close();
			}
		}
		foreach (string dBConfigurationDataName in McdRoot.DBConfigurationDataNames)
		{
			CodingFile codingFile3 = new CodingFile();
			codingFile3.Acquire(McdRoot.GetDBConfigurationData(dBConfigurationDataName));
			base.Items.Add(codingFile3);
		}
	}

	internal void ClearList()
	{
		base.Items.Clear();
		ResetList();
	}
}
