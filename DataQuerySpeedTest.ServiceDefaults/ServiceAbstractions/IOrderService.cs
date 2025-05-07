using DataQuerySpeedTest.ServiceDefaults.Models;

using ProtoBuf.Grpc.Configuration;

namespace DataQuerySpeedTest.ServiceDefaults.ServiceAbstractions;

[Service]
public interface IOrderService
{
	ValueTask<Order> GetAsync(GetQuery query);
	ValueTask<OrderList> GetAllAsync(GetAllQuery query);
	ValueTask CreateAsync(CreateCommand command);
}
