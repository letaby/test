// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbEcuStateTransitionImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbEcuStateTransitionImpl : 
  MappedObject,
  DtsDbEcuStateTransition,
  MCDDbEcuStateTransition,
  MCDDbObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsDbObject,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbEcuStateTransitionImpl(IntPtr handle)
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

  ~DtsDbEcuStateTransitionImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDDbEcuStateTransitionActions DbEcuStateTransitionActions
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr transitionActions = CSWrap.CSNIDTS_DtsDbEcuStateTransition_getDbEcuStateTransitionActions(this.Handle, out returnValue);
      if (transitionActions != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(transitionActions);
      return (MCDDbEcuStateTransitionActions) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbEcuStateTransitionActions);
    }
  }

  public MCDDbExternalAccessMethod DbExternalAccessMethod
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr externalAccessMethod = CSWrap.CSNIDTS_DtsDbEcuStateTransition_getDbExternalAccessMethod(this.Handle, out returnValue);
      if (externalAccessMethod != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(externalAccessMethod);
      return (MCDDbExternalAccessMethod) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbExternalAccessMethod);
    }
  }

  public MCDDbEcuState DbSourceState
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbSourceState = CSWrap.CSNIDTS_DtsDbEcuStateTransition_getDbSourceState(this.Handle, out returnValue);
      if (dbSourceState != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbSourceState);
      return (MCDDbEcuState) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbEcuState);
    }
  }

  public MCDDbEcuState DbTargetState
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbTargetState = CSWrap.CSNIDTS_DtsDbEcuStateTransition_getDbTargetState(this.Handle, out returnValue);
      if (dbTargetState != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbTargetState);
      return (MCDDbEcuState) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbEcuState);
    }
  }

  public bool HasDbExternalAccessMethod
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbEcuStateTransition_hasDbExternalAccessMethod(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public string LongNameID
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr longNameId = CSWrap.CSNIDTS_DtsDbObject_getLongNameID(this.Handle, out returnValue);
      if (longNameId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(longNameId);
      return returnValue.makeString();
    }
  }

  public string DescriptionID
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr descriptionId = CSWrap.CSNIDTS_DtsDbObject_getDescriptionID(this.Handle, out returnValue);
      if (descriptionId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(descriptionId);
      return returnValue.makeString();
    }
  }

  public string UniqueObjectIdentifier
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr objectIdentifier = CSWrap.CSNIDTS_DtsDbObject_getUniqueObjectIdentifier(this.Handle, out returnValue);
      if (objectIdentifier != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectIdentifier);
      return returnValue.makeString();
    }
  }

  public string DatabaseFile
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr databaseFile = CSWrap.CSNIDTS_DtsDbObject_getDatabaseFile(this.Handle, out returnValue);
      if (databaseFile != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(databaseFile);
      return returnValue.makeString();
    }
  }

  public string Description
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr description = CSWrap.CSNIDTS_DtsNamedObject_getDescription(this.Handle, out returnValue);
      if (description != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(description);
      return returnValue.makeString();
    }
  }

  public string ShortName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr shortName = CSWrap.CSNIDTS_DtsNamedObject_getShortName(this.Handle, out returnValue);
      if (shortName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(shortName);
      return returnValue.makeString();
    }
  }

  public string LongName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr longName = CSWrap.CSNIDTS_DtsNamedObject_getLongName(this.Handle, out returnValue);
      if (longName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(longName);
      return returnValue.makeString();
    }
  }

  public uint StringID
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr stringId = CSWrap.CSNIDTS_DtsNamedObject_getStringID(this.Handle, out returnValue);
      if (stringId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(stringId);
      return returnValue;
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
