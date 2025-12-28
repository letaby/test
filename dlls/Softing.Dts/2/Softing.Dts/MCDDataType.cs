namespace Softing.Dts;

public enum MCDDataType : uint
{
	eA_ASCIISTRING = 1u,
	eA_BITFIELD = 2u,
	eA_BYTEFIELD = 3u,
	eA_FLOAT32 = 4u,
	eA_FLOAT64 = 5u,
	eA_INT16 = 6u,
	eA_INT32 = 7u,
	eA_INT64 = 8u,
	eA_INT8 = 9u,
	eA_UINT16 = 10u,
	eA_UINT32 = 11u,
	eA_UINT64 = 12u,
	eA_UINT8 = 13u,
	eA_UNICODE2STRING = 14u,
	eFIELD = 15u,
	eMULTIPLEXER = 16u,
	eSTRUCTURE = 17u,
	eTEXTTABLE = 18u,
	eA_BOOLEAN = 19u,
	eDTC = 20u,
	eENVDATA = 21u,
	eEND_OF_PDU = 22u,
	eTABLE = 23u,
	eENVDATADESC = 24u,
	eKEY = 25u,
	eLENGTHKEY = 26u,
	eTABLE_ROW = 27u,
	eSTRUCT_FIELD = 28u,
	eNO_TYPE = 255u
}
