#pragma warning disable VSTHRD200

using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

using DataQuerySpeedTest.ServiceDefaults.Models;

using Mediator;

using Protocols.WebSocketProtocol;

namespace Server.QueryBuilders;

internal static class WebSocketEndpoints
{
	private static readonly IWebSocketSubProtocol[] _supportedSubProtocols =
	[
		new JsonWebSocketSubprotocol(),
		new MessagePackWebSocketSubProtocol()
	];

	public static IEndpointRouteBuilder MapWebSocketEndpoints(this IEndpointRouteBuilder routes)
	{
		RouteGroupBuilder group = routes.MapGroup("WebSocket");

		MapEndpoint(group, Get);
		MapEndpoint(group, GetAll);
		MapEndpoint(group, Create);

		return routes;
	}

	private static void MapEndpoint(
		RouteGroupBuilder group,
		Delegate handler,
		[CallerArgumentExpression(nameof(handler))] string name = "")
		=> _ = group.Map($"/{name}", handler);

	private static Task Get(
		HttpContext context,
		IMediator mediator,
		CancellationToken cancellationToken)
		=> HandleQuery<GetQuery, Order>(context, mediator, cancellationToken);

	private static Task GetAll(
		HttpContext context,
		IMediator mediator,
		CancellationToken cancellationToken)
		=> HandleQuery<GetAllQuery, IEnumerable<Order>>(context, mediator, cancellationToken);

	private static Task Create(
		HttpContext context,
		IMediator mediator,
		CancellationToken cancellationToken)
		=> HandleCommand<CreateCommand>(context, mediator, cancellationToken);

	private static Task HandleQuery<TQuery, TResponce>(
		HttpContext context,
		IMediator mediator,
		CancellationToken cancellationToken)
		where TQuery : IQuery<TResponce>
		=> HandleMessage<TQuery, TResponce>(
			context,
			(query, cancellationToken) => mediator.Send((TQuery)query, cancellationToken),
			cancellationToken);

	private static Task HandleCommand<TCommand>(
		HttpContext context,
		IMediator mediator,
		CancellationToken cancellationToken)
		where TCommand : ICommand
		=> HandleMessage<TCommand, Unit>(
			context,
			(query, cancellationToken) => mediator.Send((TCommand)query, cancellationToken),
			cancellationToken);

	private static async Task HandleMessage<TMessage, TResponce>(
		HttpContext context,
		Func<IMessage, CancellationToken, ValueTask<TResponce>> responceHandler,
		CancellationToken cancellationToken)
		where TMessage : IMessage
	{
		cancellationToken.ThrowIfCancellationRequested();

		if (!context.WebSockets.IsWebSocketRequest)
		{
			context.Response.StatusCode = StatusCodes.Status400BadRequest;
			return;
		}

		IWebSocketSubProtocol subProtocol = NegotiateSubProtocol(context.WebSockets.WebSocketRequestedProtocols);

		using WebSocket webSocket = await context.WebSockets
			.AcceptWebSocketAsync(subProtocol.SubProtocol)
			.ConfigureAwait(false);

		while (!cancellationToken.IsCancellationRequested)
		{
			switch (webSocket.State)
			{
				case WebSocketState.Open:
					await HandleData<TMessage, TResponce>(webSocket, subProtocol, responceHandler, cancellationToken)
						.ConfigureAwait(false);
					break;

				case WebSocketState.CloseReceived:
					await webSocket
						.CloseAsync(
							WebSocketCloseStatus.NormalClosure,
							null,
							cancellationToken)
						.ConfigureAwait(false);
					return;

				default:
					return;
			}
		}
	}

	private static async Task HandleData<TMessage, TResponce>(
		WebSocket webSocket,
		IWebSocketSubProtocol subProtocol,
		Func<IMessage, CancellationToken, ValueTask<TResponce>> responceHandler,
		CancellationToken cancellationToken)
		where TMessage : IMessage
	{
		WebSocketResponse<TMessage> response;
		try
		{
			response = await subProtocol
				.ReceiveAsync<TMessage>(webSocket, cancellationToken)
				.ConfigureAwait(false);
		}
		catch (SerializationException exception)
		{
			await HandleInvalidPayloadDataAsync(
				webSocket,
				exception.Message,
				cancellationToken)
				.ConfigureAwait(false);

			return;
		}

		if (webSocket.State != WebSocketState.Open)
		{
			return;
		}

		TResponce result = await responceHandler(response.Data, cancellationToken)
			.ConfigureAwait(false);

		if (result is not Unit)
		{
			await subProtocol
				.SendAsync(result, webSocket, cancellationToken)
				.ConfigureAwait(false);
		}
	}

	private static Task HandleInvalidPayloadDataAsync(
		WebSocket webSocket,
		string description,
		CancellationToken cancellationToken)
		=> webSocket.CloseAsync(
			WebSocketCloseStatus.InvalidPayloadData,
			description,
			cancellationToken);

	private static IWebSocketSubProtocol NegotiateSubProtocol(IEnumerable<string> requestedSubProtocols)
		=> _supportedSubProtocols.First(subProtocol
			=> requestedSubProtocols.Contains(subProtocol.SubProtocol));
}
