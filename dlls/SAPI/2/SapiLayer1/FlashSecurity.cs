using System.Collections.Generic;
using System.Linq;
using System.Text;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class FlashSecurity
{
	private FlashDataBlock flashDataBlock;

	private string securityMethod;

	private string ecuKey;

	private string validity;

	private Dump fwSignature;

	private Dump checksum;

	public FlashDataBlock FlashDataBlock => flashDataBlock;

	public string SecurityMethod => securityMethod;

	public Dump FirmwareSignature => fwSignature;

	public Dump Checksum => checksum;

	public string EcuKey => ecuKey;

	public string Validity => validity;

	internal FlashSecurity(FlashDataBlock fdb)
	{
		flashDataBlock = fdb;
	}

	internal void Acquire(CaesarDICffSecur flashSecurityBlock)
	{
		IEnumerable<byte> enumerable = flashSecurityBlock.GetSecurityMethod();
		IEnumerable<byte> firmwareSignature = flashSecurityBlock.GetFirmwareSignature();
		IEnumerable<byte> enumerable2 = flashSecurityBlock.GetChecksum();
		IEnumerable<byte> enumerable3 = flashSecurityBlock.GetEcuKey();
		checksum = ((enumerable2 == null) ? null : new Dump(enumerable2));
		fwSignature = ((firmwareSignature == null) ? null : new Dump(firmwareSignature));
		securityMethod = ((enumerable == null) ? null : Encoding.UTF8.GetString(enumerable.ToArray()));
		ecuKey = ((enumerable3 == null) ? null : Encoding.UTF8.GetString(enumerable3.ToArray()));
	}

	internal void Acquire(McdDBFlashSecurity flashSecurityBLock)
	{
		IEnumerable<byte> flashwareSignature = flashSecurityBLock.FlashwareSignature;
		IEnumerable<byte> flashwareChecksum = flashSecurityBLock.FlashwareChecksum;
		checksum = ((flashwareChecksum == null) ? null : new Dump(flashwareChecksum));
		fwSignature = ((flashwareSignature == null) ? null : new Dump(flashwareSignature));
		securityMethod = flashSecurityBLock.SecurityMethod;
		validity = flashSecurityBLock.Validity;
	}
}
