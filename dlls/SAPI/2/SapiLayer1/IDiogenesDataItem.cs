namespace SapiLayer1;

public interface IDiogenesDataItem
{
	Channel Channel { get; }

	string Name { get; }

	string ShortName { get; }

	string Qualifier { get; }

	string Description { get; }

	string GroupName { get; }

	string GroupQualifier { get; }

	string Units { get; }

	ChoiceCollection Choices { get; }

	bool Visible { get; }

	object Precision { get; }

	Service CombinedService { get; }
}
