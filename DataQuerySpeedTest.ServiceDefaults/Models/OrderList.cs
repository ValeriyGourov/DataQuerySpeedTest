using ProtoBuf;

namespace DataQuerySpeedTest.ServiceDefaults.Models;

[ProtoContract]
public sealed class OrderList
{
	[ProtoMember(1)]
	public required IEnumerable<Order> Orders { get; init; }
}
