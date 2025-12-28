// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBControlPrimitive
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;

#nullable disable
namespace McdAbstraction;

public class McdDBControlPrimitive : McdDBDiagComPrimitive
{
  private string internalShortName;

  internal McdDBControlPrimitive(MCDDbControlPrimitive controlPrimitive)
    : base((MCDDbDiagComPrimitive) controlPrimitive)
  {
    switch (controlPrimitive)
    {
      case DtsDbStartCommunication startCommunication:
        this.internalShortName = startCommunication.InternalShortName;
        break;
      case DtsDbStopCommunication stopCommunication:
        this.internalShortName = stopCommunication.InternalShortName;
        break;
    }
  }

  public string InternalShortName => this.internalShortName;
}
