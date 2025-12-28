namespace SapiLayer1;

public interface ICodingItem
{
	string Name { get; }

	string Description { get; }

	CodingChoiceCollection Choices { get; }

	Channel Channel { get; }
}
