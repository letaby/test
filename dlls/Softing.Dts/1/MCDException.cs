// Decompiled with JetBrains decompiler
// Type: MCDException
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using Softing.Dts;
using System;

#nullable disable
public abstract class MCDException : Exception, MCDObject, IDisposable
{
  public void Dispose() => this.Dispose(true);

  protected virtual void Dispose(bool disposing)
  {
  }

  public abstract MCDError Error { get; }

  public abstract string SourceFile { get; }

  public abstract uint SourceLine { get; }

  public abstract MCDObjectType ObjectType { get; }
}
