// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbSpecialDataElementImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbSpecialDataElementImpl : 
  MappedObject,
  DtsDbSpecialDataElement,
  MCDDbSpecialDataElement,
  MCDDbSpecialData,
  MCDObject,
  IDisposable,
  DtsDbSpecialData,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbSpecialDataElementImpl(IntPtr handle)
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

  ~DtsDbSpecialDataElementImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public string Content
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr content = CSWrap.CSNIDTS_DtsDbSpecialDataElement_getContent(this.Handle, out returnValue);
      if (content != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(content);
      return returnValue.makeString();
    }
  }

  public string TextID
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr textId = CSWrap.CSNIDTS_DtsDbSpecialDataElement_getTextID(this.Handle, out returnValue);
      if (textId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(textId);
      return returnValue.makeString();
    }
  }

  public string SemanticInformation
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr semanticInformation = CSWrap.CSNIDTS_DtsDbSpecialData_getSemanticInformation(this.Handle, out returnValue);
      if (semanticInformation != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(semanticInformation);
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
