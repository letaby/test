using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class FaultCodeCollection : LateLoadReadOnlyCollection<FaultCode>, IEnumerable<FaultCode>, IEnumerable
{
	private int faultResetDelay;

	private bool haveLoggedFaultReadException;

	private Service mcdDiagnosticTroubleCodesJob;

	private Channel channel;

	private bool autoRead;

	private bool allowEnvironmentRead;

	private byte udsFilterByte;

	private bool supportsSnapshot;

	private bool supportsPermanent;

	private bool supportsFaultRead;

	private bool hasFaultDescriptions;

	private bool haveBeenReadFromEcu;

	private bool snapshotHasBeenReadFromEcu;

	private uint snapshotRecordNumber;

	private bool uds;

	private ServiceCollection environmentDataDescriptions;

	private List<Instrument> rollcallSnapshotDescriptions = new List<Instrument>();

	public IEnumerable<FaultCode> Current => GetCurrentByFunction(ReadFunctions.NonPermanent);

	public Channel Channel => channel;

	public FaultCode this[string code] => this.FirstOrDefault((FaultCode item) => string.Equals(item.Code, code, StringComparison.Ordinal));

	public bool AutoRead
	{
		get
		{
			return autoRead;
		}
		set
		{
			autoRead = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("SyncRoot is deprecated and no longer necessary, because the collection returned by GetEnumerator is a (shallow) copy.")]
	public object SyncRoot => base.Items;

	public bool AllowEnvironmentRead
	{
		get
		{
			return allowEnvironmentRead;
		}
		set
		{
			allowEnvironmentRead = value;
		}
	}

	public ServiceCollection EnvironmentDataDescriptions => environmentDataDescriptions;

	public bool SupportsSnapshot
	{
		get
		{
			return supportsSnapshot;
		}
		set
		{
			if (value != supportsSnapshot)
			{
				supportsSnapshot = value;
				Channel.Ecu.Properties["SupportsSnapshot"] = supportsSnapshot.ToString();
			}
		}
	}

	public bool SupportsFaultRead => supportsFaultRead;

	public bool HasFaultDescriptions => hasFaultDescriptions;

	public bool HaveBeenReadFromEcu => haveBeenReadFromEcu;

	public bool SnapshotHasBeenReadFromEcu => snapshotHasBeenReadFromEcu;

	internal bool SupportsEnvironmentSnapshot { get; private set; }

	internal ServiceOutputValue McdSnapshotDescriptions { get; private set; }

	internal ServiceOutputValue McdSnapshotData { get; private set; }

	internal ServiceOutputValue McdEnvironmentDataDescriptions { get; private set; }

	internal ServiceOutputValue McdEnvironmentData { get; private set; }

	public event FaultCodesUpdateEventHandler FaultCodesUpdateEvent;

	public event SnapshotUpdateEventHandler SnapshotUpdateEvent;

	internal FaultCodeCollection(Channel c)
	{
		channel = c;
		snapshotRecordNumber = 255u;
		allowEnvironmentRead = true;
		if (channel.Ecu.Properties.ContainsKey("SupportsFaultRead"))
		{
			if (!bool.TryParse(channel.Ecu.Properties["SupportsFaultRead"], out supportsFaultRead))
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(this, "Unable to parse SupportsFaultRead property");
			}
		}
		else
		{
			supportsFaultRead = true;
		}
		uds = channel.Ecu.IsUds;
		if (uds)
		{
			udsFilterByte = 13;
			if (channel.Ecu.Properties.ContainsKey("UDSDTCFilter"))
			{
				string value = channel.Ecu.Properties["UDSDTCFilter"];
				try
				{
					udsFilterByte = Convert.ToByte(value, 16);
				}
				catch (FormatException)
				{
					Sapi.GetSapi().RaiseDebugInfoEvent(this, "Error parsing UDS DTC Filter");
				}
			}
			if (channel.Ecu.Properties.ContainsKey("SupportsSnapshot") && !bool.TryParse(channel.Ecu.Properties["SupportsSnapshot"], out supportsSnapshot))
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(this, "Unable to parse SupportsSnapshot property");
			}
			if (supportsSnapshot && channel.Ecu.Properties.ContainsKey("SnapshotRecordNumber") && !uint.TryParse(channel.Ecu.Properties["SnapshotRecordNumber"], out snapshotRecordNumber))
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(this, "Error parsing Snapshot Record Number");
			}
			if (channel.Ecu.Properties.ContainsKey("SupportsPermanentFaults") && !bool.TryParse(channel.Ecu.Properties["SupportsPermanentFaults"], out supportsPermanent))
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(this, "Unable to parse SupportsPermanentFaults property");
			}
		}
		else if (channel.IsRollCall && channel.Online && channel.Ecu.RollCallManager.IsSnapshotSupported(channel))
		{
			SupportsSnapshot = true;
		}
		if (channel.Ecu.Properties.ContainsKey("FaultResetDelay") && !int.TryParse(channel.Ecu.Properties["FaultResetDelay"], out faultResetDelay))
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(this, "Unable to parse FaultResetDelay property");
		}
		environmentDataDescriptions = new ServiceCollection(c, ServiceTypes.Environment);
		hasFaultDescriptions = true;
		if (channel.Ecu.Properties.ContainsKey("HasFaultDescriptions") && !bool.TryParse(channel.Ecu.Properties["HasFaultDescriptions"], out hasFaultDescriptions))
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(this, "Unable to parse HasFaultDescriptions property");
		}
	}

	internal void UpdateFromRollCall(Dictionary<string, byte?> codes, Type type, TimeSpan agedPersistFor)
	{
		UpdateFromRollCall(codes, type, permanent: false, agedPersistFor);
	}

	internal void UpdateFromRollCall(Dictionary<string, byte?> codes, Type type, bool permanent, TimeSpan agedPersistFor)
	{
		DateTime now = Sapi.Now;
		bool flag = false;
		foreach (KeyValuePair<string, byte?> code in codes)
		{
			FaultCode faultCode = AcquireCode(code.Key);
			flag |= faultCode.FaultCodeIncidents.AddFromRollCall(now, code.Value, type, permanent);
		}
		using (IEnumerator<FaultCode> enumerator2 = GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				FaultCode current2 = enumerator2.Current;
				if (!codes.Keys.Contains(current2.Code))
				{
					flag |= current2.FaultCodeIncidents.RemoveAgedFromRollCall(now, type, permanent, agedPersistFor);
				}
			}
		}
		if (flag)
		{
			RaiseFaultCodeUpdate(null, explicitread: false);
		}
		haveBeenReadFromEcu = true;
	}

	internal void UpdateSnapshotFromRollCall(IEnumerable<Tuple<string, IEnumerable<Tuple<Instrument, byte[]>>>> frames)
	{
		DateTime now = Sapi.Now;
		bool flag = false;
		int num = 0;
		foreach (Tuple<string, IEnumerable<Tuple<Instrument, byte[]>>> frame in frames)
		{
			FaultCode faultCode = AcquireCode(frame.Item1);
			FaultCodeIncident faultCodeIncident = new FaultCodeIncident(faultCode, now);
			faultCodeIncident.AcquireSnapshotFromRollCall(++num, frame.Item2);
			faultCode.Snapshots.Add(faultCodeIncident, readEnvironmentIfNew: false);
		}
		snapshotHasBeenReadFromEcu = true;
		using (IEnumerator<FaultCode> enumerator2 = GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				if (enumerator2.Current.Snapshots.SetCurrentTime(now))
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			FireAndForget.Invoke(this.SnapshotUpdateEvent, this, new ResultEventArgs(null));
		}
	}

	internal void InternalRead(bool explicitread)
	{
		bool flag = false;
		CaesarException ex = null;
		if (channel.ChannelHandle != null || channel.McdChannelHandle != null)
		{
			DateTime now = Sapi.Now;
			List<FaultCodeIncident> list = new List<FaultCodeIncident>();
			try
			{
				InternalReadByFunction(ReadFunctions.NonPermanent, now, list);
				if (supportsPermanent)
				{
					InternalReadByFunction(ReadFunctions.Permanent, now, list);
				}
			}
			catch (CaesarException ex2)
			{
				ex = ex2;
				if (list.Count == 0 && Sapi.GetSapi().LogFiles.Logging && !haveLoggedFaultReadException)
				{
					Sapi.GetSapi().LogFiles.LabelLog(string.Format(CultureInfo.InvariantCulture, "Fault codes may be unavailable - {0}", ex.Message), channel.Ecu);
					haveLoggedFaultReadException = true;
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				FaultCodeIncident faultCodeIncident = list[i];
				if (faultCodeIncident.FaultCode.ManipulatedValue == null)
				{
					faultCodeIncident.FaultCode.FaultCodeIncidents.Add(faultCodeIncident, uds && allowEnvironmentRead);
				}
			}
			using IEnumerator<FaultCode> enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				FaultCode current = enumerator.Current;
				if (current.ManipulatedValue != null)
				{
					current.ManipulatedValue.UpdateEndTime(now);
				}
				if (current.FaultCodeIncidents.SetCurrentTime(now))
				{
					flag = true;
				}
			}
		}
		else if (channel.IsRollCall && explicitread)
		{
			try
			{
				channel.Ecu.RollCallManager.ReadFaultCodes(channel);
			}
			catch (CaesarException ex3)
			{
				ex = ex3;
			}
		}
		if (flag || ex != null || explicitread)
		{
			RaiseFaultCodeUpdate(ex, explicitread);
		}
		haveBeenReadFromEcu = true;
		channel.SetCommunicationsState(CommunicationsState.Online);
	}

	internal void RaiseFaultCodeUpdate(Exception e, bool explicitread)
	{
		FireAndForget.Invoke(this.FaultCodesUpdateEvent, this, new ResultEventArgs(e));
		if (explicitread)
		{
			channel.SyncDone(e);
		}
	}

	internal void InternalReset()
	{
		CaesarException e = null;
		if (faultResetDelay > 0)
		{
			int num = 0;
			while (channel.ChannelRunning && num++ < faultResetDelay)
			{
				Thread.Sleep(1000);
			}
			if (!channel.ChannelRunning)
			{
				return;
			}
		}
		if (channel.ChannelHandle != null)
		{
			if (uds)
			{
				channel.ChannelHandle.ClearErrorGroup((ErrorGroup)5);
			}
			else
			{
				channel.ChannelHandle.ClearErrors();
			}
			if (channel.IsChannelErrorSet)
			{
				e = new CaesarException(channel.ChannelHandle);
			}
		}
		else if (channel.McdChannelHandle != null)
		{
			string value = channel.Ecu.Properties.GetValue("McdFaultClearService", "JobClearDTC");
			if (value != null)
			{
				try
				{
					channel.McdChannelHandle.GetService(value).Execute(0);
				}
				catch (McdException mcdError)
				{
					e = new CaesarException(mcdError);
				}
			}
		}
		else if (channel.Ecu.RollCallManager != null)
		{
			try
			{
				channel.Ecu.RollCallManager.ClearErrors(channel);
			}
			catch (CaesarException ex)
			{
				e = ex;
			}
		}
		channel.SyncDone(e);
		channel.SetCommunicationsState(CommunicationsState.Online);
	}

	internal void InternalReadSnapshot(bool explicitread)
	{
		CaesarException ex = null;
		bool flag = false;
		if (channel.ChannelHandle != null)
		{
			if (supportsSnapshot && !channel.IsChannelErrorSet)
			{
				DateTime now = Sapi.Now;
				CaesarDiagServiceIO val = channel.ChannelHandle.OpenErrorList((ErrorPartFunction)5, (ErrorGroup)5, (ErrorStatusFlag)13, (ErrorSeverityFlag)65535, (ErrorExtendedData)snapshotRecordNumber, (ErrorEnvType)0);
				try
				{
					if (val != null)
					{
						ushort errorCount = val.ErrorCount;
						for (ushort num = 0; num < errorCount; num++)
						{
							string errorComfortNumber = val.GetErrorComfortNumber(num);
							FaultCode faultCode = this[CaesarCodeToIndex(errorComfortNumber)];
							if (faultCode != null)
							{
								FaultCodeIncident faultCodeIncident = new FaultCodeIncident(faultCode, now);
								faultCodeIncident.AcquireSnapshot(val, num);
								faultCode.Snapshots.Add(faultCodeIncident, readEnvironmentIfNew: false);
							}
							else
							{
								Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Could not associate DTC of snapshot record ({0}) to a fault defined in the CBF", errorComfortNumber));
							}
						}
					}
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
				using (IEnumerator<FaultCode> enumerator = GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Snapshots.SetCurrentTime(now))
						{
							flag = true;
						}
					}
				}
				snapshotHasBeenReadFromEcu = true;
			}
			if (channel.IsChannelErrorSet && channel.ChannelHandle != null)
			{
				ex = new CaesarException(channel.ChannelHandle);
			}
		}
		else if (channel.McdChannelHandle == null && channel.IsRollCall)
		{
			try
			{
				channel.Ecu.RollCallManager.ReadSnapshot(channel);
				snapshotHasBeenReadFromEcu = true;
			}
			catch (CaesarException ex2)
			{
				ex = ex2;
			}
		}
		if (flag || ex != null || explicitread)
		{
			FireAndForget.Invoke(this.SnapshotUpdateEvent, this, new ResultEventArgs(ex));
			if (explicitread)
			{
				channel.SyncDone(ex);
			}
		}
		channel.SetCommunicationsState(CommunicationsState.Online);
	}

	internal void Invalidate()
	{
		using (IEnumerator<FaultCode> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				enumerator.Current.FaultCodeIncidents.Invalidate();
			}
		}
		RaiseFaultCodeUpdate(null, explicitread: false);
	}

	protected override void AcquireList()
	{
		if (channel.EcuHandle != null)
		{
			uint errorCount = channel.EcuHandle.ErrorCount;
			for (uint num = 0u; num < errorCount; num++)
			{
				string errorComfortCode = channel.EcuHandle.GetErrorComfortCode(num);
				Add(CaesarCodeToIndex(errorComfortCode), errorComfortCode);
			}
		}
		else
		{
			if (channel.McdEcuHandle == null)
			{
				return;
			}
			foreach (McdDBDiagTroubleCode dBDiagTroubleCode in channel.McdEcuHandle.DBDiagTroubleCodes)
			{
				Add(dBDiagTroubleCode.DisplayTroubleCode);
			}
			mcdDiagnosticTroubleCodesJob = channel.Services["DJ_JobDiagnosticTroubleCodes"];
			ServiceOutputValue serviceOutputValue = mcdDiagnosticTroubleCodesJob?.StructuredOutputValues["DtcResponseMux"]?.StructuredOutputValues["ReportDTCs"]?.StructuredOutputValues["DTC_Record"]?.StructuredOutputValues["Structure_of_DTC"];
			McdEnvironmentData = serviceOutputValue?.StructuredOutputValues["DTC_Environment_Data_Descriptor"];
			McdEnvironmentDataDescriptions = McdEnvironmentData?.StructuredOutputValues["DTCExtendedDataRecord_Wrapper"]?.StructuredOutputValues["DTC_Environment_Data_Descriptor"];
			if (!channel.Ecu.Properties.GetValue("SupportsEnvironmentSnapshotRead", defaultIfNotSet: true))
			{
				return;
			}
			McdSnapshotData = serviceOutputValue?.StructuredOutputValues["DTC_Snapshot_Data"];
			McdSnapshotDescriptions = McdSnapshotData?.StructuredOutputValues["DTCSnapshortData_Wrapper"]?.StructuredOutputValues["DIDs_and_content"]?.StructuredOutputValues["DTCSnapshortRecord_Wrapper"]?.StructuredOutputValues["Content_of_DID"];
			if (McdSnapshotDescriptions != null)
			{
				SupportsEnvironmentSnapshot = McdSnapshotDescriptions.StructuredOutputValues.Any((ServiceOutputValue sov) => sov.StructuredOutputValues != null && sov.StructuredOutputValues.Any());
				if (SupportsEnvironmentSnapshot)
				{
					channel.Ecu.Properties["PromotionPrefix"] = "ENV_SnapshotRecordNumber";
				}
			}
		}
	}

	internal void SetCurrentTime(DateTime time)
	{
		bool flag = false;
		bool flag2 = false;
		using (IEnumerator<FaultCode> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				FaultCode current = enumerator.Current;
				if (current.FaultCodeIncidents.SetCurrentTime(time))
				{
					flag = true;
				}
				if (current.Snapshots.SetCurrentTime(time))
				{
					flag2 = true;
				}
			}
		}
		if (flag)
		{
			RaiseFaultCodeUpdate(null, explicitread: false);
		}
		if (flag2)
		{
			FireAndForget.Invoke(this.SnapshotUpdateEvent, this, new ResultEventArgs(null));
		}
	}

	internal FaultCode Add(string errcode, string caesarcode = null)
	{
		FaultCode faultCode = new FaultCode(channel, errcode, caesarcode);
		faultCode.AcquireText();
		base.Items.Add(faultCode);
		return faultCode;
	}

	public void Reset(bool synchronous)
	{
		channel.QueueAction(CommunicationsState.ResetFaults, synchronous);
	}

	public void Read(bool synchronous)
	{
		channel.QueueAction(CommunicationsState.ReadFaults, synchronous);
	}

	public void ReadSnapshot(bool synchronous)
	{
		if (supportsSnapshot)
		{
			channel.QueueAction(CommunicationsState.ReadSnapshot, synchronous);
			return;
		}
		throw new InvalidOperationException("The ECU does not support snapshot read");
	}

	public IEnumerable<FaultCode> GetCurrentByFunction(ReadFunctions match)
	{
		Collection<FaultCode> collection = new Collection<FaultCode>();
		CopyCurrent(match, collection);
		return collection;
	}

	public void CopyCurrent(Collection<FaultCode> output)
	{
		CopyCurrent(ReadFunctions.NonPermanent, output);
	}

	public void CopyCurrent(ReadFunctions match, Collection<FaultCode> output)
	{
		if (output == null)
		{
			throw new ArgumentNullException("output");
		}
		lock (base.Items)
		{
			foreach (FaultCode item in this.Where((FaultCode fc) => fc.FaultCodeIncidents.GetCurrentByFunction(match) != null))
			{
				output.Add(item);
			}
		}
	}

	public FaultCode GetItemExtended(string code)
	{
		FaultCode faultCode = this[code];
		if (faultCode == null && uds)
		{
			switch (code.Length)
			{
			case 6:
			{
				string text2 = FaultCode.ConvertUdsCodeToIsoCode(code);
				if (text2 != null)
				{
					faultCode = this[text2];
				}
				break;
			}
			case 7:
			{
				string text = FaultCode.ConvertIsoCodeToUdsCode(code);
				if (text != null)
				{
					faultCode = this[text];
				}
				break;
			}
			}
		}
		return faultCode;
	}

	internal Instrument GetRollCallSnapshotDescription(string qualifier)
	{
		Instrument instrument = rollcallSnapshotDescriptions.FirstOrDefault((Instrument i) => i.Qualifier == qualifier);
		if (instrument == null)
		{
			instrument = channel.Ecu.RollCallManager.CreateBaseInstrument(channel, qualifier);
			if (instrument != null)
			{
				rollcallSnapshotDescriptions.Add(instrument);
			}
		}
		return instrument;
	}

	public new IEnumerator<FaultCode> GetEnumerator()
	{
		lock (base.Items)
		{
			return new List<FaultCode>(base.Items).GetEnumerator();
		}
	}

	IEnumerator<FaultCode> IEnumerable<FaultCode>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private FaultCode AcquireCode(CaesarDiagServiceIO dsio, ushort i)
	{
		string text = dsio.GetErrorComfortNumber(i);
		if (Channel.Ecu.FaultCodeCanBeDuplicate)
		{
			string text2 = null;
			for (ushort num = 0; num < dsio.GetErrorEnvCount(i); num++)
			{
				string errorEnvText = dsio.GetErrorEnvText(i, num);
				if (errorEnvText.StartsWith("FMI", StringComparison.Ordinal))
				{
					text2 = errorEnvText.Split(":".ToCharArray())[0];
					break;
				}
			}
			if (text2 != null)
			{
				text = text + ":" + text2;
			}
		}
		return AcquireCode(CaesarCodeToIndex(text));
	}

	private FaultCode AcquireCode(string errcode)
	{
		FaultCode faultCode = this[errcode];
		if (faultCode == null)
		{
			lock (base.Items)
			{
				faultCode = Add(errcode);
			}
		}
		return faultCode;
	}

	private void InternalReadByFunction(CaesarDiagServiceIO dsio, ReadFunctions function, DateTime thisTimeRead, List<FaultCodeIncident> outList)
	{
		if (dsio != null)
		{
			if (dsio.IsNegativeResponse)
			{
				throw new CaesarException(dsio);
			}
			for (ushort num = 0; num < dsio.ErrorCount; num++)
			{
				FaultCode faultCode = AcquireCode(dsio, num);
				if (uds && faultCode.LongNumber == 0)
				{
					faultCode.LongNumber = dsio.GetErrorLongNumber(num);
				}
				FaultCodeIncident faultCodeIncident = null;
				for (int i = 0; i < outList.Count; i++)
				{
					FaultCodeIncident faultCodeIncident2 = outList[i];
					if (faultCodeIncident2.FaultCode == faultCode)
					{
						faultCodeIncident = faultCodeIncident2;
					}
				}
				if (faultCodeIncident == null)
				{
					faultCodeIncident = new FaultCodeIncident(faultCode, thisTimeRead);
					faultCodeIncident.Acquire(dsio, num, function);
					outList.Add(faultCodeIncident);
					if (allowEnvironmentRead && !uds)
					{
						faultCodeIncident.AcquireEnvironmentData(dsio, num);
					}
				}
				else
				{
					faultCodeIncident.UpdatePartFunction(function, append: true);
				}
			}
		}
		else if (channel.IsChannelErrorSet)
		{
			throw new CaesarException(channel.ChannelHandle);
		}
	}

	private void InternalReadPermanent(DateTime thisTimeRead, List<FaultCodeIncident> outList)
	{
		if (channel.IsChannelErrorSet)
		{
			return;
		}
		ByteMessage byteMessage = new ByteMessage(channel, new Dump("1915"));
		byteMessage.InternalDoMessage(internalRequest: true);
		if (byteMessage.Response == null || byteMessage.Response.Data[0] != 89 || byteMessage.Response.Data.Count <= 3)
		{
			return;
		}
		byte[] array = byteMessage.Response.Data.Skip(3).ToArray();
		while (array.Length >= 4)
		{
			Dump dump = new Dump(array.Take(3));
			FaultCode fc = AcquireCode(dump.ToString());
			if (fc.LongNumber == 0)
			{
				fc.LongNumber = (uint)((array[0] << 16) + (array[1] << 8) + array[2]);
			}
			FaultCodeIncident faultCodeIncident = outList.FirstOrDefault((FaultCodeIncident i) => i.FaultCode == fc);
			if (faultCodeIncident == null)
			{
				outList.Add(new FaultCodeIncident(fc, thisTimeRead, (FaultCodeStatus)(array[3] | 0x100)));
			}
			else
			{
				faultCodeIncident.UpdatePartFunction(ReadFunctions.Permanent, append: true);
			}
			array = array.Skip(4).ToArray();
		}
		if (array.Length != 0)
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Implausible length of $59 response for {0}", channel.Ecu.Name));
		}
	}

	private void InternalReadByFunction(McdDiagComPrimitive dsio, ReadFunctions function, DateTime thisTimeRead, List<FaultCodeIncident> outList)
	{
		if (!dsio.IsNegativeResponse)
		{
			foreach (McdResponseParameter item in dsio.AllPositiveResponseParameters.Where((McdResponseParameter p) => p.IsDiagnosticTroubleCode))
			{
				McdDBDiagTroubleCode dBDiagTroubleCode = item.DBDiagTroubleCode;
				FaultCode faultCode = AcquireCode((dBDiagTroubleCode != null) ? dBDiagTroubleCode.DisplayTroubleCode : Convert.ToUInt32(item.Value.CodedValue, CultureInfo.InvariantCulture).ToString("X6", CultureInfo.InvariantCulture));
				FaultCodeIncident faultCodeIncident = null;
				for (int num = 0; num < outList.Count; num++)
				{
					FaultCodeIncident faultCodeIncident2 = outList[num];
					if (faultCodeIncident2.FaultCode == faultCode)
					{
						faultCodeIncident = faultCodeIncident2;
					}
				}
				if (faultCodeIncident == null)
				{
					faultCodeIncident = new FaultCodeIncident(faultCode, thisTimeRead);
					faultCodeIncident.Acquire(item.Parent.Parameters.FirstOrDefault((McdResponseParameter p) => p.Qualifier == "DTC_Status_Bits"), function);
					outList.Add(faultCodeIncident);
				}
				else
				{
					faultCodeIncident.UpdatePartFunction(function, append: true);
				}
			}
			return;
		}
		throw new CaesarException(dsio);
	}

	private void InternalReadByFunction(ReadFunctions function, DateTime thisTimeRead, List<FaultCodeIncident> outList)
	{
		if (uds)
		{
			if (channel.ChannelHandle != null)
			{
				if (function != ReadFunctions.Permanent)
				{
					CaesarDiagServiceIO val = channel.ChannelHandle.OpenErrorList((ErrorPartFunction)2, (ErrorGroup)5, (ErrorStatusFlag)udsFilterByte, (ErrorSeverityFlag)65535, (ErrorExtendedData)65535, (ErrorEnvType)0);
					try
					{
						InternalReadByFunction(val, function, thisTimeRead, outList);
						return;
					}
					finally
					{
						((IDisposable)val)?.Dispose();
					}
				}
				InternalReadPermanent(thisTimeRead, outList);
			}
			else
			{
				if (channel.McdChannelHandle == null)
				{
					return;
				}
				try
				{
					McdDiagComPrimitive service = channel.McdChannelHandle.GetService("JobDiagnosticTroubleCodes");
					service.SetInput("PartFunction", (function == ReadFunctions.Permanent) ? "ReportDTCWithPermanentStatus" : "ReportDTCbyStatusMask");
					service.SetInput("ErrorStatusFlags", "User specific");
					service.SetInput("UserSpecificErrorStatusFlag", udsFilterByte);
					service.SetInput("EnvironmentData", "No Environment");
					service.SetInput("SnapshotRecordNumber", "No Records");
					channel.McdChannelHandle.SuppressJobInfo = true;
					service.Execute(1000);
					InternalReadByFunction(service, function, thisTimeRead, outList);
				}
				catch (McdException mcdError)
				{
					throw new CaesarException(mcdError);
				}
				finally
				{
					channel.McdChannelHandle.SuppressJobInfo = false;
				}
			}
		}
		else if (channel.ChannelHandle != null)
		{
			CaesarDiagServiceIO val2 = channel.ChannelHandle.OpenErrorList((ErrorPartFunction)65535, (ErrorGroup)65535, (ErrorStatusFlag)65535, (ErrorSeverityFlag)65535, (ErrorExtendedData)65535, (ErrorEnvType)(-1));
			try
			{
				InternalReadByFunction(val2, function, thisTimeRead, outList);
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("get_ItemExtended is deprecated, please use GetItemExtended(string) instead.")]
	public FaultCode get_ItemExtended(string code)
	{
		return GetItemExtended(code);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("CopyCurrent(ArrayList) is deprecated, please use CopyCurrent(Collection<FaultCode>) instead.")]
	public void CopyCurrent(ArrayList output)
	{
		CopyCurrent(ReadFunctions.NonPermanent, output);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("CopyCurrent(ReadFunctions, ArrayList) is deprecated, please use CopyCurrent(ReadFunctions, Collection<FaultCode>) instead.")]
	public void CopyCurrent(ReadFunctions match, ArrayList output)
	{
		Collection<FaultCode> collection = new Collection<FaultCode>();
		CopyCurrent(match, collection);
		foreach (FaultCode item in collection)
		{
			output.Add(item);
		}
	}

	private string CaesarCodeToIndex(string caesarCode)
	{
		if (!uds || caesarCode.Length != 7)
		{
			return caesarCode;
		}
		return FaultCode.ConvertIsoCodeToUdsCode(caesarCode);
	}
}
