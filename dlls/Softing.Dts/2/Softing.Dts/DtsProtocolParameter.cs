using System;

namespace Softing.Dts;

public interface DtsProtocolParameter : DtsRequestParameter, MCDRequestParameter, MCDParameter, MCDNamedObject, MCDObject, IDisposable, DtsParameter, DtsNamedObject, DtsObject
{
}
