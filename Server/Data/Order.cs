namespace Server.Data;

#pragma warning disable CA1515 // Рассмотрите возможность сделать общедоступные типы внутренними
public readonly record struct Order(
	int Id,
	string ProductName,
	decimal Price,
	decimal Quantity,
	decimal Amount);
#pragma warning restore CA1515 // Рассмотрите возможность сделать общедоступные типы внутренними
