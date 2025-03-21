#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods

using DataQuerySpeedTest.ServiceDefaults.Models;

using Mediator;

using Microsoft.AspNetCore.Http.HttpResults;

namespace Server.QueryBuilders.HttpQueries;

internal static class RestEndpoints
{
	public static IEndpointRouteBuilder MapRestEndpoints(this IEndpointRouteBuilder routes)
	{
		RouteGroupBuilder group = routes.MapGroup("rest");

		_ = group
			.MapGet("/{id}", Get)
			.WithName(nameof(Get));

		_ = group
			.MapGet("/", GetAll)
			.WithName(nameof(GetAll));

		_ = group
			.MapPost("/", Create)
			.WithName(nameof(Create));

		return routes;
	}

	private static async ValueTask<Ok<Order>> Get(
		[AsParameters] GetQuery query,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		Order result = await mediator
			.Send(query, cancellationToken)
			.ConfigureAwait(false);

		return TypedResults.Ok(result);
	}

	private static async ValueTask<Ok<IEnumerable<Order>>> GetAll(
		[AsParameters] GetAllQuery query,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		IEnumerable<Order> result = await mediator
			.Send(query, cancellationToken)
			.ConfigureAwait(false);

		return TypedResults.Ok(result);
	}

	private static async ValueTask<Created> Create(
		CreateCommand command,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		_ = await mediator
			.Send(command, cancellationToken)
			.ConfigureAwait(false);

		return TypedResults.Created();
	}
}
