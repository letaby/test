// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdOptionItem
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;

#nullable disable
namespace McdAbstraction;

public class McdOptionItem : IDisposable
{
  private MCDOptionItem optionItem;
  private McdDBOptionItem dbOptionItem;
  private bool disposedValue = false;

  internal McdOptionItem(MCDOptionItem optionItem, McdDBOptionItem dbOptionItem)
  {
    this.optionItem = optionItem;
    this.dbOptionItem = dbOptionItem;
  }

  public McdDBOptionItem DBOptionItem => this.dbOptionItem;

  public void SetItemValueByDBObject(McdDBItemValue value)
  {
    try
    {
      this.optionItem.ItemValueByDbObject = value.Handle;
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, "ItemValueByDBObject");
    }
  }

  protected virtual void Dispose(bool disposing)
  {
    if (this.disposedValue)
      return;
    if (disposing && this.optionItem != null)
    {
      this.optionItem.Dispose();
      this.optionItem = (MCDOptionItem) null;
    }
    this.disposedValue = true;
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }
}
