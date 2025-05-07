using DataQuerySpeedTest.ServiceDefaults.Models;
using DataQuerySpeedTest.ServiceDefaults.ServiceAbstractions;

using Mediator;

namespace Server.QueryBuilders;

#pragma warning disable CA1812
internal sealed class GrpcService(IMediator mediator) : IOrderService
{
	private readonly IMediator _mediator = mediator;

	public ValueTask<Order> GetAsync(GetQuery query) => _mediator.Send(query);

	public async ValueTask<OrderList> GetAllAsync(GetAllQuery query)
	{
		IEnumerable<Order> orders = await _mediator
			.Send(query)
			.ConfigureAwait(false);
		return new() { Orders = orders };
	}

	public async ValueTask CreateAsync(CreateCommand command)
		=> _ = await _mediator
			.Send(command)
			.ConfigureAwait(false);
}
