using System.Linq;

namespace SapiLayer1;

public sealed class ParameterGroupCollection : LateLoadReadOnlyCollection<ParameterGroup>
{
	private Channel channel;

	public ParameterGroup this[string groupQualifier] => this.FirstOrDefault((ParameterGroup pg) => pg.Qualifier == groupQualifier);

	internal ParameterGroupCollection(Channel parent)
	{
		channel = parent;
	}

	protected override void AcquireList()
	{
		foreach (IGrouping<string, Parameter> item2 in from p in channel.Parameters
			group p by p.GroupQualifier)
		{
			ParameterGroup item = new ParameterGroup(item2.Key, item2.ToList());
			base.Items.Add(item);
		}
	}
}
