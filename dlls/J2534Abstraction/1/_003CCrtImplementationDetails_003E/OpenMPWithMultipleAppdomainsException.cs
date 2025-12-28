// Decompiled with JetBrains decompiler
// Type: <CrtImplementationDetails>.OpenMPWithMultipleAppdomainsException
// Assembly: J2534Abstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: F558D3F4-6D07-4AE0-B148-E7AD8371AFDC
// Assembly location: C:\Users\petra\Downloads\Архив (2)\J2534Abstraction.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace \u003CCrtImplementationDetails\u003E;

[Serializable]
internal class OpenMPWithMultipleAppdomainsException : System.Exception
{
  protected OpenMPWithMultipleAppdomainsException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }

  public OpenMPWithMultipleAppdomainsException()
  {
  }
}
