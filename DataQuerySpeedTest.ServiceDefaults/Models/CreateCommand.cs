using Mediator;

using MessagePack;

using ProtoBuf;

namespace DataQuerySpeedTest.ServiceDefaults.Models;

[MessagePackObject, ProtoContract]
public sealed class CreateCommand : ICommand
{
	[property: Key(0), ProtoMember(1)]
	public required string ProductName { get; init; }

	[property: Key(1), ProtoMember(2)]
	public required decimal Quantity { get; init; }
}
