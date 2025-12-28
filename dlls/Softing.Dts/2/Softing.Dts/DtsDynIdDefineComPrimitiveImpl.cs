using System;

namespace Softing.Dts;

internal class DtsDynIdDefineComPrimitiveImpl : MappedObject, DtsDynIdDefineComPrimitive, MCDDynIdDefineComPrimitive, MCDDiagService, MCDDataPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable, DtsDiagService, DtsDataPrimitive, DtsDiagComPrimitive, DtsObject
{
	protected IntPtr m_dtsHandle = IntPtr.Zero;

	private uint handler_count;

	private uint listener_handle;

	public IntPtr Handle
	{
		get
		{
			return m_dtsHandle;
		}
		set
		{
			m_dtsHandle = value;
		}
	}

	public string[] DynIdParams
	{
		set
		{
			GC.KeepAlive(this);
			StringArray_Struct _paramnames = new StringArray_Struct(value);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDynIdDefineComPrimitive_setDynIdParams(Handle, ref _paramnames);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public MCDValue DynId
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDynIdDefineComPrimitive_getDynId(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDynIdDefineComPrimitive_setDynId(Handle, DTS_ObjectMapper.getHandle(value as MappedObject));
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public string DefinitionMode
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDynIdDefineComPrimitive_getDefinitionMode(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDynIdDefineComPrimitive_setDefinitionMode(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public ushort ResultBufferSize
	{
		get
		{
			GC.KeepAlive(this);
			ushort returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDataPrimitive_getResultBufferSize(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDataPrimitive_setResultBufferSize(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public MCDRepetitionState RepetitionState
	{
		get
		{
			GC.KeepAlive(this);
			MCDRepetitionState returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDataPrimitive_getRepetitionState(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public ushort RepetitionTime
	{
		get
		{
			GC.KeepAlive(this);
			ushort returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDataPrimitive_getRepetitionTime(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDataPrimitive_setRepetitionTime(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public MCDResultState ResultState
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDataPrimitive_getResultState(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResultState;
		}
	}

	public MCDDbDiagComPrimitive DbObject
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDiagComPrimitive_getDbObject(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDiagComPrimitive;
		}
	}

	public MCDErrors Errors
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDiagComPrimitive_getErrors(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsErrors;
		}
	}

	public MCDRequest Request
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDiagComPrimitive_getRequest(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsRequest;
		}
	}

	public MCDObject Parent
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDiagComPrimitive_getParent(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsObject;
		}
	}

	public uint UniqueRuntimeID
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDiagComPrimitive_getUniqueRuntimeID(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public MCDDiagComPrimitiveState State
	{
		get
		{
			GC.KeepAlive(this);
			MCDDiagComPrimitiveState returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDiagComPrimitive_getState(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public string InternalShortName
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDiagComPrimitive_getInternalShortName(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
	}

	public MCDObjectType ObjectType
	{
		get
		{
			GC.KeepAlive(this);
			MCDObjectType returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsObject_getObjectType(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public uint ObjectID
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsObject_getObjectID(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	internal event OnPrimitiveBufferOverflow __PrimitiveBufferOverflow;

	public event OnPrimitiveBufferOverflow PrimitiveBufferOverflow
	{
		add
		{
			lock (this)
			{
				__PrimitiveBufferOverflow += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__PrimitiveBufferOverflow -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnPrimitiveCanceledDuringExecution __PrimitiveCanceledDuringExecution;

	public event OnPrimitiveCanceledDuringExecution PrimitiveCanceledDuringExecution
	{
		add
		{
			lock (this)
			{
				__PrimitiveCanceledDuringExecution += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__PrimitiveCanceledDuringExecution -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnPrimitiveCanceledFromQueue __PrimitiveCanceledFromQueue;

	public event OnPrimitiveCanceledFromQueue PrimitiveCanceledFromQueue
	{
		add
		{
			lock (this)
			{
				__PrimitiveCanceledFromQueue += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__PrimitiveCanceledFromQueue -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnPrimitiveError __PrimitiveError;

	public event OnPrimitiveError PrimitiveError
	{
		add
		{
			lock (this)
			{
				__PrimitiveError += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__PrimitiveError -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnPrimitiveHasIntermediateResult __PrimitiveHasIntermediateResult;

	public event OnPrimitiveHasIntermediateResult PrimitiveHasIntermediateResult
	{
		add
		{
			lock (this)
			{
				__PrimitiveHasIntermediateResult += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__PrimitiveHasIntermediateResult -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnPrimitiveHasResult __PrimitiveHasResult;

	public event OnPrimitiveHasResult PrimitiveHasResult
	{
		add
		{
			lock (this)
			{
				__PrimitiveHasResult += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__PrimitiveHasResult -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnPrimitiveJobInfo __PrimitiveJobInfo;

	public event OnPrimitiveJobInfo PrimitiveJobInfo
	{
		add
		{
			lock (this)
			{
				__PrimitiveJobInfo += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__PrimitiveJobInfo -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnPrimitiveProgressInfo __PrimitiveProgressInfo;

	public event OnPrimitiveProgressInfo PrimitiveProgressInfo
	{
		add
		{
			lock (this)
			{
				__PrimitiveProgressInfo += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__PrimitiveProgressInfo -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnPrimitiveRepetitionStopped __PrimitiveRepetitionStopped;

	public event OnPrimitiveRepetitionStopped PrimitiveRepetitionStopped
	{
		add
		{
			lock (this)
			{
				__PrimitiveRepetitionStopped += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__PrimitiveRepetitionStopped -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnPrimitiveTerminated __PrimitiveTerminated;

	public event OnPrimitiveTerminated PrimitiveTerminated
	{
		add
		{
			lock (this)
			{
				__PrimitiveTerminated += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__PrimitiveTerminated -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	public DtsDynIdDefineComPrimitiveImpl(IntPtr handle)
	{
		Handle = handle;
		DTS_ObjectMapper.registerObject(Handle, this);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!(Handle != IntPtr.Zero))
		{
			return;
		}
		if (listener_handle != 0)
		{
			IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, listener_handle);
			if (intPtr != IntPtr.Zero)
			{
				CSWrap.CSNIDTS_releaseObject(intPtr);
			}
			listener_handle = 0u;
		}
		DTS_ObjectMapper.unregisterObject(Handle);
		Handle = IntPtr.Zero;
	}

	~DtsDynIdDefineComPrimitiveImpl()
	{
		Dispose(disposing: false);
	}

	public void StartRepetition()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDataPrimitive_startRepetition(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void StopRepetition()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDataPrimitive_stopRepetition(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void UpdateRepetitionParameters()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDataPrimitive_updateRepetitionParameters(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void ExecuteAsync()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDataPrimitive_executeAsync(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public MCDResults FetchResults(int numReq)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDataPrimitive_fetchResults(Handle, numReq, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResults;
	}

	public void StartCyclicSend(uint cyclicTime, int numSendCycles)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDataPrimitive_startCyclicSend(Handle, cyclicTime, numSendCycles);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void Cancel()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDiagComPrimitive_cancel(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public MCDResult ExecuteSync()
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDiagComPrimitive_executeSync(Handle, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResult;
	}

	public void ResetToDefaultValue(string parameterName)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDiagComPrimitive_resetToDefaultValue(Handle, parameterName);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void ResetToDefaultValues()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDiagComPrimitive_resetToDefaultValues(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public MCDResultState ExecuteSyncWithResultState()
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDiagComPrimitive_executeSyncWithResultState(Handle, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResultState;
	}

	public MCDResults ExecuteSyncWithResults()
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDiagComPrimitive_executeSyncWithResults(Handle, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResults;
	}

	internal bool _onPrimitiveBufferOverflow(MCDDiagComPrimitive primitive, MCDLogicalLink link)
	{
		lock (this)
		{
			if (this.__PrimitiveBufferOverflow != null)
			{
				this.__PrimitiveBufferOverflow(this, new PrimitiveBufferOverflowArgs(primitive, link));
				return true;
			}
		}
		return false;
	}

	internal bool _onPrimitiveCanceledDuringExecution(MCDDiagComPrimitive primitive, MCDLogicalLink link)
	{
		lock (this)
		{
			if (this.__PrimitiveCanceledDuringExecution != null)
			{
				this.__PrimitiveCanceledDuringExecution(this, new PrimitiveCanceledDuringExecutionArgs(primitive, link));
				return true;
			}
		}
		return false;
	}

	internal bool _onPrimitiveCanceledFromQueue(MCDDiagComPrimitive primitive, MCDLogicalLink link)
	{
		lock (this)
		{
			if (this.__PrimitiveCanceledFromQueue != null)
			{
				this.__PrimitiveCanceledFromQueue(this, new PrimitiveCanceledFromQueueArgs(primitive, link));
				return true;
			}
		}
		return false;
	}

	internal bool _onPrimitiveError(MCDDiagComPrimitive primitive, MCDLogicalLink link, MCDError error)
	{
		lock (this)
		{
			if (this.__PrimitiveError != null)
			{
				this.__PrimitiveError(this, new PrimitiveErrorArgs(primitive, link, error));
				return true;
			}
		}
		return false;
	}

	internal bool _onPrimitiveHasIntermediateResult(MCDDiagComPrimitive primitive, MCDLogicalLink link, MCDResultState resultstate)
	{
		lock (this)
		{
			if (this.__PrimitiveHasIntermediateResult != null)
			{
				this.__PrimitiveHasIntermediateResult(this, new PrimitiveHasIntermediateResultArgs(primitive, link, resultstate));
				return true;
			}
		}
		return false;
	}

	internal bool _onPrimitiveHasResult(MCDDiagComPrimitive primitive, MCDLogicalLink link, MCDResultState resultstate)
	{
		lock (this)
		{
			if (this.__PrimitiveHasResult != null)
			{
				this.__PrimitiveHasResult(this, new PrimitiveHasResultArgs(primitive, link, resultstate));
				return true;
			}
		}
		return false;
	}

	internal bool _onPrimitiveJobInfo(MCDDiagComPrimitive primitive, MCDLogicalLink link, string info)
	{
		lock (this)
		{
			if (this.__PrimitiveJobInfo != null)
			{
				this.__PrimitiveJobInfo(this, new PrimitiveJobInfoArgs(primitive, link, info));
				return true;
			}
		}
		return false;
	}

	internal bool _onPrimitiveProgressInfo(MCDDiagComPrimitive primitive, MCDLogicalLink link, byte progress)
	{
		lock (this)
		{
			if (this.__PrimitiveProgressInfo != null)
			{
				this.__PrimitiveProgressInfo(this, new PrimitiveProgressInfoArgs(primitive, link, progress));
				return true;
			}
		}
		return false;
	}

	internal bool _onPrimitiveRepetitionStopped(MCDDiagComPrimitive primitive, MCDLogicalLink link)
	{
		lock (this)
		{
			if (this.__PrimitiveRepetitionStopped != null)
			{
				this.__PrimitiveRepetitionStopped(this, new PrimitiveRepetitionStoppedArgs(primitive, link));
				return true;
			}
		}
		return false;
	}

	internal bool _onPrimitiveTerminated(MCDDiagComPrimitive primitive, MCDLogicalLink link, MCDResultState resultstate)
	{
		lock (this)
		{
			if (this.__PrimitiveTerminated != null)
			{
				this.__PrimitiveTerminated(this, new PrimitiveTerminatedArgs(primitive, link, resultstate));
				return true;
			}
		}
		return false;
	}
}
