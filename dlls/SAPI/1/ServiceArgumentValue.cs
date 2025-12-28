// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ServiceArgumentValue
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;

#nullable disable
namespace SapiLayer1;

public sealed class ServiceArgumentValue
{
  private object value;
  private bool valuePreprocessed;
  private DateTime time;
  private object parent;

  internal ServiceArgumentValue(
    object serviceArgumentValue,
    DateTime time,
    object parent,
    bool valuePreprocessed)
  {
    this.value = serviceArgumentValue;
    this.valuePreprocessed = valuePreprocessed;
    this.time = time;
    this.parent = parent;
  }

  public object Value => this.value;

  public bool ValuePreprocessed => this.valuePreprocessed;

  public DateTime Time => this.time;

  public ServiceInputValue InputValue => this.parent as ServiceInputValue;

  public ServiceOutputValue OutputValue => this.parent as ServiceOutputValue;
}
