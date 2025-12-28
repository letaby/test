// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.WizardPage
// Assembly: Reprogramming, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 6E09671B-250E-411A-80FC-C490A3A17075
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Reprogramming.dll

using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public abstract class WizardPage
{
  private ReprogrammingView wizard;
  private HtmlElement element;
  private HtmlElement tab;

  internal WizardPage(ReprogrammingView wizard, HtmlElement element)
  {
    this.wizard = wizard;
    this.element = element;
  }

  internal virtual WizardPage OnWizardNext()
  {
    return this.wizard.Pages[this.wizard.Pages.IndexOf(this.wizard.ActivePage) + 1];
  }

  internal virtual WizardPage OnWizardBack()
  {
    return this.wizard.Pages[this.wizard.Pages.IndexOf(this.wizard.ActivePage) - 1];
  }

  internal abstract void OnSetActive();

  internal virtual void OnSetInactive()
  {
  }

  internal abstract void Navigate(string fragment);

  internal bool Visible
  {
    set
    {
      this.element.SetAttribute("className", value ? "show" : "hide");
      if (!(this.tab != (HtmlElement) null))
        return;
      ReprogrammingView.EnableElement(this.tab, value);
      this.tab.SetAttribute("className", value ? "tablinks active" : "tablinks");
    }
  }

  internal HtmlElement Tab
  {
    set => this.tab = value;
  }

  internal HtmlElement BackButton { get; set; }

  internal HtmlElement NextButton { get; set; }

  internal ReprogrammingView Wizard => this.wizard;
}
