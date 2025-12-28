// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsTraceConfigImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsTraceConfigImpl : MappedObject, DtsTraceConfig, DtsObject, MCDObject, IDisposable
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsTraceConfigImpl(IntPtr handle)
  {
    this.Handle = handle;
    DTS_ObjectMapper.registerObject(this.Handle, (object) this);
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!(this.Handle != IntPtr.Zero))
      return;
    DTS_ObjectMapper.unregisterObject(this.Handle);
    this.Handle = IntPtr.Zero;
  }

  ~DtsTraceConfigImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public bool ClientCallTrace
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr clientCallTrace = CSWrap.CSNIDTS_DtsTraceConfig_getClientCallTrace(this.Handle, out returnValue);
      if (clientCallTrace != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(clientCallTrace);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setClientCallTrace(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool ClientCallTraceThreadContext
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr traceThreadContext = CSWrap.CSNIDTS_DtsTraceConfig_getClientCallTraceThreadContext(this.Handle, out returnValue);
      if (traceThreadContext != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(traceThreadContext);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setClientCallTraceThreadContext(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool ClientCallTraceFunctionCalls
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr traceFunctionCalls = CSWrap.CSNIDTS_DtsTraceConfig_getClientCallTraceFunctionCalls(this.Handle, out returnValue);
      if (traceFunctionCalls != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(traceFunctionCalls);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setClientCallTraceFunctionCalls(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool ClientCallTraceFunctionParameters
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr functionParameters = CSWrap.CSNIDTS_DtsTraceConfig_getClientCallTraceFunctionParameters(this.Handle, out returnValue);
      if (functionParameters != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(functionParameters);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setClientCallTraceFunctionParameters(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool ClientCallTraceExceptions
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr callTraceExceptions = CSWrap.CSNIDTS_DtsTraceConfig_getClientCallTraceExceptions(this.Handle, out returnValue);
      if (callTraceExceptions != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(callTraceExceptions);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setClientCallTraceExceptions(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool ClientCallTraceExtendedObjectInfo
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr extendedObjectInfo = CSWrap.CSNIDTS_DtsTraceConfig_getClientCallTraceExtendedObjectInfo(this.Handle, out returnValue);
      if (extendedObjectInfo != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(extendedObjectInfo);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setClientCallTraceExtendedObjectInfo(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool ClientCallTraceObjectLifetime
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr traceObjectLifetime = CSWrap.CSNIDTS_DtsTraceConfig_getClientCallTraceObjectLifetime(this.Handle, out returnValue);
      if (traceObjectLifetime != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(traceObjectLifetime);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setClientCallTraceObjectLifetime(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool ClientCallTraceFunctionTimings
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr traceFunctionTimings = CSWrap.CSNIDTS_DtsTraceConfig_getClientCallTraceFunctionTimings(this.Handle, out returnValue);
      if (traceFunctionTimings != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(traceFunctionTimings);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setClientCallTraceFunctionTimings(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool ClientCallTraceSuppressPointerAddress
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr suppressPointerAddress = CSWrap.CSNIDTS_DtsTraceConfig_getClientCallTraceSuppressPointerAddress(this.Handle, out returnValue);
      if (suppressPointerAddress != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(suppressPointerAddress);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setClientCallTraceSuppressPointerAddress(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public uint TraceMaxFileSize
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr traceMaxFileSize = CSWrap.CSNIDTS_DtsTraceConfig_getTraceMaxFileSize(this.Handle, out returnValue);
      if (traceMaxFileSize != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(traceMaxFileSize);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setTraceMaxFileSize(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool UseSubDirectory
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr useSubDirectory = CSWrap.CSNIDTS_DtsTraceConfig_getUseSubDirectory(this.Handle, out returnValue);
      if (useSubDirectory != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(useSubDirectory);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setUseSubDirectory(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool PduApiCallTrace
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr pduApiCallTrace = CSWrap.CSNIDTS_DtsTraceConfig_getPduApiCallTrace(this.Handle, out returnValue);
      if (pduApiCallTrace != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(pduApiCallTrace);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setPduApiCallTrace(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool PduApiCallTraceLogFunctionParameters
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr functionParameters = CSWrap.CSNIDTS_DtsTraceConfig_getPduApiCallTraceLogFunctionParameters(this.Handle, out returnValue);
      if (functionParameters != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(functionParameters);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setPduApiCallTraceLogFunctionParameters(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool PduApiCallTraceLogVersionInfo
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr traceLogVersionInfo = CSWrap.CSNIDTS_DtsTraceConfig_getPduApiCallTraceLogVersionInfo(this.Handle, out returnValue);
      if (traceLogVersionInfo != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(traceLogVersionInfo);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setPduApiCallTraceLogVersionInfo(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool PduApiCallTraceLogComParameterCalls
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr comParameterCalls = CSWrap.CSNIDTS_DtsTraceConfig_getPduApiCallTraceLogComParameterCalls(this.Handle, out returnValue);
      if (comParameterCalls != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(comParameterCalls);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setPduApiCallTraceLogComParameterCalls(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool PduApiCallTraceLogDetails
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr callTraceLogDetails = CSWrap.CSNIDTS_DtsTraceConfig_getPduApiCallTraceLogDetails(this.Handle, out returnValue);
      if (callTraceLogDetails != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(callTraceLogDetails);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setPduApiCallTraceLogDetails(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool DebugTrace
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr debugTrace = CSWrap.CSNIDTS_DtsTraceConfig_getDebugTrace(this.Handle, out returnValue);
      if (debugTrace != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(debugTrace);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setDebugTrace(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool LimitNumberOfTraceFiles
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr numberOfTraceFiles = CSWrap.CSNIDTS_DtsTraceConfig_getLimitNumberOfTraceFiles(this.Handle, out returnValue);
      if (numberOfTraceFiles != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(numberOfTraceFiles);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setLimitNumberOfTraceFiles(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool LimitNumberOfTraceSessions
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr numberOfTraceSessions = CSWrap.CSNIDTS_DtsTraceConfig_getLimitNumberOfTraceSessions(this.Handle, out returnValue);
      if (numberOfTraceSessions != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(numberOfTraceSessions);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setLimitNumberOfTraceSessions(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public uint MaxNumberOfTraceFiles
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr numberOfTraceFiles = CSWrap.CSNIDTS_DtsTraceConfig_getMaxNumberOfTraceFiles(this.Handle, out returnValue);
      if (numberOfTraceFiles != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(numberOfTraceFiles);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setMaxNumberOfTraceFiles(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public uint MaxNumberOfTraceSessions
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr numberOfTraceSessions = CSWrap.CSNIDTS_DtsTraceConfig_getMaxNumberOfTraceSessions(this.Handle, out returnValue);
      if (numberOfTraceSessions != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(numberOfTraceSessions);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setMaxNumberOfTraceSessions(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool UseTracePathForWritingSimFile
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr forWritingSimFile = CSWrap.CSNIDTS_DtsTraceConfig_getUseTracePathForWritingSimFile(this.Handle, out returnValue);
      if (forWritingSimFile != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(forWritingSimFile);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setUseTracePathForWritingSimFile(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool AppendSimFileTrace
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr appendSimFileTrace = CSWrap.CSNIDTS_DtsTraceConfig_getAppendSimFileTrace(this.Handle, out returnValue);
      if (appendSimFileTrace != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(appendSimFileTrace);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setAppendSimFileTrace(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool ClientCallTraceSuppressTimestamps
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr suppressTimestamps = CSWrap.CSNIDTS_DtsTraceConfig_getClientCallTraceSuppressTimestamps(this.Handle, out returnValue);
      if (suppressTimestamps != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(suppressTimestamps);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setClientCallTraceSuppressTimestamps(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool WriteMicroseconds
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr writeMicroseconds = CSWrap.CSNIDTS_DtsTraceConfig_getWriteMicroseconds(this.Handle, out returnValue);
      if (writeMicroseconds != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(writeMicroseconds);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setWriteMicroseconds(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool UseSystemTracePath
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr useSystemTracePath = CSWrap.CSNIDTS_DtsTraceConfig_getUseSystemTracePath(this.Handle, out returnValue);
      if (useSystemTracePath != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(useSystemTracePath);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setUseSystemTracePath(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public uint PduApiCallTraceMaxPduSize
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr callTraceMaxPduSize = CSWrap.CSNIDTS_DtsTraceConfig_getPduApiCallTraceMaxPduSize(this.Handle, out returnValue);
      if (callTraceMaxPduSize != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(callTraceMaxPduSize);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setPduApiCallTraceMaxPduSize(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool ClientCallTraceFunctionReturns
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr traceFunctionReturns = CSWrap.CSNIDTS_DtsTraceConfig_getClientCallTraceFunctionReturns(this.Handle, out returnValue);
      if (traceFunctionReturns != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(traceFunctionReturns);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setClientCallTraceFunctionReturns(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool ClientCallTraceSuppressThreadChanges
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr suppressThreadChanges = CSWrap.CSNIDTS_DtsTraceConfig_getClientCallTraceSuppressThreadChanges(this.Handle, out returnValue);
      if (suppressThreadChanges != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(suppressThreadChanges);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setClientCallTraceSuppressThreadChanges(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool PduApiCallTraceMergeIntoApiTrace
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr mergeIntoApiTrace = CSWrap.CSNIDTS_DtsTraceConfig_getPduApiCallTraceMergeIntoApiTrace(this.Handle, out returnValue);
      if (mergeIntoApiTrace != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(mergeIntoApiTrace);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setPduApiCallTraceMergeIntoApiTrace(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool DebugTraceToDebugOut
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr debugTraceToDebugOut = CSWrap.CSNIDTS_DtsTraceConfig_getDebugTraceToDebugOut(this.Handle, out returnValue);
      if (debugTraceToDebugOut != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(debugTraceToDebugOut);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr debugOut = CSWrap.CSNIDTS_DtsTraceConfig_setDebugTraceToDebugOut(this.Handle, value);
      if (debugOut != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(debugOut);
    }
  }

  public bool DebugTraceToApiTrace
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr debugTraceToApiTrace = CSWrap.CSNIDTS_DtsTraceConfig_getDebugTraceToApiTrace(this.Handle, out returnValue);
      if (debugTraceToApiTrace != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(debugTraceToApiTrace);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr apiTrace = CSWrap.CSNIDTS_DtsTraceConfig_setDebugTraceToApiTrace(this.Handle, value);
      if (apiTrace != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(apiTrace);
    }
  }

  public DtsDebugTraceLevel DebugTraceLevel
  {
    get
    {
      GC.KeepAlive((object) this);
      DtsDebugTraceLevel returnValue;
      IntPtr debugTraceLevel = CSWrap.CSNIDTS_DtsTraceConfig_getDebugTraceLevel(this.Handle, out returnValue);
      if (debugTraceLevel != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(debugTraceLevel);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setDebugTraceLevel(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool JobLogging
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr jobLogging = CSWrap.CSNIDTS_DtsTraceConfig_getJobLogging(this.Handle, out returnValue);
      if (jobLogging != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(jobLogging);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setJobLogging(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool RemoteLogging
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr remoteLogging = CSWrap.CSNIDTS_DtsTraceConfig_getRemoteLogging(this.Handle, out returnValue);
      if (remoteLogging != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(remoteLogging);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setRemoteLogging(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string RemoteLoggingAddress
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr remoteLoggingAddress = CSWrap.CSNIDTS_DtsTraceConfig_getRemoteLoggingAddress(this.Handle, out returnValue);
      if (remoteLoggingAddress != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(remoteLoggingAddress);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setRemoteLoggingAddress(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool PduApiCallTraceLogEvents
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr callTraceLogEvents = CSWrap.CSNIDTS_DtsTraceConfig_getPduApiCallTraceLogEvents(this.Handle, out returnValue);
      if (callTraceLogEvents != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(callTraceLogEvents);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsTraceConfig_setPduApiCallTraceLogEvents(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public MCDObjectType ObjectType
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDObjectType returnValue;
      IntPtr objectType = CSWrap.CSNIDTS_DtsObject_getObjectType(this.Handle, out returnValue);
      if (objectType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectType);
      return returnValue;
    }
  }

  public uint ObjectID
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr objectId = CSWrap.CSNIDTS_DtsObject_getObjectID(this.Handle, out returnValue);
      if (objectId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectId);
      return returnValue;
    }
  }
}
