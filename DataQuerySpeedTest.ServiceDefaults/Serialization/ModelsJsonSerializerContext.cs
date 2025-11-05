using System.Text.Json.Serialization;

using DataQuerySpeedTest.ServiceDefaults.Models;

namespace DataQuerySpeedTest.ServiceDefaults.Serialization;

[JsonSerializable(typeof(IEnumerable<Order>))]
[JsonSerializable(typeof(CreateCommand))]
[JsonSerializable(typeof(GetAllQuery))]
[JsonSerializable(typeof(GetQuery))]
public sealed partial class ModelsJsonSerializerContext : JsonSerializerContext;
