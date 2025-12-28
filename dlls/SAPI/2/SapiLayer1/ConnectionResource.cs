using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class ConnectionResource
{
	private readonly int? sourceAddress;

	private Ecu ecu;

	private EcuInterface ecuInterface;

	private string mcdInterfaceQualifier;

	private string mcdInterfaceResourceQualifier;

	private string type;

	private int portIndex;

	private string hardwareName;

	private bool restricted;

	private uint actualBaudRate;

	private bool isPassthru;

	internal string MCDInterfaceQualifier => mcdInterfaceQualifier;

	internal string MCDResourceQualifier => mcdInterfaceResourceQualifier;

	public string Type => type;

	public int PortIndex => portIndex;

	public string HardwareName => hardwareName;

	public bool IsPassThru => isPassthru;

	public Ecu Ecu => ecu;

	public EcuInterface Interface => ecuInterface;

	public bool Restricted
	{
		get
		{
			return restricted;
		}
		internal set
		{
			restricted = value;
		}
	}

	public int BaudRate => (int)actualBaudRate;

	internal byte? SourceAddress
	{
		get
		{
			if (!sourceAddress.HasValue)
			{
				return null;
			}
			return (byte)sourceAddress.Value;
		}
	}

	internal int? SourceAddressLong => sourceAddress;

	public string Identifier
	{
		get
		{
			if (!SourceAddress.HasValue)
			{
				return Ecu.Identifier;
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}-{1}", Ecu.ProtocolName, SourceAddress);
		}
	}

	public bool IsEthernet { get; }

	internal IEnumerable<DictionaryEntry> EcuInfoComParameters
	{
		get
		{
			foreach (DictionaryEntry ecuInfoComParameter in Ecu.EcuInfoComParameters)
			{
				yield return ecuInfoComParameter;
			}
			if (Interface == null)
			{
				yield break;
			}
			foreach (DictionaryEntry ecuInfoComParameter2 in Interface.EcuInfoComParameters)
			{
				yield return ecuInfoComParameter2;
			}
		}
	}

	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("ActualBaudRate is deprecated due to non-CLS compliance, please use BaudRate instead.")]
	public uint ActualBaudRate => actualBaudRate;

	internal ConnectionResource(Ecu ecu, EcuInterface ecuInterface, McdInterface theInterface, McdInterfaceResource theResource, int portIndex)
	{
		this.ecu = ecu;
		hardwareName = theInterface.HardwareName;
		type = theResource.PhysicalInterfaceLinkType;
		this.portIndex = portIndex;
		restricted = false;
		this.ecuInterface = ecuInterface;
		mcdInterfaceQualifier = theInterface.Qualifier;
		mcdInterfaceResourceQualifier = theResource.Qualifier;
		IsEthernet = theResource.IsEthernet;
		isPassthru = theResource.IsPassThru;
		if (!IsEthernet && (!isPassthru || RollCallJ1939.GlobalInstance == null || !RollCallJ1939.GlobalInstance.IsAutoBaudRate || !Sapi.GetSapi().AllowAutoBaudRate))
		{
			object obj = this.ecuInterface?.PrioritizedComParameterValue("CP_Baudrate");
			if (obj != null)
			{
				actualBaudRate = Convert.ToUInt32(obj, CultureInfo.InvariantCulture);
			}
		}
		if (this.ecuInterface != null && (!McdRoot.LocationPriority.Contains(this.ecuInterface.ProtocolName) || McdRoot.LocationRestricted.Contains(this.ecuInterface.ProtocolName)))
		{
			restricted = true;
		}
		else
		{
			restricted = this.ecu != null && !this.ecu.PassesConnectionResourceFilter(this);
		}
	}

	internal ConnectionResource(Ecu ecu, CaesarResource resource, byte? sourceAddress)
	{
		this.ecu = ecu;
		type = resource.Name;
		portIndex = resource.Number;
		hardwareName = resource.HardwareName;
		isPassthru = resource.IsPassThru;
		this.sourceAddress = sourceAddress;
		CaesarEcuInterface val = resource.EcuInterface;
		ecuInterface = this.ecu.Interfaces[val.Qualifier];
		if (!isPassthru || RollCallJ1939.GlobalInstance == null || !RollCallJ1939.GlobalInstance.IsAutoBaudRate || !Sapi.GetSapi().AllowAutoBaudRate)
		{
			object obj = ecuInterface.PrioritizedComParameterValue("CP_BAUDRATE");
			if (obj != null)
			{
				actualBaudRate = Convert.ToUInt32(obj, CultureInfo.InvariantCulture);
			}
		}
		restricted = !this.ecu.PassesConnectionResourceFilter(this);
	}

	internal ConnectionResource(DiagnosisProtocol protocol, CaesarResource resource, byte sourceAddress, uint desiredBaudRate)
	{
		ecu = new Ecu(protocol.Name + "-" + sourceAddress.ToString(CultureInfo.InvariantCulture), sourceAddress, protocol);
		type = resource.Name;
		portIndex = resource.Number;
		hardwareName = resource.HardwareName;
		isPassthru = resource.IsPassThru;
		this.sourceAddress = sourceAddress;
		actualBaudRate = desiredBaudRate;
	}

	internal ConnectionResource(DiagnosisProtocol protocol, McdInterface theInterface, McdInterfaceResource theResource, int portIndex, byte sourceAddress, uint desiredBaudRate)
	{
		ecu = new Ecu(protocol.Name + "-" + sourceAddress.ToString(CultureInfo.InvariantCulture), sourceAddress, protocol);
		type = theResource.PhysicalInterfaceLinkType;
		this.portIndex = portIndex;
		hardwareName = theInterface.HardwareName;
		this.sourceAddress = sourceAddress;
		actualBaudRate = desiredBaudRate;
		mcdInterfaceQualifier = theInterface.Qualifier;
		mcdInterfaceResourceQualifier = theResource.Qualifier;
		IsEthernet = theResource.IsEthernet;
	}

	internal ConnectionResource(Ecu ecu, Protocol protocol, string deviceName, uint baudRate, int portIndex, int sourceAddress)
	{
		this.ecu = ecu;
		restricted = true;
		actualBaudRate = baudRate;
		hardwareName = deviceName;
		type = protocol.ToString();
		this.portIndex = portIndex;
		this.sourceAddress = sourceAddress;
		isPassthru = true;
	}

	private ConnectionResource(Ecu ecu, string type, string hardwareName, int portIndex, uint? baudRate, byte? sourceAddress = null, string interfaceQualifier = null)
	{
		this.ecu = ecu;
		restricted = true;
		this.type = type;
		this.hardwareName = hardwareName;
		this.portIndex = portIndex;
		if (baudRate.HasValue)
		{
			actualBaudRate = baudRate.Value;
		}
		this.sourceAddress = sourceAddress;
		if (!string.IsNullOrEmpty(interfaceQualifier))
		{
			ecuInterface = ecu.Interfaces[interfaceQualifier];
		}
		IsEthernet = type == "ETHERNET" || type == "DoIP";
	}

	public static ConnectionResource FromString(Ecu ecu, string content)
	{
		if (content.Contains("NEXIQ WVL2 Dev:"))
		{
			content = content.Replace("Dev:", "Dev ");
		}
		string[] array = content.Split(":".ToCharArray(), StringSplitOptions.None);
		if (array.Length >= 4)
		{
			int result = 0;
			int num = 0;
			if (int.TryParse(array[2], out result))
			{
				num = result;
			}
			else
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(ecu, "Unable to parse the hardware index information");
			}
			string interfaceQualifier = null;
			byte? b = null;
			if (array.Length > 4)
			{
				if (byte.TryParse(array[4], out var result2))
				{
					b = result2;
				}
				if (array.Length > 5)
				{
					interfaceQualifier = array[5];
				}
			}
			if (uint.TryParse(array[3], out var result3))
			{
				return new ConnectionResource(ecu, array[0], array[1], num, result3, b, interfaceQualifier);
			}
			return new ConnectionResource(ecu, array[0], array[1], num, null, b, interfaceQualifier);
		}
		return null;
	}

	public override string ToString()
	{
		object[] source = new object[6]
		{
			type,
			hardwareName.Replace(":", " "),
			portIndex,
			actualBaudRate,
			sourceAddress,
			(ecuInterface != null && ecuInterface.Ecu.DiagnosisSource == DiagnosisSource.McdDatabase) ? ecuInterface.Qualifier : null
		};
		return string.Join(":", source.Select((object o) => (o == null) ? string.Empty : o));
	}

	internal bool IsEquivalent(ConnectionResource other)
	{
		bool flag = type == other.type && hardwareName == other.hardwareName && portIndex == other.portIndex && actualBaudRate == other.actualBaudRate;
		if (!flag && ecu != null && other.ecu != null && ecu.DiagnosisSource != other.ecu.DiagnosisSource)
		{
			ConnectionResource connectionResource = new ConnectionResource[2] { this, other }.FirstOrDefault((ConnectionResource cr) => cr.ecu.DiagnosisSource == DiagnosisSource.CaesarApi1 || cr.ecu.DiagnosisSource == DiagnosisSource.CaesarDatabase);
			ConnectionResource connectionResource2 = new ConnectionResource[2] { this, other }.FirstOrDefault((ConnectionResource cr) => cr.ecu.DiagnosisSource == DiagnosisSource.McdApi1 || cr.ecu.DiagnosisSource == DiagnosisSource.McdDatabase);
			if (connectionResource != null && connectionResource2 != null && connectionResource.type.StartsWith(connectionResource2.type, StringComparison.Ordinal) && connectionResource2.hardwareName.Equals(connectionResource.hardwareName + " (PDU-API)", StringComparison.Ordinal))
			{
				flag = portIndex == other.portIndex && actualBaudRate == other.actualBaudRate;
			}
		}
		return flag;
	}

	public override int GetHashCode()
	{
		return ToString().GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (obj != null)
		{
			return string.Equals(ToString(), obj.ToString());
		}
		return false;
	}
}
