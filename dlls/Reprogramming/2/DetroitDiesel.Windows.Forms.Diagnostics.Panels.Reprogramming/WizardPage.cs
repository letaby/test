using System.Windows.Forms;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public abstract class WizardPage
{
	private ReprogrammingView wizard;

	private HtmlElement element;

	private HtmlElement tab;

	internal bool Visible
	{
		set
		{
			element.SetAttribute("className", value ? "show" : "hide");
			if (tab != null)
			{
				ReprogrammingView.EnableElement(tab, value);
				tab.SetAttribute("className", value ? "tablinks active" : "tablinks");
			}
		}
	}

	internal HtmlElement Tab
	{
		set
		{
			tab = value;
		}
	}

	internal HtmlElement BackButton { get; set; }

	internal HtmlElement NextButton { get; set; }

	internal ReprogrammingView Wizard => wizard;

	internal WizardPage(ReprogrammingView wizard, HtmlElement element)
	{
		this.wizard = wizard;
		this.element = element;
	}

	internal virtual WizardPage OnWizardNext()
	{
		return wizard.Pages[wizard.Pages.IndexOf(wizard.ActivePage) + 1];
	}

	internal virtual WizardPage OnWizardBack()
	{
		return wizard.Pages[wizard.Pages.IndexOf(wizard.ActivePage) - 1];
	}

	internal abstract void OnSetActive();

	internal virtual void OnSetInactive()
	{
	}

	internal abstract void Navigate(string fragment);
}
