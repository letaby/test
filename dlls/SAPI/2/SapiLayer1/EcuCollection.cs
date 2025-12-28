using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class EcuCollection : LateLoadReadOnlyCollection<Ecu>
{
	private class CbfVersionInfo
	{
		private static Regex regexGPD = new Regex("\\(\\(VERSION (?<ver>\\d+.\\d+.\\d+), (?<date>\\d+.\\d+.\\d+),", RegexOptions.Compiled);

		public readonly string EcuQualifier;

		public readonly string CbfPath;

		public readonly Version DescriptionVersion;

		public readonly bool Is64BitCompatible;

		public bool HasDiagjobs
		{
			get
			{
				bool result = false;
				CaesarRoot.AddCbfFile(CbfPath);
				IEnumerable<string> ecus = CaesarRoot.Ecus;
				if (ecus.Any())
				{
					CaesarEcu val = Ecu.OpenEcuHandle(ecus.Single());
					try
					{
						StringEnumerator enumerator = val.Variants.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								string current = enumerator.Current;
								if (val.SetVariant(current) && val.GetServices((ServiceTypes)262144).Count > 0)
								{
									result = true;
									break;
								}
							}
						}
						finally
						{
							if (enumerator is IDisposable disposable)
							{
								disposable.Dispose();
							}
						}
					}
					finally
					{
						((IDisposable)val)?.Dispose();
					}
				}
				CaesarRoot.RemoveCbfFile(CbfPath);
				return result;
			}
		}

		public CbfVersionInfo(string cbfFile)
		{
			CaesarRoot.AddCbfFile(cbfFile);
			IEnumerable<string> ecus = CaesarRoot.Ecus;
			if (ecus.Any())
			{
				CaesarEcu val = Ecu.OpenEcuHandle(ecus.Single());
				try
				{
					Is64BitCompatible = false;
					EcuQualifier = val.Name;
					CbfPath = val.FileName;
					DescriptionVersion = GetCBFVersion(cbfFile, val.DescriptionDataVersion);
					Match match = regexGPD.Match(val.GpdVersion);
					if (match.Success && Version.TryParse(match.Groups["ver"].Value, out var result))
					{
						Is64BitCompatible = result.Major >= 4;
					}
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
				CaesarRoot.RemoveCbfFile(cbfFile);
				return;
			}
			throw new CaesarException(string.Format(CultureInfo.InvariantCulture, "Version number of CBF {0} could not be determined because the file could not be loaded into CAESAR. the file may be corrupt.", Path.GetFileName(cbfFile)));
		}

		private static Version GetCBFVersion(string cbffilename, string version)
		{
			try
			{
				return new Version(version);
			}
			catch (FormatException ex)
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(cbffilename, "Version cannot be parsed: " + ex.Message);
			}
			catch (ArgumentException ex2)
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(cbffilename, "Version cannot be parsed: " + ex2.Message);
			}
			catch (OverflowException ex3)
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(cbffilename, "Version cannot be parsed: " + ex3.Message);
			}
			return new Version();
		}
	}

	public Ecu this[string name] => this.FirstOrDefault((Ecu item) => string.Equals(item.Name, name, StringComparison.Ordinal));

	public bool IgnoreDiagnosisDescriptionLoadFailure { get; set; }

	public event EcusUpdateEventHandler EcusUpdateEvent;

	internal EcuCollection()
	{
	}

	private IEnumerable<CbfVersionInfo> GetVersionInfo(string[] paths)
	{
		List<CbfVersionInfo> list = new List<CbfVersionInfo>();
		foreach (string cbfFile in paths)
		{
			try
			{
				list.Add(new CbfVersionInfo(cbfFile));
			}
			catch (CaesarException ex)
			{
				if (IgnoreDiagnosisDescriptionLoadFailure)
				{
					Sapi.GetSapi().RaiseDebugInfoEvent(this, ex.Message);
					continue;
				}
				throw;
			}
		}
		return list;
	}

	protected override void AcquireList()
	{
		//IL_02d0: Expected O, but got Unknown
		List<string> list = new List<string>();
		foreach (IGrouping<string, CbfVersionInfo> item in from cv in GetVersionInfo(Directory.GetFiles(Sapi.GetSapi().ConfigurationItems["CBFFiles"].Value, "*.cbf"))
			orderby cv.DescriptionVersion, cv.Is64BitCompatible
			group cv by cv.EcuQualifier into g
			orderby g.Key
			select g)
		{
			CbfVersionInfo cbfVersionInfo = item.LastOrDefault((CbfVersionInfo cbf) => cbf.Is64BitCompatible || !Environment.Is64BitProcess);
			CbfVersionInfo cbfVersionInfo2 = item.LastOrDefault();
			if (cbfVersionInfo == null && !cbfVersionInfo2.HasDiagjobs)
			{
				cbfVersionInfo = cbfVersionInfo2;
			}
			foreach (CbfVersionInfo item2 in item.Except(Enumerable.Repeat(cbfVersionInfo, 1)))
			{
				if (item2 == cbfVersionInfo2)
				{
					Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Greatest CBF version found for {0} is not 64-bit compatible. Not adding {1} ({2}) to CAESAR. A compatible lower version {3} available.", item2.EcuQualifier, item2.DescriptionVersion, Path.GetFileName(item2.CbfPath), (cbfVersionInfo != null) ? "is" : "is not"));
				}
				else if (item.Count() > 1)
				{
					Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Multiple CBFs found for {0}, not adding {1} ({2}) to CAESAR", item2.EcuQualifier, item2.DescriptionVersion, Path.GetFileName(item2.CbfPath)));
				}
			}
			if (cbfVersionInfo != null)
			{
				if (item.Count() > 1)
				{
					Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "CBF used for {0} is {1} ({2})", cbfVersionInfo.EcuQualifier, cbfVersionInfo.DescriptionVersion, Path.GetFileName(cbfVersionInfo.CbfPath)));
				}
				list.Add(cbfVersionInfo.CbfPath);
			}
			else
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "There are no 64-bit compatible CBFs for {0}", item.Key));
			}
		}
		list.ForEach(delegate(string usedCbf)
		{
			CaesarRoot.AddCbfFile(usedCbf);
		});
		base.Items.Clear();
		foreach (string ecu4 in CaesarRoot.Ecus)
		{
			Ecu ecu = new Ecu(ecu4, string.Empty, DiagnosisSource.CaesarDatabase);
			try
			{
				ecu.AcquireInfo();
				base.Items.Add(ecu);
			}
			catch (CaesarErrorException ex)
			{
				CaesarErrorException caesarError = ex;
				Sapi.GetSapi().RaiseExceptionEvent(ecu, new CaesarException(caesarError));
			}
		}
		foreach (string dBEcuBaseVariantName in McdRoot.DBEcuBaseVariantNames)
		{
			if (McdRoot.GetDBEcuBaseVariant(dBEcuBaseVariantName).DBEcuVariantNames.Any())
			{
				Ecu ecu2 = new Ecu(dBEcuBaseVariantName, string.Empty, DiagnosisSource.McdDatabase);
				ecu2.AcquireInfoMCD();
				base.Items.Add(ecu2);
			}
		}
		using (IEnumerator<Ecu> enumerator4 = GetEnumerator())
		{
			while (enumerator4.MoveNext())
			{
				Ecu current4 = enumerator4.Current;
				if (!current4.Properties.ContainsKey("ViaEcu"))
				{
					continue;
				}
				string[] array = current4.Properties["ViaEcu"].Split(";".ToCharArray());
				foreach (string text in array)
				{
					int num2 = text.IndexOf(',');
					Ecu ecu3 = this[(num2 != -1) ? text.Substring(0, num2) : text];
					if (ecu3 != null)
					{
						current4.AddViaEcu(ecu3, (num2 != -1) ? text.Substring(num2 + 1) : null);
					}
				}
			}
		}
		foreach (Ecu item3 in RollCallJ1708.GlobalInstance.Ecus.Union(RollCallJ1939.GlobalInstance.Ecus).Union(RollCallDoIP.GlobalInstance.Ecus))
		{
			base.Items.Add(item3);
		}
		foreach (IGrouping<string, Ecu> item4 in from e in base.Items
			group e by e.Name into g
			where g.Count() > 1
			select g)
		{
			if (item4.Count() != 2 || item4.Count((Ecu e) => e.DiagnosisSource == DiagnosisSource.CaesarDatabase) != 1 || item4.Count((Ecu e) => e.DiagnosisSource == DiagnosisSource.McdDatabase) != 1)
			{
				throw new InvalidOperationException("Ecu names must be unique. The Ecu '" + item4.Key + "' is defined in multiple locations.");
			}
		}
		FireAndForget.Invoke(this.EcusUpdateEvent, this, new EventArgs());
	}

	internal void Remove(Ecu ecu)
	{
		base.Items.Remove(ecu);
	}

	public Ecu AddDescriptionFile(string path)
	{
		//IL_007e: Expected O, but got Unknown
		try
		{
			IEnumerable<string> ecus = CaesarRoot.Ecus;
			CaesarRoot.AddCbfFile(path);
			Ecu ecu = new Ecu(CaesarRoot.Ecus.Except(ecus).FirstOrDefault(), string.Empty, DiagnosisSource.CaesarDatabase);
			Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Adding CBF: {0}", ecu.Name));
			ecu.AcquireInfo();
			base.Items.Add(ecu);
			FireAndForget.Invoke(this.EcusUpdateEvent, this, new EventArgs());
			return ecu;
		}
		catch (CaesarErrorException ex)
		{
			throw new CaesarException(ex, null, null);
		}
	}

	internal void ClearList()
	{
		if (base.Acquired)
		{
			string cbfPath = Sapi.GetSapi().ConfigurationItems["CBFFiles"].Value;
			foreach (string item in (IEnumerable<string>)base.Items.Select((Ecu e) => Path.Combine(cbfPath, e.DescriptionFileName)).ToList())
			{
				CaesarRoot.RemoveCbfFile(item);
			}
		}
		base.Items.Clear();
		ResetList();
		FireAndForget.Invoke(this.EcusUpdateEvent, this, new EventArgs());
	}

	internal DiagnosisVariant GetDiagnosisVariantFromIDBlock(object identifier, CaesarIdBlock idBlock)
	{
		using (IEnumerator<Ecu> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Ecu current = enumerator.Current;
				if (object.Equals(identifier, current.Identifier))
				{
					DiagnosisVariant diagnosisVariantFromIDBlock = current.GetDiagnosisVariantFromIDBlock(idBlock);
					if (diagnosisVariantFromIDBlock != null)
					{
						return diagnosisVariantFromIDBlock;
					}
				}
			}
		}
		return null;
	}

	internal DiagnosisVariant GetDiagnosisVariantFromIDBlock(object identifier, McdLogicalLink link)
	{
		using (IEnumerator<Ecu> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Ecu current = enumerator.Current;
				if (object.Equals(identifier, current.Identifier))
				{
					DiagnosisVariant diagnosisVariant = current.DiagnosisVariants.FirstOrDefault((DiagnosisVariant v) => v.IsMatch(link));
					if (diagnosisVariant != null)
					{
						return diagnosisVariant;
					}
				}
			}
		}
		return null;
	}

	internal Ecu GetBestEcuFromVariant(string identifier, Predicate<DiagnosisVariant> predicate)
	{
		var source = from ecu in this
			where ecu.Identifier == identifier
			let matchCount = ecu.DiagnosisVariants.Count((DiagnosisVariant v) => predicate(v))
			where matchCount > 0
			select new
			{
				Ecu = ecu,
				Count = matchCount
			};
		if (!source.Any())
		{
			return null;
		}
		return source.OrderBy(r => r.Count).Last().Ecu;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("get_ConnectedCountForIdentifier is deprecated, please use GetConnectedCountForIdentifier(string) instead.")]
	public int get_ConnectedCountForIdentifier(string identifier)
	{
		return GetConnectedCountForIdentifier(identifier);
	}

	public int GetConnectedCountForIdentifier(string identifier)
	{
		return this.Where((Ecu ecu) => identifier == ecu.Identifier).Sum((Ecu ecu) => ecu.ConnectedChannelCount);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("get_MarkedForAutoConnect is deprecated, please use GetMarkedForAutoConnect(string) instead.")]
	public bool get_MarkedForAutoConnect(string identifier)
	{
		return GetMarkedForAutoConnect(identifier);
	}

	public bool GetMarkedForAutoConnect(string identifier)
	{
		List<Ecu> obj = this.Where((Ecu ecu) => object.Equals(identifier, ecu.Identifier)).ToList();
		if (((ICollection<Ecu>)obj).Count == 0)
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Identifier {0} is not assigned to any Ecus", identifier));
		}
		return !obj.Any((Ecu ecu) => !ecu.MarkedForAutoConnect);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("set_MarkedForAutoConnect is deprecated, please use SetMarkedForAutoConnect(string, bool) instead.")]
	public void set_MarkedForAutoConnect(string identifier, bool marked)
	{
		SetMarkedForAutoConnect(identifier, marked);
	}

	public void SetMarkedForAutoConnect(string identifier, bool marked)
	{
		List<Ecu> obj = this.Where((Ecu ecu) => object.Equals(identifier, ecu.Identifier)).ToList();
		if (((ICollection<Ecu>)obj).Count == 0)
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Identifier {0} is not assigned to any Ecus", identifier));
		}
		foreach (Ecu item in (IEnumerable<Ecu>)obj)
		{
			item.MarkedForAutoConnect = marked;
		}
	}
}
