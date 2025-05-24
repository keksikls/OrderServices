using OrderService.Domain.Enum;

namespace OrderService.Application.Models.Orders;

public class OrderDto : CreateOrderDto
{
    public Guid Id { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}