using Mediator;

using MessagePack;

namespace DataQuerySpeedTest.ServiceDefaults.Models;

[MessagePackObject]
public sealed class GetAllQuery : IQuery<IEnumerable<Order>>
{
	[property: Key(0)]
	public ushort PageSize { get; init; } = 20;
}
