using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class EcuInterfaceCollection : ReadOnlyCollection<EcuInterface>
{
	private Ecu ecu;

	public EcuInterface this[string qualifier] => this.FirstOrDefault((EcuInterface item) => string.Equals(item.Qualifier, qualifier, StringComparison.Ordinal));

	internal EcuInterfaceCollection(Ecu parent)
		: base((IList<EcuInterface>)new List<EcuInterface>())
	{
		ecu = parent;
	}

	internal void AcquireList()
	{
		//IL_00f7: Expected O, but got Unknown
		if (ecu.IsMcd)
		{
			IEnumerable<McdDBLogicalLink> dBLogicalLinksForEcu = McdRoot.GetDBLogicalLinksForEcu(ecu.Name);
			int num = 0;
			{
				foreach (McdDBLogicalLink item in dBLogicalLinksForEcu.OrderByDescending((McdDBLogicalLink ll) => McdRoot.LocationPriority.IndexOf(ll.ProtocolLocation.Qualifier)))
				{
					EcuInterface ecuInterface = new EcuInterface(ecu, num++);
					ecuInterface.Acquire(item);
					base.Items.Add(ecuInterface);
				}
				return;
			}
		}
		uint availableInterfaceTypeCount = CaesarRoot.GetAvailableInterfaceTypeCount(ecu.Name);
		for (uint num2 = 0u; num2 < availableInterfaceTypeCount; num2++)
		{
			try
			{
				CaesarEcuInterface interfaceByIndex = CaesarRoot.GetInterfaceByIndex(ecu.Name, num2);
				try
				{
					if (interfaceByIndex != null)
					{
						EcuInterface ecuInterface2 = new EcuInterface(ecu, (int)num2);
						ecuInterface2.Acquire(interfaceByIndex);
						base.Items.Add(ecuInterface2);
					}
				}
				finally
				{
					((IDisposable)interfaceByIndex)?.Dispose();
				}
			}
			catch (CaesarErrorException ex)
			{
				CaesarErrorException caesarError = ex;
				Sapi.GetSapi().RaiseExceptionEvent(ecu, new CaesarException(caesarError));
			}
		}
	}
}
