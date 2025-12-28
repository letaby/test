// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.EcuInfo.EcuInfoView
// Assembly: EcuInfo, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 02DA79A8-6904-4617-BF9C-5C5A1F77D676
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ECUInfo.dll

using DetroitDiesel.Reflection;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using System.ComponentModel;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.EcuInfo;

public class EcuInfoView : PanelBase
{
  public EcuInfoView()
    : base((PanelIdentifier) 0)
  {
    this.InitializeComponent();
  }

  protected virtual void Dispose(bool disposing) => base.Dispose(disposing);

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (EcuInfoView));
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Name = nameof (EcuInfoView);
    ((Control) this).ResumeLayout(false);
  }
}
