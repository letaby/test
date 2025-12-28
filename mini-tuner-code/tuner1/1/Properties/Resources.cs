// Decompiled with JetBrains decompiler
// Type: TunerSolution.Properties.Resources
// Assembly: TunerSolution, Version=1.0.0.142, Culture=neutral, PublicKeyToken=null
// MVID: 9D02C703-4AB8-4296-B056-FAFCB6EB03BA
// Assembly location: C:\Users\petra\Downloads\TunerSolution\TunerSolution.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace TunerSolution.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resources
{
  private static ResourceManager resourceMan;
  private static CultureInfo resourceCulture;

  internal Resources()
  {
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static ResourceManager ResourceManager
  {
    get
    {
      if (TunerSolution.Properties.Resources.resourceMan == null)
        TunerSolution.Properties.Resources.resourceMan = new ResourceManager("TunerSolution.Properties.Resources", typeof (TunerSolution.Properties.Resources).Assembly);
      return TunerSolution.Properties.Resources.resourceMan;
    }
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static CultureInfo Culture
  {
    get => TunerSolution.Properties.Resources.resourceCulture;
    set => TunerSolution.Properties.Resources.resourceCulture = value;
  }
}
