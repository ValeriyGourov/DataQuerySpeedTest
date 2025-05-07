using Mediator;

using MessagePack;

using ProtoBuf;

namespace DataQuerySpeedTest.ServiceDefaults.Models;

[MessagePackObject, ProtoContract]
public sealed class GetQuery : IQuery<Order>
{
	[property: Key(0), ProtoMember(1)]
	public required int Id { get; init; }
}
