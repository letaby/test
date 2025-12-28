using System;

namespace Softing.Dts;

internal class DtsLogicalLinkMonitorImpl : MappedObject, DtsLogicalLinkMonitor, DtsObject, MCDObject, IDisposable
{
	protected IntPtr m_dtsHandle = IntPtr.Zero;

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

	public uint RingBufferSize
	{
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_setRingBufferSize(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
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

	public DtsLogicalLinkMonitorImpl(IntPtr handle)
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
		if (Handle != IntPtr.Zero)
		{
			DTS_ObjectMapper.unregisterObject(Handle);
			Handle = IntPtr.Zero;
		}
	}

	~DtsLogicalLinkMonitorImpl()
	{
		Dispose(disposing: false);
	}

	public void AddAllLogicalLinkForMonitoring()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_addAllLogicalLinkForMonitoring(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void RemoveAllLogicalLinkForMonitoring()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_removeAllLogicalLinkForMonitoring(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void AddLogicalLinkForMonitoring(string NewLogicalLinkShortName)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_addLogicalLinkForMonitoring(Handle, NewLogicalLinkShortName);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void RemoveLogicalLinkFromMonitoring(string RemoveLogicalLinkShortName)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_removeLogicalLinkFromMonitoring(Handle, RemoveLogicalLinkShortName);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void Start()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_start(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void Stop()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_stop(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void OpenFileTrace(string FileName, bool bOverwrite)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_openFileTrace(Handle, FileName, bOverwrite);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void StartFileTrace()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_startFileTrace(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void StopFileTrace()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_stopFileTrace(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void CloseFileTrace()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_closeFileTrace(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public MCDCollection GetLatestEvents(uint uMaxNoOfNewEvents)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_getLatestEvents(Handle, uMaxNoOfNewEvents, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsCollection;
	}

	public void SetFilter(DtsLogicalLinkFilterConfig filterConfig, bool filterView, bool filterTrace)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_setFilter(Handle, DTS_ObjectMapper.getHandle(filterConfig as MappedObject), filterView, filterTrace);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void OpenFileTraceInFolder(string outputFolderPath, string FileName)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_openFileTraceInFolder(Handle, outputFolderPath, FileName);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void StartSnapshotModeTracing(uint timeInterval)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_startSnapshotModeTracing(Handle, timeInterval);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void GenerateSnapshotTrace(string outputPath)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_generateSnapshotTrace(Handle, outputPath);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void TakeSnapshotTrace()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_takeSnapshotTrace(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void StopSnapshotModeTracing()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_stopSnapshotModeTracing(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}
}
