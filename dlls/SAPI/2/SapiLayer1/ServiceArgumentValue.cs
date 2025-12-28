using System;

namespace SapiLayer1;

public sealed class ServiceArgumentValue
{
	private object value;

	private bool valuePreprocessed;

	private DateTime time;

	private object parent;

	public object Value => value;

	public bool ValuePreprocessed => valuePreprocessed;

	public DateTime Time => time;

	public ServiceInputValue InputValue => parent as ServiceInputValue;

	public ServiceOutputValue OutputValue => parent as ServiceOutputValue;

	internal ServiceArgumentValue(object serviceArgumentValue, DateTime time, object parent, bool valuePreprocessed)
	{
		value = serviceArgumentValue;
		this.valuePreprocessed = valuePreprocessed;
		this.time = time;
		this.parent = parent;
	}
}
