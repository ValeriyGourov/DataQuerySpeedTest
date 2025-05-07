using Mediator;

using MessagePack;

using ProtoBuf;

namespace DataQuerySpeedTest.ServiceDefaults.Models;

[MessagePackObject, ProtoContract]
public sealed class GetAllQuery : IQuery<IEnumerable<Order>>
{
	[property: Key(0), ProtoMember(1)]
	public required ushort PageSize { get; init; }
}
