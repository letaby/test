using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class FaultCodeIncident
{
	private const string RecordNumberPrefix = "Case_";

	private const string RecordNumberDefault = "Case_Default";

	private EnvironmentDataCollection environmentDatas;

	private FaultCode faultCode;

	private MilStatus mil;

	private StoredStatus stored;

	private ActiveStatus active;

	private TestNotCompleteStatus testNotComplete;

	private TestFailedSinceLastClearStatus testFailedSinceLastClear;

	private ImmediateStatus immediate;

	private PendingStatus pending;

	private ReadFunctions functions;

	private DateTime startTime;

	private DateTime endTime;

	public FaultCode FaultCode => faultCode;

	public MilStatus Mil => mil;

	public StoredStatus Stored => stored;

	public ActiveStatus Active => active;

	public TestNotCompleteStatus TestNotComplete => testNotComplete;

	public TestFailedSinceLastClearStatus TestFailedSinceLastClear => testFailedSinceLastClear;

	public PendingStatus Pending => pending;

	public ImmediateStatus Immediate => immediate;

	public EnvironmentDataCollection EnvironmentDatas => environmentDatas;

	public DateTime StartTime => startTime;

	public DateTime EndTime => endTime;

	public ReadFunctions Functions => functions;

	public Choice Value
	{
		get
		{
			FaultCodeStatus faultCodeStatus = FaultCodeStatus.None;
			if (Active == ActiveStatus.Active)
			{
				faultCodeStatus |= FaultCodeStatus.Active;
			}
			if (Stored == StoredStatus.Stored)
			{
				faultCodeStatus |= FaultCodeStatus.Stored;
			}
			if (Pending == PendingStatus.Pending)
			{
				faultCodeStatus |= FaultCodeStatus.Pending;
			}
			if (Mil == MilStatus.On)
			{
				faultCodeStatus |= FaultCodeStatus.Mil;
			}
			if (TestFailedSinceLastClear == TestFailedSinceLastClearStatus.TestFailedSinceLastClear)
			{
				faultCodeStatus |= FaultCodeStatus.TestFailedSinceLastClear;
			}
			if (Immediate == ImmediateStatus.Immediate)
			{
				faultCodeStatus |= FaultCodeStatus.Immediate;
			}
			if ((Functions & ReadFunctions.Permanent) != ReadFunctions.None)
			{
				faultCodeStatus |= FaultCodeStatus.Permanent;
			}
			return FaultCode.Channel.CompleteFaultCodeStatusChoices.GetItemFromRawValue(faultCodeStatus);
		}
	}

	private string SnapshotContent
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < environmentDatas.Count; i++)
			{
				EnvironmentData environmentData = environmentDatas[i];
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}\r\n", environmentData.ToString());
			}
			return stringBuilder.ToString();
		}
	}

	internal FaultCodeIncident(FaultCode faultCode, DateTime thisTimeRead, FaultCodeStatus status)
		: this(faultCode, thisTimeRead, ((status & FaultCodeStatus.Active) != FaultCodeStatus.None) ? ActiveStatus.Active : ActiveStatus.NotActive, ((status & FaultCodeStatus.Stored) != FaultCodeStatus.None) ? StoredStatus.Stored : StoredStatus.NotStored, ((status & FaultCodeStatus.Pending) != FaultCodeStatus.None) ? PendingStatus.Pending : PendingStatus.NotPending, ((status & FaultCodeStatus.Mil) != FaultCodeStatus.None) ? MilStatus.On : MilStatus.Off, ((status & FaultCodeStatus.TestFailedSinceLastClear) != FaultCodeStatus.None) ? TestFailedSinceLastClearStatus.TestFailedSinceLastClear : TestFailedSinceLastClearStatus.TestNotFailedSinceLastClear, ((status & FaultCodeStatus.Immediate) != FaultCodeStatus.None) ? ImmediateStatus.Immediate : ImmediateStatus.NotImmediate, ((status & FaultCodeStatus.Permanent) == 0) ? ReadFunctions.NonPermanent : (ReadFunctions.NonPermanent | ReadFunctions.Permanent))
	{
	}

	internal FaultCodeIncident(FaultCode faultCode, DateTime thisTimeRead, ActiveStatus activeStatus, StoredStatus storedStatus, PendingStatus pendingStatus, MilStatus milStatus, TestFailedSinceLastClearStatus testFailedSinceLastClearStatus, ImmediateStatus immediateStatus, ReadFunctions functions)
		: this(faultCode, thisTimeRead)
	{
		active = activeStatus;
		stored = storedStatus;
		pending = pendingStatus;
		testFailedSinceLastClear = testFailedSinceLastClearStatus;
		mil = milStatus;
		immediate = immediateStatus;
		this.functions = functions;
		CheckOnBoardDiagnosticState();
	}

	internal FaultCodeIncident(FaultCode faultCode, DateTime thisTimeRead)
		: this(faultCode)
	{
		startTime = thisTimeRead;
		endTime = thisTimeRead;
	}

	internal FaultCodeIncident(FaultCode faultCode)
	{
		mil = MilStatus.Undefined;
		stored = StoredStatus.Undefined;
		active = ActiveStatus.Undefined;
		testNotComplete = TestNotCompleteStatus.Undefined;
		testFailedSinceLastClear = TestFailedSinceLastClearStatus.Undefined;
		immediate = ImmediateStatus.Undefined;
		pending = PendingStatus.Undefined;
		this.faultCode = faultCode;
		environmentDatas = new EnvironmentDataCollection();
	}

	internal void Acquire(CaesarDiagServiceIO dsio, ushort i, ReadFunctions singleFunction)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected I4, but got Unknown
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected I4, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected I4, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected I4, but got Unknown
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected I4, but got Unknown
		mil = (MilStatus)dsio.GetErrorMil(i);
		stored = (StoredStatus)dsio.GetErrorStored(i);
		active = (ActiveStatus)dsio.GetErrorActive(i);
		testNotComplete = (TestNotCompleteStatus)dsio.GetErrorTestNotComplete(i);
		pending = (PendingStatus)dsio.GetErrorPending(i);
		ushort errorStatus = dsio.GetErrorStatus(i);
		testFailedSinceLastClear = (((errorStatus & 0x20) == 32) ? TestFailedSinceLastClearStatus.TestFailedSinceLastClear : TestFailedSinceLastClearStatus.TestNotFailedSinceLastClear);
		UpdatePartFunction(singleFunction, append: false);
	}

	internal void Acquire(McdResponseParameter statuses, ReadFunctions singleFunction)
	{
		foreach (McdResponseParameter parameter in statuses.Parameters)
		{
			bool? flag = null;
			object codedValue = parameter.Value.CodedValue;
			if (codedValue != null)
			{
				Type type = codedValue.GetType();
				if (type == typeof(bool))
				{
					flag = (bool)codedValue;
				}
				else if (type == typeof(uint))
				{
					flag = (((uint)codedValue != 0) ? true : false);
				}
				else if (type == typeof(string) && codedValue is string text)
				{
					int result2;
					if (bool.TryParse(text, out var result))
					{
						flag = result;
					}
					else if (int.TryParse(text, out result2))
					{
						flag = ((result2 != 0) ? true : false);
					}
				}
			}
			if (flag.HasValue)
			{
				switch (parameter.Qualifier)
				{
				case "DTC_Status_Active":
					active = (flag.Value ? ActiveStatus.Active : ActiveStatus.NotActive);
					break;
				case "DTC_Status_Pending":
					pending = (flag.Value ? PendingStatus.Pending : PendingStatus.NotPending);
					break;
				case "DTC_Status_Stored":
					stored = (flag.Value ? StoredStatus.Stored : StoredStatus.NotStored);
					break;
				case "DTC_Status_FailedSinceLastClear":
					testFailedSinceLastClear = (flag.Value ? TestFailedSinceLastClearStatus.TestFailedSinceLastClear : TestFailedSinceLastClearStatus.TestNotFailedSinceLastClear);
					break;
				case "DTC_Status_MIL":
					mil = (flag.Value ? MilStatus.On : MilStatus.Off);
					break;
				}
			}
		}
		UpdatePartFunction(singleFunction, append: false);
	}

	internal void UpdateEndTime(DateTime time)
	{
		endTime = time;
	}

	internal void UpdateStartTime(DateTime time)
	{
		startTime = time;
	}

	internal void UpdatePartFunction(ReadFunctions singleFunction, bool append)
	{
		if (append)
		{
			functions |= singleFunction;
		}
		else
		{
			functions = singleFunction;
		}
	}

	internal static FaultCodeIncident FromXElement(XElement element, LogFileFormatTagCollection format, Channel channel)
	{
		string value = element.Attribute(format[TagName.Code]).Value;
		FaultCode faultCode = channel.FaultCodes.GetItemExtended(value);
		if (faultCode == null)
		{
			faultCode = channel.FaultCodes.Add(value);
			if (channel.Ecu.RollCallManager == null)
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(Sapi.GetSapi(), string.Format(CultureInfo.InvariantCulture, "Fault code not defined in CBF: {0}", value));
			}
		}
		FaultCodeIncident faultCodeIncident = new FaultCodeIncident(faultCode);
		faultCodeIncident.startTime = Sapi.TimeFromString(element.Attribute(format[TagName.StartTime]).Value);
		faultCodeIncident.endTime = Sapi.TimeFromString(element.Attribute(format[TagName.EndTime]).Value);
		XAttribute xAttribute = element.Attribute(format[TagName.ReadFunctions]);
		faultCodeIncident.functions = ((xAttribute == null) ? ReadFunctions.NonPermanent : ((ReadFunctions)Convert.ToUInt32(xAttribute.Value, CultureInfo.InvariantCulture)));
		if (faultCodeIncident.Functions != ReadFunctions.Snapshot)
		{
			faultCodeIncident.mil = (MilStatus)Convert.ToByte(element.Element(format[TagName.MaintenanceIndicatorLamp]).Value, CultureInfo.InvariantCulture);
			faultCodeIncident.stored = (StoredStatus)Convert.ToByte(element.Element(format[TagName.Stored]).Value, CultureInfo.InvariantCulture);
			faultCodeIncident.active = (ActiveStatus)Convert.ToByte(element.Element(format[TagName.Active]).Value, CultureInfo.InvariantCulture);
			XElement xElement = element.Element(format[TagName.Pending]);
			XElement xElement2 = element.Element(format[TagName.TestNotComplete]);
			XElement xElement3 = element.Element(format[TagName.TestFailedSinceLastClear]);
			if (xElement != null)
			{
				faultCodeIncident.pending = (PendingStatus)Convert.ToByte(xElement.Value, CultureInfo.InvariantCulture);
			}
			if (xElement2 != null)
			{
				faultCodeIncident.testNotComplete = (TestNotCompleteStatus)Convert.ToByte(xElement2.Value, CultureInfo.InvariantCulture);
			}
			if (xElement3 != null)
			{
				faultCodeIncident.testFailedSinceLastClear = (TestFailedSinceLastClearStatus)Convert.ToByte(xElement3.Value, CultureInfo.InvariantCulture);
			}
			XElement xElement4 = element.Element(format[TagName.Immediate]);
			if (xElement4 != null)
			{
				faultCodeIncident.immediate = (ImmediateStatus)Convert.ToByte(xElement4.Value, CultureInfo.InvariantCulture);
			}
		}
		else if (channel.IsRollCall)
		{
			channel.FaultCodes.SupportsSnapshot = true;
		}
		EnvironmentDataCollection environmentDataCollection = new EnvironmentDataCollection();
		foreach (XElement item in element.Elements(format[TagName.EnvironmentDatas]).First().Elements(format[TagName.EnvironmentData]))
		{
			EnvironmentData environmentData = EnvironmentData.FromXElement(item, format, faultCodeIncident);
			if (environmentData != null)
			{
				environmentDataCollection.Add(environmentData);
			}
		}
		faultCodeIncident.BuildCompoundContent(environmentDataCollection);
		faultCodeIncident.CheckOnBoardDiagnosticState();
		return faultCodeIncident;
	}

	internal XElement GetXElement(DateTime startTime, DateTime endTime)
	{
		if (EndTime < startTime || StartTime > endTime)
		{
			return null;
		}
		DateTime time = ((StartTime > startTime) ? StartTime : startTime);
		DateTime time2 = ((EndTime < endTime) ? EndTime : endTime);
		LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
		XElement xElement = new XElement(currentFormat[TagName.FaultCode], new XAttribute(currentFormat[TagName.Code], FaultCode.Code), new XAttribute(currentFormat[TagName.StartTime], Sapi.TimeToString(time)), new XAttribute(currentFormat[TagName.EndTime], Sapi.TimeToString(time2)), new XAttribute(currentFormat[TagName.ReadFunctions], Functions.ToNumberString()));
		if (Functions != ReadFunctions.Snapshot)
		{
			xElement.Add(new XElement(currentFormat[TagName.MaintenanceIndicatorLamp], Mil.ToNumberString()));
			xElement.Add(new XElement(currentFormat[TagName.Stored], Stored.ToNumberString()));
			xElement.Add(new XElement(currentFormat[TagName.Active], Active.ToNumberString()));
			xElement.Add(new XElement(currentFormat[TagName.Pending], Pending.ToNumberString()));
			xElement.Add(new XElement(currentFormat[TagName.TestNotComplete], TestNotComplete.ToNumberString()));
			xElement.Add(new XElement(currentFormat[TagName.TestFailedSinceLastClear], TestFailedSinceLastClear.ToNumberString()));
			xElement.Add(new XElement(currentFormat[TagName.Immediate], Immediate.ToNumberString()));
		}
		XElement xElement2 = new XElement(currentFormat[TagName.EnvironmentDatas]);
		foreach (EnvironmentData item in EnvironmentDatas.Where((EnvironmentData ed) => ed.CompoundDescription == null))
		{
			xElement2.Add(item.XElement);
		}
		xElement.Add(xElement2);
		return xElement;
	}

	internal void InternalReadEnvironmentData()
	{
		if (faultCode.Channel.IsChannelErrorSet)
		{
			return;
		}
		if (faultCode.Channel.ChannelHandle != null)
		{
			CaesarDiagServiceIO val = faultCode.Channel.ChannelHandle.OpenErrorList((ErrorPartFunction)6, (ErrorGroup)faultCode.LongNumber, (ErrorStatusFlag)13, (ErrorSeverityFlag)65535, (ErrorExtendedData)65535, (ErrorEnvType)(-1));
			try
			{
				if (val != null)
				{
					AcquireEnvironmentData(val, 0);
				}
				return;
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		if (faultCode.Channel.McdChannelHandle == null)
		{
			return;
		}
		McdDiagComPrimitive service = faultCode.Channel.McdChannelHandle.GetService("JobDiagnosticTroubleCodes");
		service.SetInput("PartFunction", "ReportDTCbyDTCNumber");
		service.SetInput("DTC_or_Group_Number", faultCode.Code);
		service.SetInput("EnvironmentData", "All Environment");
		service.SetInput("SnapshotRecordNumber", faultCode.Channel.FaultCodes.SupportsEnvironmentSnapshot ? "All Records" : "No Records");
		try
		{
			faultCode.Channel.McdChannelHandle.SuppressJobInfo = true;
			service.Execute(0, preferResultsOverErrors: true);
			AcquireEnvironmentData(service);
		}
		catch (McdException ex)
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(faultCode.Channel, "InternalReadEnvironmentData: " + ex.Message);
		}
		finally
		{
			faultCode.Channel.McdChannelHandle.SuppressJobInfo = false;
		}
	}

	internal void AcquireEnvironmentDataFromRollCall(byte? occurrenceCount)
	{
		if (occurrenceCount.HasValue)
		{
			Service description = FaultCode.Channel.FaultCodes.EnvironmentDataDescriptions[RollCall.ID.OccurrenceCount.ToNumberString()];
			EnvironmentDatas.Add(new EnvironmentData(this, description, occurrenceCount.Value));
		}
	}

	private void AcquireEnvironmentData(McdDiagComPrimitive dsio)
	{
		if (dsio.IsNegativeResponse)
		{
			return;
		}
		EnvironmentDataCollection temporaryEnvData = new EnvironmentDataCollection();
		ServiceOutputValue envDataRootDesc = faultCode.Channel.FaultCodes.McdEnvironmentData;
		McdResponseParameter mcdResponseParameter = dsio.AllPositiveResponseParameters.FirstOrDefault((McdResponseParameter p) => p.QualifierPath == envDataRootDesc.McdParameterQualifierPath);
		if (mcdResponseParameter != null)
		{
			foreach (McdResponseParameter envSet in mcdResponseParameter.AllParameters.Where((McdResponseParameter p) => p.IsEnvironmentalData || p.Qualifier.StartsWith("Case_", StringComparison.OrdinalIgnoreCase)))
			{
				AcquireEnvironmentData(envSet, faultCode.Channel.FaultCodes.McdEnvironmentDataDescriptions.StructuredOutputValues.FirstOrDefault((ServiceOutputValue sov) => sov.ParameterQualifier == envSet.Qualifier), 0);
			}
		}
		if (faultCode.Channel.FaultCodes.SupportsEnvironmentSnapshot)
		{
			ServiceOutputValue snapshotDataRootDesc = faultCode.Channel.FaultCodes.McdSnapshotData;
			McdResponseParameter mcdResponseParameter2 = dsio.AllPositiveResponseParameters.FirstOrDefault((McdResponseParameter p) => p.QualifierPath == snapshotDataRootDesc.McdParameterQualifierPath);
			if (mcdResponseParameter2 != null)
			{
				foreach (McdResponseParameter parameter in mcdResponseParameter2.Parameters)
				{
					McdResponseParameter snapshotRecordNumber = parameter.Parameters.FirstOrDefault((McdResponseParameter p) => p.Qualifier == "SnapshotRecordNumber");
					if (snapshotRecordNumber == null || snapshotRecordNumber.Value == null || snapshotRecordNumber.Value.CodedValue == null)
					{
						continue;
					}
					int num = Convert.ToInt32(snapshotRecordNumber.Value.CodedValue, CultureInfo.InvariantCulture);
					EnvironmentData environmentData = new EnvironmentData(this, snapshotDataRootDesc.StructuredOutputValues.First().StructuredOutputValues.FirstOrDefault((ServiceOutputValue ov) => ov.ParameterQualifier == snapshotRecordNumber.Qualifier), num, null);
					environmentData.Read(snapshotRecordNumber);
					temporaryEnvData.Add(environmentData);
					foreach (McdResponseParameter envSet2 in parameter.AllParameters.Where((McdResponseParameter p) => p.Qualifier.StartsWith("Case_", StringComparison.OrdinalIgnoreCase)))
					{
						AcquireEnvironmentData(envSet2, faultCode.Channel.FaultCodes.McdSnapshotDescriptions.StructuredOutputValues.FirstOrDefault((ServiceOutputValue sov) => sov.ParameterQualifier == envSet2.Qualifier), num);
					}
				}
			}
		}
		if (temporaryEnvData.Any())
		{
			BuildCompoundContent(temporaryEnvData);
		}
		void AcquireEnvironmentData(McdResponseParameter mcdResponseParameter3, ServiceOutputValue descriptionStruct, int recordIndex)
		{
			if (descriptionStruct != null)
			{
				foreach (McdResponseParameter item in mcdResponseParameter3.AllParameters)
				{
					ServiceOutputValue serviceOutputValue = descriptionStruct.StructuredOutputValues.FirstOrDefault((ServiceOutputValue ov) => ov.ParameterQualifier == item.Qualifier);
					if (serviceOutputValue != null)
					{
						EnvironmentData environmentData2 = new EnvironmentData(this, serviceOutputValue, recordIndex, null);
						environmentData2.Read(item);
						temporaryEnvData.Add(environmentData2);
					}
				}
			}
		}
	}

	internal void AcquireEnvironmentData(CaesarDiagServiceIO dsio, ushort i)
	{
		ushort errorEnvCount = dsio.GetErrorEnvCount(i);
		if (errorEnvCount <= 0)
		{
			return;
		}
		EnvironmentDataCollection environmentDataCollection = new EnvironmentDataCollection();
		for (ushort num = 0; num < errorEnvCount; num++)
		{
			Service service = FaultCode.Channel.FaultCodes.EnvironmentDataDescriptions[dsio.GetErrorEnvQualifier(i, num)];
			if (service != null)
			{
				EnvironmentData environmentData = new EnvironmentData(this, service, null);
				environmentData.Read(dsio, i, num);
				environmentDataCollection.Add(environmentData);
			}
		}
		BuildCompoundContent(environmentDataCollection);
	}

	private void BuildCompoundContent(EnvironmentDataCollection temporaryEnvData)
	{
		foreach (CompoundEnvironmentData compoundData in faultCode.Channel.Ecu.CompoundEnvironmentDatas)
		{
			if (temporaryEnvData[compoundData.Qualifier] != null)
			{
				continue;
			}
			EnvironmentData[] array = new EnvironmentData[compoundData.Referenced.Count];
			int l;
			for (l = 0; l < compoundData.Referenced.Count; l++)
			{
				EnvironmentData environmentData = temporaryEnvData.FirstOrDefault((EnvironmentData item) => Regex.Match(item.Qualifier, compoundData.Referenced[l]).Success);
				if (environmentData != null)
				{
					array[l] = environmentData;
					if (compoundData.HideComponents)
					{
						environmentData.Visible = false;
					}
				}
			}
			EnvironmentData environmentDataValue = new EnvironmentData(this, compoundData, array);
			EnvironmentDatas.Add(environmentDataValue);
		}
		for (int num = 0; num < temporaryEnvData.Count; num++)
		{
			EnvironmentData environmentDataValue2 = temporaryEnvData[num];
			EnvironmentDatas.Add(environmentDataValue2);
		}
	}

	internal void AcquireSnapshot(CaesarDiagServiceIO dsio, ushort dtcIndex)
	{
		UpdatePartFunction(ReadFunctions.Snapshot, append: false);
		ushort errorSnapshotRecordCount = dsio.GetErrorSnapshotRecordCount(dtcIndex);
		for (ushort num = 0; num < errorSnapshotRecordCount; num++)
		{
			uint errorSnapshotCount = dsio.GetErrorSnapshotCount(dtcIndex, num);
			uint errorSnapshotRecordNumber = dsio.GetErrorSnapshotRecordNumber(dtcIndex, num);
			for (ushort num2 = 0; num2 < errorSnapshotCount; num2++)
			{
				uint errorSnapshotDiagServiceIOCount = dsio.GetErrorSnapshotDiagServiceIOCount(dtcIndex, num, num2);
				for (ushort num3 = 0; num3 < errorSnapshotDiagServiceIOCount; num3++)
				{
					CaesarDiagServiceIO errorSnapshotDiagServiceIO = dsio.GetErrorSnapshotDiagServiceIO(dtcIndex, num, num2, num3);
					try
					{
						if (errorSnapshotDiagServiceIO != null)
						{
							if (faultCode.Channel.IsChannelErrorSet)
							{
								CaesarException ex = new CaesarException(faultCode.Channel.ChannelHandle);
								if (ex.ErrorNumber != 6058)
								{
									Sapi.GetSapi().RaiseExceptionEvent(faultCode, ex);
								}
							}
							CaesarDiagService diagService = errorSnapshotDiagServiceIO.DiagService;
							if (diagService != null)
							{
								Service service = new Service(faultCode.Channel, ServiceTypes.None, diagService.Qualifier);
								service.Acquire(diagService);
								object presentation = service.OutputValues[0].GetPresentation(errorSnapshotDiagServiceIO);
								EnvironmentData environmentDataValue = new EnvironmentData(this, service, (int)errorSnapshotRecordNumber, presentation);
								environmentDatas.Add(environmentDataValue);
							}
						}
					}
					finally
					{
						((IDisposable)errorSnapshotDiagServiceIO)?.Dispose();
					}
				}
			}
		}
	}

	internal void AcquireSnapshotFromRollCall(int recordNumber, IEnumerable<Tuple<Instrument, byte[]>> content)
	{
		UpdatePartFunction(ReadFunctions.Snapshot, append: false);
		foreach (Tuple<Instrument, byte[]> item in content)
		{
			try
			{
				environmentDatas.Add(new EnvironmentData(this, item.Item1, recordNumber, item.Item1.GetPresentation(item.Item2, 1, 1)));
			}
			catch (CaesarException ex)
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(faultCode.Channel.SourceAddress.Value, "AcquireSnapshotFromRollCall: " + ex.Message + " while retrieving presentation for " + item.Item1.Name);
			}
		}
	}

	internal bool IsEquivalent(FaultCodeIncident obj)
	{
		if (obj != null && Functions == obj.Functions)
		{
			if (Functions != ReadFunctions.Snapshot)
			{
				if (Mil == obj.Mil && Stored == obj.Stored && Active == obj.Active && Pending == obj.Pending && Immediate == obj.Immediate && TestFailedSinceLastClear == obj.TestFailedSinceLastClear)
				{
					return TestNotComplete == obj.TestNotComplete;
				}
				return false;
			}
			return SnapshotContent.Equals(obj.SnapshotContent);
		}
		return false;
	}

	private void CheckOnBoardDiagnosticState()
	{
		if (FaultCode.Channel.Ecu.RollCallManager != null && (Pending != PendingStatus.Undefined || Stored != StoredStatus.Undefined || Mil != MilStatus.Undefined || (functions & ReadFunctions.Permanent) != ReadFunctions.None) && !FaultCode.Channel.Ecu.Properties.ContainsKey("SupportsPendingFaults"))
		{
			FaultCode.Channel.Ecu.Properties["SupportsPendingFaults"] = true.ToString();
			FaultCode.Channel.ResetFaultCodeStatusChoices();
			FaultCode.Channel.FaultCodes.RaiseFaultCodeUpdate(null, explicitread: false);
		}
	}

	internal bool IsStatusClarifiedBy(FaultCodeIncident newIncident)
	{
		if (Functions != newIncident.Functions)
		{
			return false;
		}
		if (Active != newIncident.Active)
		{
			return false;
		}
		if ((Stored != StoredStatus.Undefined && Stored != newIncident.Stored) || (Pending != PendingStatus.Undefined && Pending != newIncident.Pending) || (Mil != MilStatus.Undefined && Mil != newIncident.Mil) || (Immediate != ImmediateStatus.Undefined && Immediate != newIncident.Immediate) || (TestFailedSinceLastClear != TestFailedSinceLastClearStatus.Undefined && TestFailedSinceLastClear != newIncident.TestFailedSinceLastClear))
		{
			return false;
		}
		if ((Stored != StoredStatus.Undefined || newIncident.Stored != StoredStatus.NotStored) && (Pending != PendingStatus.Undefined || newIncident.Pending != PendingStatus.NotPending) && (Mil != MilStatus.Undefined || newIncident.Mil != MilStatus.Off) && (Immediate != ImmediateStatus.Undefined || newIncident.Immediate != ImmediateStatus.NotImmediate))
		{
			if (TestFailedSinceLastClear == TestFailedSinceLastClearStatus.Undefined)
			{
				return newIncident.TestFailedSinceLastClear == TestFailedSinceLastClearStatus.TestNotFailedSinceLastClear;
			}
			return false;
		}
		return true;
	}
}
