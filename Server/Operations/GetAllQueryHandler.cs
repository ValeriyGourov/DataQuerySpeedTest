using AutoFixture;

using DataQuerySpeedTest.ServiceDefaults.Models;

using Mediator;

namespace Server.Operations;

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
