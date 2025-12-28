// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbCodeInformationImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbCodeInformationImpl : 
  MappedObject,
  DtsDbCodeInformation,
  MCDDbCodeInformation,
  MCDObject,
  IDisposable,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbCodeInformationImpl(IntPtr handle)
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

  ~DtsDbCodeInformationImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public string CodeFile
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr codeFile = CSWrap.CSNIDTS_DtsDbCodeInformation_getCodeFile(this.Handle, out returnValue);
      if (codeFile != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(codeFile);
      return returnValue.makeString();
    }
  }

  public string EntryPoint
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr entryPoint = CSWrap.CSNIDTS_DtsDbCodeInformation_getEntryPoint(this.Handle, out returnValue);
      if (entryPoint != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(entryPoint);
      return returnValue.makeString();
    }
  }

  public string Syntax
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr syntax = CSWrap.CSNIDTS_DtsDbCodeInformation_getSyntax(this.Handle, out returnValue);
      if (syntax != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(syntax);
      return returnValue.makeString();
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
