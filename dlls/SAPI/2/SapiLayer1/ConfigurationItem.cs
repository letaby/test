using System.Linq;

namespace SapiLayer1;

public sealed class ConfigurationItem
{
	private string name;

	private string value;

	private ChoiceCollection choices;

	internal object RawValue => choices.FirstOrDefault((Choice c) => c.Name == value || c.OriginalName == value).RawValue;

	public string Name => name;

	public string Value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	public ChoiceCollection Choices => choices;

	internal ConfigurationItem(string name, string defaultValue)
	{
		this.name = name;
		value = defaultValue;
		choices = new ChoiceCollection();
	}
}
