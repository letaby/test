using System.Collections.Generic;
using System.Linq;
using Softing.Dts;

namespace McdAbstraction;

public class McdDBService : McdDBDiagComPrimitive
{
	private MCDDbService service;

	private IEnumerable<byte> requestMessage;

	private Dictionary<string, string> specialData;

	public override IEnumerable<byte> RequestMessage
	{
		get
		{
			if (requestMessage == null && base.DefaultPdu != null)
			{
				long num = base.AllRequestParameters.Max((McdDBRequestParameter rp) => rp.BytePos + rp.ByteLength);
				if (base.DefaultPdu.Count() < num)
				{
					byte[] array = new byte[num];
					base.DefaultPdu.ToArray().CopyTo(array, 0);
					requestMessage = array;
				}
				else
				{
					requestMessage = base.DefaultPdu;
				}
			}
			return requestMessage;
		}
	}

	public Dictionary<string, string> SpecialData
	{
		get
		{
			if (specialData == null)
			{
				specialData = McdDBDiagComPrimitive.GetSpecialData(service.DbSDGs);
			}
			return specialData;
		}
	}

	internal McdDBService(MCDDbService service)
		: base(service)
	{
		this.service = service;
	}
}
