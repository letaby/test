using System;

namespace Softing.Dts;

public interface MCDResponse : MCDNamedObject, MCDObject, IDisposable
{
	MCDResponseState State { get; }

	bool HasError { get; }

	MCDError Error { get; set; }

	MCDAccessKey AccessKeyOfLocation { get; }

	MCDValue ResponseMessage { get; }

	MCDDbResponse DbObject { get; }

	MCDResponseParameters ResponseParameters { get; }

	MCDObject Parent { get; }

	MCDValue ContainedResponseMessage { get; }

	ulong EndTime { get; }

	ulong StartTime { get; }
}
