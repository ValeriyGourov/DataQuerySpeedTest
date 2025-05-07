using System.Security.Cryptography;

using DataQuerySpeedTest.ServiceDefaults.Models;

namespace Tester.Core;

public static class DataFactory
{
	public static ushort DefaultPageSize { get; } = 1000;

	public static int GetDataId() => RandomNumberGenerator.GetInt32(int.MaxValue);

	public static GetQuery NewGetQuery() => new()
	{
		Id = GetDataId()
	};

	public static GetAllQuery NewGetAllQuery() => new()
	{
		PageSize = DefaultPageSize
	};

	public static CreateCommand NewCreateCommand() => new()
	{
		ProductName = "Товар 1234",
		Quantity = 12.34m
	};
}
