using OrderService.Application.Models.Carts;
using OrderService.Domain.ValueObjects;

namespace OrderService.Application.Models.Orders;

public class CreateOrderDto
{
    public OrderName? OrderName { get; set; }
    public Guid CustomerId { get; set; }
    public CartDto? Cart { get; set; }
    public Guid  MerchantId { get; set; }
}