// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsSmartComPrimitiveImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsSmartComPrimitiveImpl : 
  MappedObject,
  DtsSmartComPrimitive,
  DtsObject,
  MCDObject,
  IDisposable
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsSmartComPrimitiveImpl(IntPtr handle)
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

  ~DtsSmartComPrimitiveImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public string LogicalLinkLongName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr logicalLinkLongName = CSWrap.CSNIDTS_DtsSmartComPrimitive_getLogicalLinkLongName(this.Handle, out returnValue);
      if (logicalLinkLongName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(logicalLinkLongName);
      return returnValue.makeString();
    }
  }

  public string PhysicalInterfaceName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr physicalInterfaceName = CSWrap.CSNIDTS_DtsSmartComPrimitive_getPhysicalInterfaceName(this.Handle, out returnValue);
      if (physicalInterfaceName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(physicalInterfaceName);
      return returnValue.makeString();
    }
  }

  public string Description
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr description = CSWrap.CSNIDTS_DtsSmartComPrimitive_getDescription(this.Handle, out returnValue);
      if (description != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(description);
      return returnValue.makeString();
    }
  }

  public MCDObjectType ComPrimitiveType
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDObjectType returnValue;
      IntPtr comPrimitiveType = CSWrap.CSNIDTS_DtsSmartComPrimitive_getComPrimitiveType(this.Handle, out returnValue);
      if (comPrimitiveType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(comPrimitiveType);
      return returnValue;
    }
  }

  public string VariantLongName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr variantLongName = CSWrap.CSNIDTS_DtsSmartComPrimitive_getVariantLongName(this.Handle, out returnValue);
      if (variantLongName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(variantLongName);
      return returnValue.makeString();
    }
  }

  public MCDAccessKey AccessKey
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr accessKey = CSWrap.CSNIDTS_DtsSmartComPrimitive_getAccessKey(this.Handle, out returnValue);
      if (accessKey != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(accessKey);
      return (MCDAccessKey) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsAccessKey);
    }
  }

  public string LogicalLinkShortName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr logicalLinkShortName = CSWrap.CSNIDTS_DtsSmartComPrimitive_getLogicalLinkShortName(this.Handle, out returnValue);
      if (logicalLinkShortName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(logicalLinkShortName);
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
