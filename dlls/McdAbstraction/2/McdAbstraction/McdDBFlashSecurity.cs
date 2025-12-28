using System.Collections.Generic;
using Softing.Dts;

namespace McdAbstraction;

public class McdDBFlashSecurity
{
	private string securityMethod;

	private string validity;

	private IEnumerable<byte> flashwareSignature;

	private IEnumerable<byte> flashwareChecksum;

	public string SecurityMethod => securityMethod;

	public string Validity => validity;

	public IEnumerable<byte> FlashwareSignature => flashwareSignature;

	public IEnumerable<byte> FlashwareChecksum => flashwareChecksum;

	internal McdDBFlashSecurity(MCDDbFlashSecurity security)
	{
		securityMethod = ((security.SecurityMethod.DataType == MCDDataType.eNO_TYPE) ? null : security.SecurityMethod.ValueAsString);
		validity = ((security.Validity.DataType == MCDDataType.eNO_TYPE) ? null : security.Validity.ValueAsString);
		flashwareSignature = ((security.FlashwareSignature.DataType == MCDDataType.eA_BYTEFIELD) ? security.FlashwareSignature.Bytefield : null);
		flashwareChecksum = ((security.FlashwareChecksum.DataType == MCDDataType.eA_BYTEFIELD) ? security.FlashwareChecksum.Bytefield : null);
	}
}
