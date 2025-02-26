using AutoFixture;

using Mediator;

using Server.Data;

namespace Server.Operations;

#pragma warning disable CA1515
public readonly record struct GetAllQuery(ushort PageSize = 20) : IQuery<IEnumerable<Order>>;
#pragma warning restore CA1515

#pragma warning disable CA1812
internal sealed class GetAllQueryHandler : IQueryHandler<GetAllQuery, IEnumerable<Order>>
{
	private static readonly Fixture _fixture = new();

	public ValueTask<IEnumerable<Order>> Handle(
		GetAllQuery query,
		CancellationToken cancellationToken)
	{
		IEnumerable<Order> orders = _fixture.CreateMany<Order>(query.PageSize);
		return ValueTask.FromResult(orders);
	}
}
#pragma warning restore CA1812
