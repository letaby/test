// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDatabaseExceptionImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;
using System.Text;

#nullable disable
namespace Softing.Dts;

internal class DtsDatabaseExceptionImpl : DtsDatabaseException
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDatabaseExceptionImpl(IntPtr handle)
  {
    this.Handle = handle;
    DTS_ObjectMapper.registerObject(this.Handle, (object) this);
  }

  protected override void Dispose(bool disposing)
  {
    if (!(this.Handle != IntPtr.Zero))
      return;
    this.Handle = IntPtr.Zero;
  }

  ~DtsDatabaseExceptionImpl() => this.Dispose(false);

  public override string Message => this.ToString();

  public override string ToString()
  {
    MCDError error = this.Error;
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendLine($"{error.CodeDescription.ToString()} (code = {((int) error.Code).ToString()})");
    stringBuilder.Append("Severity = ");
    stringBuilder.AppendLine(error.Severity.ToString());
    stringBuilder.Append("VendorCodeDescription = ");
    stringBuilder.AppendLine($"{error.VendorCodeDescription} (code = {((int) error.VendorCode).ToString()})");
    stringBuilder.AppendLine("");
    stringBuilder.Append("SourceFile = ");
    stringBuilder.AppendLine($"{this.SourceFile}({this.SourceLine.ToString()})");
    return stringBuilder.ToString();
  }

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public override MCDError Error
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr error = CSWrap.CSNIDTS_DtsException_getError(this.Handle, out returnValue);
      if (error != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(error);
      return (MCDError) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsError);
    }
  }

  public override string SourceFile
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr sourceFile = CSWrap.CSNIDTS_DtsException_getSourceFile(this.Handle, out returnValue);
      if (sourceFile != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(sourceFile);
      return returnValue.makeString();
    }
  }

  public override uint SourceLine
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr sourceLine = CSWrap.CSNIDTS_DtsException_getSourceLine(this.Handle, out returnValue);
      if (sourceLine != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(sourceLine);
      return returnValue;
    }
  }

  public override MCDObjectType ObjectType
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
}
