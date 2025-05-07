using MessagePack;

using ProtoBuf;

namespace DataQuerySpeedTest.ServiceDefaults.Models;

[MessagePackObject, ProtoContract]
public sealed class Order
{
	[Key(0), ProtoMember(1)]
	public required int Id { get; init; }

	[Key(1), ProtoMember(2)]
	public required string ProductName { get; init; }

	[Key(2), ProtoMember(3)]
	public required decimal Price { get; init; }

	[Key(3), ProtoMember(4)]
	public required decimal Quantity { get; init; }

	[Key(4), ProtoMember(5)]
	public required decimal Amount { get; init; }
}
