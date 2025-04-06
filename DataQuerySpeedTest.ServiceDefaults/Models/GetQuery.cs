using Mediator;

using MessagePack;

namespace DataQuerySpeedTest.ServiceDefaults.Models;

[MessagePackObject]
public sealed class GetQuery : IQuery<Order>
{
	[property: Key(0)]
	public required int Id { get; init; }
}
