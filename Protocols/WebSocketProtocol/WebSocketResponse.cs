namespace Protocols.WebSocketProtocol;

public readonly record struct WebSocketResponse<T>(T Data, long Size);
